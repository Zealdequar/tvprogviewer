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
    public class EFProgrammeRepository : BaseEFRepository, IProgrammesRepository
    {
        /// <summary>
        /// Получение системных передач за сейчас
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="category">Категория</param>
        public Task<KeyValuePair<int,SystemProgramme[]>> GetSystemProgrammesAtNowAsyncList(int typeProgID, DateTimeOffset dateTimeOffset, string category, string sidx, string sord, int page, int rows)
        {
            return Task<KeyValuePair<int, SystemProgramme[]>>.Factory.StartNew(() => { return TvProgService.GetSystemProgrammeList(typeProgID, dateTimeOffset, 1, category, sidx, sord, page, rows); });
        }

        /// <summary>
        /// Получение пользовательских телепередач за сейчас
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="dateTimeOffset">Время</param>
        /// <param name="category">Категория</param>
        public Task<SystemProgramme[]> GetUserProgrammesAtNowAsyncList(long uid, int typeProgID, DateTimeOffset dateTimeOffset, string category)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetUserProgrammeList(uid, typeProgID, dateTimeOffset, 1, category); });
        }

        /// <summary>
        /// Получение системных телепередач следующих после текущих
        /// </summary>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Текущее время</param>
        /// <param name="category">Категория</param>
        public Task<KeyValuePair<int, SystemProgramme[]>> GetSystemProgrammesAtNextAsyncList(int typeProgID, DateTimeOffset dateTimeOffset, string category,
            string sidx, string sord, int page, int rows)
        {
            return Task<KeyValuePair<int, SystemProgramme[]>>.Factory.StartNew(() => { return TvProgService.GetSystemProgrammeList(typeProgID, dateTimeOffset, 2, category, sidx, sord, page, rows); });
        }

        /// <summary>
        /// Получение пользовательских телепередач следующих после текущих
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Тип программы телепередач</param>
        /// <param name="dateTimeOffset">Текущее время</param>
        /// <param name="category">Категория</param>
        public Task<SystemProgramme[]> GetUserProgrammesAtNextAsyncList(long uid, int typeProgID, DateTimeOffset dateTimeOffset, string category)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetUserProgrammeList(uid, typeProgID, dateTimeOffset, 2, category); });
        }

        /// <summary>
        /// Получение пользовательских телепередач за день
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа программы телепередач</param>
        /// <param name="cid">Код канала</param>
        /// <param name="tsStart">Время начала выборки</param>
        /// <param name="tsEnd">Время завершения выборки</param>
        /// <param name="category">Категория</param>
        public Task<SystemProgramme[]> GetUserProgrammesOfDayList(long uid, int typeProgID, int cid, DateTime tsStart, DateTime tsEnd, string category)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.GetUserProgrammeDayList(uid, typeProgID, cid, tsStart, tsEnd, category); });
        }

        /// <summary>
        /// Получение провайдера и типов телепередач
        /// </summary>
        public Task<ProviderType[]> GetProviderTypeAsyncList()
        {
            return Task<ProviderType[]>.Factory.StartNew(() => {
                ProviderType[] provType = TvProgService.GetProviderTypeList();
                return provType;  });
        }

        /// <summary>
        /// Получение всех категорий
        /// </summary>
        public Task<IEnumerable<string>> GetCategories()
        {
            return Task<IEnumerable<string>>.Factory.StartNew(() => { return TvProgService.GetCategories(); });
        }

        /// <summary>
        /// Поиск по системной программе телепередач
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Поисковая подстрока</param>
        public Task<SystemProgramme[]> SearchProgramme(int typeProgID, string findTitle)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.SearchProgramme(typeProgID, findTitle); });
        }

        /// <summary>
        /// Поиск по пользовательской программе телепередач
        /// </summary>
        /// <param name="uid">Идентификатор пользователя</param>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        /// <param name="findTitle">Посиковая подстрока</param>
        public Task<SystemProgramme[]> SearchUserProgramme(long uid, int typeProgID, string findTitle)
        {
            return Task<SystemProgramme[]>.Factory.StartNew(() => { return TvProgService.SearchUserProgramme(uid, typeProgID, findTitle); });
        }

        /// <summary>
        /// Получение периода телепрограммы
        /// </summary>
        /// <param name="typeProgID">Идентификатор типа телепрограммы</param>
        public Task<ProgPeriod> GetSystemProgrammePeriodAsync(int typeProgID)
        {
            return Task<ProgPeriod>.Factory.StartNew(() => { return TvProgService.GetSystemProgrammePeriod(typeProgID); });
        }
    }
}