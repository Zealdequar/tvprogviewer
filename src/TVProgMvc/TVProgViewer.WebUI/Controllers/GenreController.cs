using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TVProgViewer.BusinessLogic.Users;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.WebUI.Infrastructure;

namespace TVProgViewer.WebUI.Controllers
{
    /// <summary>
    /// Жанры
    /// </summary>
    public class GenreController : Controller
    {
        const long Max_Length = (long)8 * 1024 * 1024 * 1024;
        private IGenresRepository _genresRepository;
        private IUsersRepository _usersRepository;
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        private long? UserId { get { return LazyGlobalist.Instance.UserId(System.Web.HttpContext.Current); } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="genresRepository"></param>
        public GenreController(IGenresRepository genresRepository, IUsersRepository usersRepository)
        {
            _genresRepository = genresRepository;
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Вывод по умолчанию
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region Жанры

        /// <summary>
        /// Получение жанров в формате JSON
        /// </summary>
        public async Task<ActionResult> GetGenres()
        {
            if (UserId == null)
                return View();

            return Json(await _genresRepository.GetGenres(UserId.Value), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Добавление жанра
        /// </summary>
        /// <param name="genreName">Название жанра</param>
        /// <param name="visible">Видимость</param>
        [HttpPost]
        public void AddGenre(string genreName, string visible)
        {
            if (!UserId.HasValue)
                return;

            if (ModelState.IsValid)
            Session["GenreAddedId"] = _genresRepository.AddGenre(UserId.Value, genreName, null, visible.ToUpper() == "YES" || visible.ToUpper() == "ДА");
        }

        /// <summary>
        /// Обновление жанра
        /// </summary>
        /// <param name="genreId">Идентификатор жанра</param>
        /// <param name="genreName">Название жанра</param>
        /// <param name="visible">Видимость жанра</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void UpdateGenre(string genreId, string genreName, string visible)
        {
            long gid = 0;
            
            if (ModelState.IsValid && long.TryParse(genreId, out gid))
            {
                if (gid != 0 && UserId.HasValue)
                {
                    _genresRepository.UpdateGenre(gid, genreName, visible.ToUpper() == "YES" || visible.ToUpper() == "ДА");
                }
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// Загрузка пиктограммы для жанра
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="genreId">Идентификатор жанра</param>
        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, string genreId)
        {
            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                if (file == null)
                    return Json("Нет файла!", JsonRequestBehavior.AllowGet);

                long gid;

                if (!long.TryParse(genreId, out gid))
                {
                    long.TryParse(Session["GenreAddedId"].ToString(), out gid);
                    Session["GenreAddedId"] = 0;
                }

                if (gid == 0)
                    return Json("Жанр ещё не был добавлен", JsonRequestBehavior.AllowGet);

                if (!(file.ContentType == "image/png" ||
                    file.ContentType == "image/jpeg" ||
                    file.ContentType == "image/jpg" ||
                    file.ContentType == "image/gif"))
                    return Json("Некорректный формат файла", JsonRequestBehavior.AllowGet);
                if (file.ContentLength == 0)
                    return Json("Файл должен иметь размер", JsonRequestBehavior.AllowGet);
                if ((long)file.ContentLength > Max_Length)
                    return Json("Некорректный размер файла. Размер файла должен быть менее 8 Мб", JsonRequestBehavior.AllowGet);

                int errCode;
                if (UserId == null)
                    return Json("Не получен идентификатор пользователя", JsonRequestBehavior.AllowGet);
                User user = await _usersRepository.GetUser(UserId.Value, out errCode);
                string path = Server.MapPath($"~/imgs/{user.Catalog}");

                Image.GetThumbnailImageAbort myCallback =
                        new Image.GetThumbnailImageAbort(ThumbnailCallback);
                try
                {
                    Image img25 = Image.FromStream(file.InputStream).GetThumbnailImage(25, 25, myCallback, IntPtr.Zero);

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (!Directory.Exists($"{path}/genre"))
                        Directory.CreateDirectory($"{path}/genre");

                    string fileName = $"{Guid.NewGuid().ToString().ToUpper()}.{file.ContentType.Split('/')[1]}";
                    string pathNameOrig = $@"{path}\genre\{fileName}";
                    string pathName25 = $@"{path}\genre\{fileName}";

                    using (TransactionScope tran = new TransactionScope())
                    {
                        img25.Save(pathName25);
                        var length = new FileInfo(pathNameOrig).Length;
                        var length25 = new FileInfo(pathName25).Length;
                        string pathOrig = pathNameOrig.Substring(pathNameOrig.IndexOf("\\imgs"),
                                                  pathNameOrig.IndexOf(fileName) - pathNameOrig.IndexOf("\\imgs"))
                                          .Replace('\\', '/');
                        string path25 = pathName25.Substring(pathName25.IndexOf("\\imgs"),
                                                  pathName25.IndexOf(fileName) - pathName25.IndexOf("\\imgs"))
                                        .Replace('\\', '/');
                        _genresRepository.ChangeGenreImage(UserId.Value, gid, fileName, file.ContentType,
                            (int)length25, pathOrig, path25);
                        tran.Complete();
                    }
                }
                catch (Exception ex)
                {
                    //Лог
                }
            }
            
            return Json(errors, JsonRequestBehavior.AllowGet);
        }

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void DeleteGenre(string genreId)
        {
            long gid = 0;
            if (ModelState.IsValid && long.TryParse(genreId, out gid))
            {
                if (gid <= 0)
                    return;
                _genresRepository.DeleteGenre(gid);
            }
        }

        #endregion

        #region Классификатор жанров

        /// <summary>
        /// Получение классификатора жанров в формате JSON
        /// </summary>
        public async Task<ActionResult> GetGenreClassificators()
        {
            if (UserId == null)
                return View();

            return Json(await _genresRepository.GetGenreClassificators(UserId.Value), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Добавление элемента классификации жанров
        /// </summary>
        /// <param name="gid">Идентификатор жанра</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Удалится после даты</param>
        [HttpPost]
        public void AddGenreClassificator(long gid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate)
        {
            if (!UserId.HasValue)
                return;

            if (ModelState.IsValid)
                _genresRepository.AddGenreClassificator(gid, UserId.Value, containPhrases, nonContainPhrases, deleteAfterDate);
        }

        /// <summary>
        /// Обновление элемента классификации жанров
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификатора жанров</param>
        /// <param name="gid">Идентификатор жанра</param>
        /// <param name="containPhrases">Содержащая фраза</param>
        /// <param name="nonContainPhrases">Не содержащая фраза</param>
        /// <param name="deleteAfterDate">Удалится после наступления даты</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void UpdateGenreClassificator(string genreClassificatorId, long gid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate)
        {
            long gcid = 0;

            if (ModelState.IsValid && long.TryParse(genreClassificatorId, out gcid))
            {
                if (gid != 0 && UserId.HasValue)
                {
                    _genresRepository.UpdateGenreClassificator(gcid, gid, containPhrases, nonContainPhrases, deleteAfterDate);
                }
            }
        }

        /// <summary>
        /// Удаление элемента классификации жанра
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void DeleteGenreClassificator(string genreClassificatorId)
        {
            long gcid = 0;
            if (ModelState.IsValid && long.TryParse(genreClassificatorId, out gcid))
            {
                if (gcid <= 0)
                    return;
                _genresRepository.DeleteGenreClassificator(gcid);
            }
        }

        /// <summary>
        /// Поднять элемент классификации жанра выше по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void UpGenreClassificateElem(string genreClassificatorId)
        {
            long gcid = 0;
            if (ModelState.IsValid && long.TryParse(genreClassificatorId, out gcid))
            {
                if (gcid <= 0)
                    return;
                _genresRepository.UpGenreClassificateElem(gcid);
            }
        }

        /// <summary>
        /// Опустить элемент классификации жанра ниже по приоритету применимости
        /// </summary>
        /// <param name="genreClassificatorId">Идентификатор элемента классификации жанра</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void DownGenreClassificateElem(string genreClassificatorId)
        {
            long gcid = 0;
            if (ModelState.IsValid && long.TryParse(genreClassificatorId, out gcid))
            {
                if (gcid <= 0)
                    return;
                _genresRepository.DownGenreClassificateElem(gcid);
            }
        }

        #endregion
    }
}