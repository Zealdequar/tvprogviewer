using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.Users;
using System.Transactions;
using TVProgViewer.WebUI.Infrastructure;

namespace TVProgViewer.WebUI.Controllers
{
    /// <summary>
    /// Контроллер для работы с телеканалами
    /// </summary>
    [Authorize]
    public class ChannelController : Controller
    {
        const long Max_Length = (long)8 * 1024 * 1024 * 1024;
        /// <summary>
        /// Репозиторий для каналов
        /// </summary>
        private IChannelsRepository _channelsRepository;
        private IUsersRepository _usersRepository;
        private long? UserId { get { return LazyGlobalist.Instance.UserId(System.Web.HttpContext.Current); } }
        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="channelsRepository">Репозиторий для телеканалов</param>
        public ChannelController(IChannelsRepository channelsRepository, IUsersRepository usersRepository)
        {
            _channelsRepository = channelsRepository;
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

        /// <summary>
        /// Получение пользовательских телеканалов в формате JSON
        /// </summary>
        /// <param name="tvProgProvider">Провайдер программы телепередач</param>
        /// <param name="progType">Тип программы телепередач</param>
        public async Task<ActionResult> GetUserChannels(int tvProgProvider, int progType)
        {
            if (UserId == null)
                return View();
            return Json(await _channelsRepository.GetUserInSystemChannels(UserId.Value, tvProgProvider, progType), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Обновление телеканалов пользователем
        /// </summary>
        /// <param name="tvProgProviderID">Идентификатор источника программы телеканалов</param>
        /// <param name="userChannelID">Идентификатор пользовательского телеканала</param>
        /// <param name="channelID">Системный индентификатор канала</param>
        /// <param name="visible">Видимость</param>
        /// <param name="userTitle">Пользовательское название</param>
        /// <param name="orderCol">Порядковый номер телеканала</param>
        /// <param name="diff">Смещение времени относительно Гринвича</param>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
        public void UpdateChannel(int tvProgProviderID, string userChannelID, int channelID, string visible, string userTitle, int orderCol, string diff)
        {
            int ucid = 0;
            if(int.TryParse(userChannelID, out ucid))
            {
                if (ucid != 0 && UserId.HasValue)
                {
                    if (visible.ToUpper() == "YES" || visible.ToUpper() == "ДА")
                    {
                        _channelsRepository.InsertUserChannel(ucid, UserId.Value, tvProgProviderID, channelID, userTitle, orderCol);
                    }
                    else
                    {
                        _channelsRepository.DeleteUserChannel(UserId.Value, channelID);
                    }
                }
                else
                {
                    _channelsRepository.InsertUserChannel(ucid, UserId.Value, tvProgProviderID, channelID, userTitle, orderCol);
                }
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, int userChannelId)
        {
            List<string> errors = new List<string>();
            if (file == null)
                return Json("Нет файла!", JsonRequestBehavior.AllowGet);
            if (userChannelId == 0)
                return Json("Телеканал ещё не был добавлен", JsonRequestBehavior.AllowGet);

            if (!(file.ContentType == "image/png" ||
                file.ContentType == "image/jpeg" ||
                file.ContentType == "image/jpg" ||
                file.ContentType == "image/gif" ))
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
                Image imgOrig = Image.FromStream(file.InputStream).GetThumbnailImage(45, 45, myCallback, IntPtr.Zero);
                Image img25 = Image.FromStream(file.InputStream).GetThumbnailImage(25, 25, myCallback, IntPtr.Zero);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!Directory.Exists($"{path}/small"))
                    Directory.CreateDirectory($"{path}/small");
                if (!Directory.Exists($"{path}/large"))
                    Directory.CreateDirectory($"{path}/large");

                string fileName = $"{Guid.NewGuid().ToString().ToUpper()}.{file.ContentType.Split('/')[1]}";
                string pathNameOrig = $@"{path}\large\{fileName}";
                string pathName25 = $@"{path}\small\{fileName}";
                
                using (TransactionScope tran = new TransactionScope())
                {
                    imgOrig.Save(pathNameOrig);
                    img25.Save(pathName25);
                    var length = new FileInfo(pathNameOrig).Length;
                    var length25 = new FileInfo(pathName25).Length;
                    string pathOrig = pathNameOrig.Substring(pathNameOrig.IndexOf("\\imgs"),
                                              pathNameOrig.IndexOf(fileName) - pathNameOrig.IndexOf("\\imgs"))
                                      .Replace('\\', '/');
                    string path25 = pathName25.Substring(pathName25.IndexOf("\\imgs"),
                                              pathName25.IndexOf(fileName) - pathName25.IndexOf("\\imgs"))
                                    .Replace('\\', '/');
                    _channelsRepository.ChangeChannelImage(UserId.Value, userChannelId, fileName, file.ContentType,
                        (int)length, (int)length25, pathOrig, path25);
                    tran.Complete();
                }
                    
            }
            catch (Exception ex)
            {
                //Лог
            }
                        
            return Json(errors, JsonRequestBehavior.AllowGet); 
        }
    }
}