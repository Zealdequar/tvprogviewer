using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.WebUI.Abstract
{
    public interface ITreeRepository
    {
        /// <summary>
        /// Получение периода телепрограммы
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        ProgPeriod GetSystemProgrammePeriod(int typeProgID);

        /// <summary>
        /// Получение пользовательских телеканалов
        /// </summary>
        /// <param name="uid">Код пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепередач</param>
        List<UserChannel> GetUserChannelList(long uid, int typeProgID);
    }
}