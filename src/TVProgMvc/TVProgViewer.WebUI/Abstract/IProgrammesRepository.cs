using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.WebUI.MainServiceReferences;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.WebUI.Abstract
{
    public interface IProgrammesRepository
    {
        /// <summary>
        /// Получение системных передач за сейчас
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="category">Категория</param>
        Task<KeyValuePair<int,SystemProgramme[]>> GetSystemProgrammesAtNowAsyncList(int typeProgID, DateTimeOffset dateTimeOffset, string category, string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Получение пользовательских телепередач за сейчас
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="category">Категория</param>
        Task<SystemProgramme[]> GetUserProgrammesAtNowAsyncList(long uid, int typeProgID, DateTimeOffset dateTimeOffset, string category, string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Получение системных телепередач следующих после текущих
        /// </summary>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Текущее время</param>
        /// <param name="category">Категория</param>
        Task<KeyValuePair<int,SystemProgramme[]>> GetSystemProgrammesAtNextAsyncList(int typeProgID, DateTimeOffset dateTimeOffset, string category,
            string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Получение пользовательских телепередач следующих после текущих
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Текущее время</param>
        /// <param name="category">Категория</param>
        Task<SystemProgramme[]> GetUserProgrammesAtNextAsyncList(long uid, int typeProgID, DateTimeOffset dateTimeOffset, string category, string sidx, string sord, int page, int rows, string genres);

        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="cid">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        Task<SystemProgramme[]> GetUserProgrammesOfDayList(long uid, int typeProgID, int cid, DateTime tsStart, DateTime tsEnd, string category);

        /// <summary>
        /// Получение провайдера и типов телепередач
        /// </summary>
        Task<ProviderType[]> GetProviderTypeAsyncList();
        
        /// <summary>
        /// Получение всех категорий
        /// </summary>
        Task<IEnumerable<string>> GetCategories();

        /// <summary>
        /// Поиск по системной программе телепередач
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        Task<SystemProgramme[]> SearchProgramme(int typeProgID, string findTitle, string category,
                                                         string sidx, string sord, int page, int rows, string genres, string dates);
        
        /// <summary>
        /// Поиск по пользовательской программе телепередач
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Посиковая подстрока</param>
        Task<SystemProgramme[]> SearchUserProgramme(long uid, int typeProgID, string findTitle);

        /// <summary>
        /// Получение периода телепрограммы
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        Task<ProgPeriod> GetSystemProgrammePeriodAsync(int typeProgID);
    }
}
