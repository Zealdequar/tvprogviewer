using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using TVProgViewer.Core.Domain.TvProgMain;
using TVProgViewer.Services.TvProgMain;

namespace TVProgViewer.TVProgUpdaterV2
{
    public class Updater : IUpdater
    {
        private static Logger _logger = LogManager.GetLogger("dbupdate");
        private static Dictionary<string, string> _dictChanCodeOld = new Dictionary<string, string>();
        private static Dictionary<string, string> _dictChanCodeNew = new Dictionary<string, string>();
        private readonly IUpdaterService _updaterService;

        public Updater(IUpdaterService updaterService)
        {
            _updaterService = updaterService;
        }
        public virtual async Task UpdateTvProgrammes()
        {
            _logger.Info(" =========== Начало обновления ============ ");
            Stopwatch sw = Stopwatch.StartNew();
            IList<WebResources> webResources = await _updaterService.GetAllWebResourcesAsync();

            _logger.Trace("Ресурсы из базы успешно получены: {0}", string.Join(";", webResources.ToList().Select<WebResources, string>(
                   wr => string.Format("WRID = '{0}', FileName = '{1}', ResourceName = '{2}', ResourceUrl ='{3}'",
                   wr.Id, wr.FileName, wr.ResourceName, wr.ResourceUrl))));

            foreach (WebResources webResource in webResources)
            {
                try
                {
                    _logger.Info("Обновление для ID='{0}'", webResource.Id);
                    Stopwatch swLoc = Stopwatch.StartNew();
                    string fileName = webResource.FileName;
                    WebPart.GetWebTVProgramm(new Uri(webResource.ResourceUrl), ref fileName);
                    webResource.FileName = fileName;
                    XDocument xDoc = new XDocument();
                    var sourceType = (await _updaterService.GetTypeProgByIdAsync(webResource.TypeProgId))
                        .TypeName == "Формат XMLTV" ? Enums.TypeProg.XMLTV : Enums.TypeProg.InterTV;
                    if (sourceType == Enums.TypeProg.XMLTV)
                    {
                        if (webResource.Id == 6)
                        {
                            string tempFile = Path.GetTempFileName();

                            using (var streamReader = new StreamReader(webResource.FileName))
                            using (var streamWriter = new StreamWriter(tempFile))
                            {
                                string line;

                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    if (!line.Contains("}{отт@бь)ч"))
                                        streamWriter.WriteLine(line);
                                }
                            }

                            File.Delete(webResource.FileName);
                            File.Move(tempFile, webResource.FileName); 
                        }

                        xDoc = XDocument.Load(webResource.FileName, LoadOptions.SetLineInfo);
                        xDoc = WebPart.ReformatXDoc(xDoc);
                        if (webResource.Id == 1)
                        {
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeOld[xChan.Element("display-name").Value] = id;
                            }
                        }
                        else if (webResource.Id == 2)
                        {
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeNew[xChan.Element("display-name").Value] = xChan.Attribute("id").Value;
                            }
                        }
                    }
                    else if (sourceType == Enums.TypeProg.InterTV)
                    {
                        if (webResource.Id == 3)
                            xDoc = ConverterProg.InterToXml(webResource.FileName, _dictChanCodeOld);
                        else if (webResource.Id == 4)
                            xDoc = ConverterProg.InterToXml(webResource.FileName, _dictChanCodeNew);
                    }

                    _logger.Info("Каналы и тв-программа ID='{0}' подготовлены к запуску обработки в базе", webResource.Id);
                    await _updaterService.RunXmlToDbAsync(webResource.Id, xDoc.ToString());

                    if (webResource.Id == 2)
                    {
                        _logger.Info("Обработка каналов и тв-программы ID='{0}' завершена. Начало загрузки пиктограмм.", webResource.Id);
                        Stopwatch swPict = Stopwatch.StartNew();
                        foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                        {
                            string id = xChan.Attribute("id").Value;
                            if (!string.IsNullOrWhiteSpace(id) && xChan.Element("icon") != null)
                            {
                                string src = xChan.Element("icon").Attribute("src").Value;
                                if (!string.IsNullOrWhiteSpace(src))
                                {
                                    byte[] channelIcons = WebPart.GetAndSaveIcon(id, src);
                                    await _updaterService.UpdateIconAsync(Convert.ToInt32(id), src, id + ".gif", "image/gif", "gzip",
                                        channelIcons);
                                }
                            }
                        }
                        swPict.Stop();
                        _logger.Info("Загрузка пиктограмм завершена. Потребовалось {0} сек.", swPict.ElapsedMilliseconds / 1000.0);
                    }
                    swLoc.Stop();
                    _logger.Info("Обновление для ID='{0}' успешно завершено. Затрачено '{1}' сек.", webResource.Id, swLoc.ElapsedMilliseconds / 1000.0);
                }
                catch (Exception ex)
                {
                    _logger.Error("Обновление тв-программы прошло неуспешно: ErrMessage='{0}', StackTrace='{1}'", ex.Message, ex.StackTrace);
                }
            }
            sw.Stop();
            _logger.Info(" ========== Обновление завершено ========== (Затрачено: {0} сек.)", sw.ElapsedMilliseconds / 1000.0);
        }
    }
}
