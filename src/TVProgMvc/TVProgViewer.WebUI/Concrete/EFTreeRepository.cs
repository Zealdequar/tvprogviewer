using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TVProgViewer.WebUI.Abstract;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.WebUI.Concrete
{
    public class EFTreeRepository: BaseEFRepository, ITreeRepository
    {
        /// <summary>
        /// Получение периода телепрограммы
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        public ProgPeriod GetSystemProgrammePeriod(int typeProgID)
        {
            return TvProgService.GetSystemProgrammePeriod(typeProgID);
        }

        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        public List<UserChannel> GetUserChannelList(long uid, int typeProgID)
        {
            return TvProgService.GetUserChannelList(uid, typeProgID).ToList();
        }
    }
}