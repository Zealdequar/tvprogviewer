using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.WebUI.Concrete
{
    public class EFChannelRepository: BaseEFRepository, IChannelsRepository
    {
        /// <summary>
        /// Получение карты пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="progProviderID">Идентификатор провайдера</param>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        public Task<SystemChannel[]> GetUserInSystemChannels(long uid, int progProviderID, int typeProgID)
        {
            return Task<SystemChannel[]>.Factory.StartNew(() => { return TvProgService.GetUserInSystemChannels(uid, progProviderID, typeProgID); });
        }

        /// <summary>
        /// Добавление пользовательского телеканала
        /// </summary>
        /// <param name="ucid">Идентификатор пользовательского телеканала</param>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="tvProgProviderId">Идентификатор источника телепрограммы</param>
        /// <param name="cid">Идентификатор телеканала</param>
        /// <param name="displayName">Пользовательское название телеканала</param>
        /// <param name="orderCol">Порядковый номер телеканала</param>
        public void InsertUserChannel(int ucid, long uid, int tvProgProviderId, int cid, string displayName, int orderCol)
        {
            TvProgService.InsertUserChannel(ucid, uid, tvProgProviderId, cid, displayName, orderCol);
        }

        /// <summary>
        /// Удаление пользовательского телеканала
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="cid">Мдентификатор телеканала</param>
        public void DeleteUserChannel(long uid, int cid)
        {
            TvProgService.DeleteUserChannel(uid, cid);
        }

        /// <summary>
        /// Смена пиктограммы телеканала
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="userChannelId">Идентификатор телеканала</param>
        /// <param name="fileName">Название файла</param>
        /// <param name="contentType">Тип содержимого</param>
        /// <param name="length">Размер в байтах большой пиктограммы</param>
        /// <param name="length25">Размер в байтах маленькой пиктограммы</param>
        /// <param name="pathOrig">Путь к большой пиктограмме</param>
        /// <param name="path25">Путь к маленькой пиктограмме</param>
        public void ChangeChannelImage(long uid, int userChannelId, string fileName, string contentType,
            int length, int length25, string pathOrig, string path25)
        {
            TvProgService.ChangeChannelImage(uid, userChannelId, fileName, contentType, length, length25, pathOrig, path25);
        }
    }
}