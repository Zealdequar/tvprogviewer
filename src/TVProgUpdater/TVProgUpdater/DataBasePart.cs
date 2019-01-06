using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using TVProgViewer.BusinessLogic.Updater;
using TVProgViewer.DataAccess.Adapters;

namespace TVProgUpdater
{
    public class DataBasePart
    {
        private static Logger _logger = LogManager.GetLogger("dbupdate");


        public static List<WebResource> GetWebResources()
        {
            UpdaterAdapter ua = new UpdaterAdapter();
            List<WebResource> listWebResources = new List<WebResource>();
            _logger.Info("Начало получения ресурсов из базы.");
             try
             {
                listWebResources = ua.GetWebResource();
                if (listWebResources != null)
                {
                 _logger.Trace("Ресурсы из базы успешно получены: {0}", string.Join(";", listWebResources.Select<WebResource,string>( 
                    wr => string.Format("WRID = '{0}', FileName = '{1}', ResourceName = '{2}', ResourceUrl ='{3}'",
                    wr.WebResourceID, wr.FileName, wr.ResourceName, wr.WrUri))));
                }
                else
                {
                    _logger.Error("Не удалось получить ресурсы из базы.");
                }
             }
             catch(Exception ex)
             {
                 _logger.Error("Не удалось получить ресурсы из базы. ErrMessage = '{0}'\nStackTrace = '{1}'", ex.Message, ex.StackTrace);
             }
            
            return listWebResources;
        }

    }
}
