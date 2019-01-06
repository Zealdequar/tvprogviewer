using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using TVProgViewer.BusinessLogic.Updater;
using TVProgViewer.Common;

namespace TVProgViewer.DataAccess.Adapters
{
    /// <summary>
    /// Адаптер для робота-обновителя
    /// </summary>
    internal class UpdaterAdapter : AdapterBase
    {
        /// <summary>
        /// Получение ресурсов программ телепередач
        /// </summary>
        public List<WebResource> GetWebResource()
        {
            List<WebResource> wrList = new List<WebResource>();
            try
            {
                wrList = (from wr in dataContext.WebResources.AsNoTracking()
                          join tp in dataContext.TypeProg.AsNoTracking() on wr.TPID equals tp.TypeProgID
                          join prov in dataContext.TVProgProviders.AsNoTracking() on tp.TVProgProviderID equals prov.TVProgProviderID
                          select new
                          {
                              WebResourceID = wr.WebResourceID,
                              SourceType = (tp.TypeName == "Формат XMLTV") ?
                              Enums.TypeProg.XMLTV :
                              Enums.TypeProg.InterTV,
                              FileName = wr.FileName,
                              ResourceName = wr.ResourceName,
                              ResourceUrl = wr.ResourceUrl,
                              Rss = prov.Rss
                          }).ToList().Select(x => new WebResource()
                          {
                              WebResourceID = x.WebResourceID,
                              SourceType = x.SourceType,
                              FileName = x.FileName,
                              ResourceName = x.ResourceName,
                              WrUri = new Uri(x.ResourceUrl),
                              Rss = x.Rss
                          }).ToList();

            }
            catch (Exception ex)
            {
            }
            return wrList;
        }

        /// <summary>
        /// Запуск мигратора
        /// </summary>
        /// <param name="wrid">Идентификатор ресурса</param>
        /// <param name="xmlDoc">Программа телепередач в формате XML</param>
        public void ExecWebResource(int wrid, string xmlDoc)
        {
            List<SqlParameter> pars = new List<SqlParameter>
            {
                new SqlParameter("@WRID", SqlDbType.Int) {Value = wrid},
                new SqlParameter("@ChanAndProg", SqlDbType.Xml) {Value = xmlDoc}
            };
           da.ExecCommand(GetTvProgMainConnection(), "spUpdateXmlChansAndProgs",  pars.Cast<DbParameter>().ToList<DbParameter>());
        }

        /// <summary>
        /// Обновление данных о пиктограмме
        /// </summary>
        /// <param name="cid">Идентификатор канала</param>
        /// <param name="iconWebSrc">Адрес пиктограммы в интернете</param>
        /// <param name="channelIconName">Название пиктограммы</param>
        /// <param name="contentType">Тип содержимого (ContentType) пиктограммы</param>
        /// <param name="contentCoding">Кодировка пиктограммы</param>
        /// <param name="channelOrigIcon">Оригинальная пиктограмма</param>
        /// <param name="channel25Icon">Миниатюра 25x25 пиктограммы</param>
        public string UpdateIcon(int cid, string iconWebSrc, string channelIconName, string contentType, string contentCoding,
            byte[] channelOrigIcon)
        {
            List<SqlParameter> pars = new List<SqlParameter>
            {
                new SqlParameter("@CID", SqlDbType.Int) {Value = cid},
                new SqlParameter("@IconWebSrc", SqlDbType.NVarChar, iconWebSrc.Length) {Value = iconWebSrc},
                new SqlParameter("@ChannelIconName", SqlDbType.NVarChar, channelIconName.Length) {Value = channelIconName},
                new SqlParameter("@ContentType", SqlDbType.NVarChar, contentType.Length) {Value = contentType},
                new SqlParameter("@ContentCoding", SqlDbType.NVarChar, contentCoding.Length) {Value = contentCoding},
                new SqlParameter("@ChannelOrigIcon", SqlDbType.VarBinary, channelOrigIcon.Length) {Value = channelOrigIcon},
                new SqlParameter("@IsSystem", SqlDbType.Bit) {Value = 1}
            };
            SqlParameter oPar = new SqlParameter("@ErrCode", SqlDbType.NVarChar, 4000);
            oPar.Direction = ParameterDirection.Output;
            pars.Add(oPar);
            da.ExecCommand(GetTvProgMainConnection(), "spUdtChannelImage", pars.Cast<DbParameter>().ToList<DbParameter>());
            return oPar.Value != null ? oPar.Value.ToString() : null;
        }
    }
}