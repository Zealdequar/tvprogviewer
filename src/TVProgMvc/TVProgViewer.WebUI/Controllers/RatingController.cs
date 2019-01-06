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
using TVProgViewer.BusinessLogic.Users;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.WebUI.Infrastructure;

namespace TVProgViewer.WebUI.Controllers
{
    /// <summary>
    /// Рейтинги
    /// </summary>
    public class RatingController : Controller
    {
        const long Max_Length = (long)8 * 1024 * 1024 * 1024;
        private IRatingsRepository _ratingsRepository;
        private IUsersRepository _usersRepository;

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        private long? UserId { get { return LazyGlobalist.Instance.UserId(System.Web.HttpContext.Current); } }

        /// <summary>
        /// Конструктор
        /// </summary>
        public RatingController(IRatingsRepository ratingsRepository, IUsersRepository usersRepository)
        {
            _ratingsRepository = ratingsRepository;
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Вывод по умолчанию
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        #region Рейтинги

        /// <summary>
        /// Получение рейтингов в формате JSON
        /// </summary>
        public async Task<ActionResult> GetRatings()
        {
            if (!UserId.HasValue)
                return View();

            return Json(await _ratingsRepository.GetRatings(UserId.Value), JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Добавление рейтинга
        /// </summary>
        /// <param name="ratingName">Название рейтинга</param>
        /// <param name="visible">Видимость</param>
        [HttpPost]
        public void AddRating(string ratingName, string visible)
        {
            if (!UserId.HasValue)
                return;

            if (ModelState.IsValid)
                Session["RatingAddedId"] = _ratingsRepository.AddRating(UserId.Value, ratingName, null, visible.ToUpper() == "YES" || visible.ToUpper() == "ДА");
        }

        /// <summary>
        /// Обновление рейтинга
        /// </summary>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        /// <param name="ratingName">Название рейтинга</param>
        /// <param name="visible">Видимость рейтинга</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void UpdateRating(string ratingId, string ratingName, string visible)
        {
            long rid = 0;

            if (ModelState.IsValid && long.TryParse(ratingId, out rid))
            {
                if (rid != 0 && UserId.HasValue)
                {
                    _ratingsRepository.UpdateRating(rid, ratingName, visible.ToUpper() == "YES" || visible.ToUpper() == "ДА");
                }
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// Загрузка пиктограммы для рейтинга
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="ratingId">Идентификатор рейтинга</param>
        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, string ratingId)
        {
            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                if (file == null)
                    return Json("Нет файла!", JsonRequestBehavior.AllowGet);

                long rid;

                if (!long.TryParse(ratingId, out rid))
                {
                    long.TryParse(Session["RatingAddedId"].ToString(), out rid);
                    Session["RatingAddedId"] = 0;
                }

                if (rid == 0)
                    return Json("Рейтинг ещё не был добавлен", JsonRequestBehavior.AllowGet);

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
                    if (!Directory.Exists($"{path}/fav"))
                        Directory.CreateDirectory($"{path}/fav");

                    string fileName = $"{Guid.NewGuid().ToString().ToUpper()}.{file.ContentType.Split('/')[1]}";
                    string pathNameOrig = $@"{path}\fav\{fileName}";
                    string pathName25 = $@"{path}\fav\{fileName}";

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
                        _ratingsRepository.ChangeRatingImage(UserId.Value, rid, fileName, file.ContentType,
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
        public void DeleteRating(string ratingId)
        {
            long rid = 0;
            if (ModelState.IsValid && long.TryParse(ratingId, out rid))
            {
                if (rid <= 0)
                    return;
                _ratingsRepository.DeleteRating(rid);
            }
        }

        #endregion

        #region Классификатор рейтингов

        /// <summary>
        /// Получение классификатора рейтингов в формате JSON
        /// </summary>
        public async Task<ActionResult> GetRatingClassificators()
        {
            if (!UserId.HasValue)
                return View();

            return Json(await _ratingsRepository.GetRatingClassificators(UserId.Value), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Добавление элемента классификации рейтингов
        /// </summary>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="containPhrases">Содержащаяся фраза</param>
        /// <param name="nonContainPhrases">Не содержащаяся фраза</param>
        /// <param name="deleteAfterDate">Удалится после даты</param>
        [HttpPost]
        public void AddRatingClassificator(long rid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate)
        {
            if (!UserId.HasValue)
                return;

            if (ModelState.IsValid)
                _ratingsRepository.AddRatingClassificator(rid, UserId.Value, containPhrases, nonContainPhrases, deleteAfterDate);
        }

        /// <summary>
        /// Обновление элемента классификации рейтингов
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификатора рейтингов</param>
        /// <param name="rid">Идентификатор рейтинга</param>
        /// <param name="containPhrases">Содержащая фраза</param>
        /// <param name="nonContainPhrases">Не содержащая фраза</param>
        /// <param name="deleteAfterDate">Удалится после наступления даты</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void UpdateRatingClassificator(string ratingClassificatorId, long rid, string containPhrases, string nonContainPhrases, DateTime? deleteAfterDate)
        {
            long rcid = 0;

            if (ModelState.IsValid && long.TryParse(ratingClassificatorId, out rcid))
            {
                if (rid != 0 && UserId.HasValue)
                {
                    _ratingsRepository.UpdateRatingClassificator(rcid, rid, containPhrases, nonContainPhrases, deleteAfterDate);
                }
            }
        }

        /// <summary>
        /// Удаление элемента классификации рейтингов
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void DeleteRatingClassificator(string ratingClassificatorId)
        {
            long rcid = 0;
            if (ModelState.IsValid && long.TryParse(ratingClassificatorId, out rcid))
            {
                if (rcid <= 0)
                    return;
                _ratingsRepository.DeleteRatingClassificator(rcid);
            }
        }

        /// <summary>
        /// Поднять элемент классификации рейтинга выше по приоритету применимости
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификации рейтинга</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void UpRatingClassificateElem(string ratingClassificatorId)
        {
            long rcid = 0;
            if (ModelState.IsValid && long.TryParse(ratingClassificatorId, out rcid))
            {
                if (rcid <= 0)
                    return;
                _ratingsRepository.UpRatingClassificateElem(rcid);
            }
        }

        /// <summary>
        /// Опустить элемент классификации рейтинга ниже по приоритету применимости
        /// </summary>
        /// <param name="ratingClassificatorId">Идентификатор элемента классификации рейтинга</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void DownRatingClassificateElem(string ratingClassificatorId)
        {
            long rcid = 0;
            if (ModelState.IsValid && long.TryParse(ratingClassificatorId, out rcid))
            {
                if (rcid <= 0)
                    return;
                _ratingsRepository.DownRatingClassificateElem(rcid);
            }
        }
        #endregion
    }
}