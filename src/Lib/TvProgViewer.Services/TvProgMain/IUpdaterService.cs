using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.TvProgMain;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Интерфейс для работы с обновлениями
    /// </summary>
    public partial interface IUpdaterService
    {
        /// <summary>
        /// Получение всех веб-ресурсов
        /// </summary>
        /// <returns></returns>
        public Task<IList<WebResources>> GetAllWebResourcesAsync(bool showHidden = false);

        /// <summary>
        /// Получение типа программы по идентификатору
        /// </summary>
        /// <param name="typeProgId">Идентификатор типа программы</param>
        /// <returns>Тип программы</returns>
        public Task<TypeProg> GetTypeProgByIdAsync(int typeProgId);

        /// <summary>
        /// Запуск хранимой процедуры обработки и обновления телепрограммы
        /// </summary>
        /// <param name="webResourceId">Идентификатор веб-ресурса</param>
        /// <param name="tvProgXml">Xml-формат телепрограммы</param>
        /// <returns></returns>
        public Task RunXmlToDbAsync(int webResourceId, string tvProgXml);

        /// <summary>
        /// Обновление телепрограммы
        /// </summary>
        public Task UpdateTvProgrammes();
    }
}