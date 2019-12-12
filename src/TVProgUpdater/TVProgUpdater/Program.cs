using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TVProgViewer.BusinessLogic.Updater;
using TVProgViewer.DataAccess.Adapters;
using NLog;
using System.Diagnostics;
using System.Xml.XPath;
using System.ServiceModel.Syndication;
using System.Globalization;
using static TVProgViewer.Common.Enums;

namespace TVProgUpdater
{
    class Program
    {
        private static Logger _logger = LogManager.GetLogger("dbupdate");
        private static Dictionary<string, string> _dictChanCodeOld = new Dictionary<string, string>();
        private static Dictionary<string, string> _dictChanCodeNew = new Dictionary<string,string>();
        private static bool IsDateTimeValid(string s)
        {
            CultureInfo provider = new CultureInfo("ru-Ru");
            string dateTimeString = string.Concat(s.Replace('.', ' '), DateTime.Now.Year);
            if (DateTime.Now <= DateTime.ParseExact(dateTimeString, "dd MMMM yyyy", provider))
              return true;
            else
            {
                _logger.Info(string.Concat("Актуальная программа есть только за ", dateTimeString));
                return false;
            }
            return false;
        }

        static void Main(string[] args)
        {
            _logger.Info(" =========== Начало обновления ============ ");
            Stopwatch sw = Stopwatch.StartNew();
            List<WebResource> listWebResources = DataBasePart.GetWebResources();
            foreach (WebResource wr in listWebResources)
            {
                try
                {
                    /*if (!string.IsNullOrWhiteSpace(wr.Rss))
                    {
                        try
                        {
                            using (var reader = XmlReader.Create(wr.Rss))
                            {
                                var formatter = new Rss20FeedFormatter();
                                formatter.ReadFrom(reader);
                                var feedItems = formatter.Feed.Items;
                                bool isSuccess1 = false, isSuccess2 = false;
                                foreach (var fi in feedItems)
                                {
                                    string feedContent = fi.Title.Text;
                                    Match m = Regex.Match(feedContent, @"(\d\d\s[а-я]+[\s\.])", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
                                    if (m.Success)
                                    {
                                        string s = m.Value;
                                        isSuccess1 = IsDateTimeValid(s);
                                        s = m.NextMatch().Value;
                                        isSuccess2 = IsDateTimeValid(s);
                                    }
                                }
                                if (!isSuccess1 && !isSuccess2)
                                    continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }*/
                    _logger.Info("Обновление для ID='{0}'", wr.WebResourceID);
                    Stopwatch swLoc = Stopwatch.StartNew();
                    string fileName = wr.FileName;
                    WebPart.GetWebTVProgramm(wr.WrUri, ref fileName);
                    wr.FileName = fileName;
                    XDocument xDoc = new XDocument();
                    if (wr.SourceType == TypeProg.XMLTV)
                    {
                        xDoc = XDocument.Load(wr.FileName, LoadOptions.SetLineInfo);
                        xDoc = WebPart.ReformatXDoc(xDoc);
                        if (wr.WebResourceID == 1)
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeOld[xChan.Element("display-name").Value] = id;
                            }
                        else if (wr.WebResourceID == 2)
                            foreach (XElement xChan in xDoc.XPathSelectElements("tv/channel"))
                            {
                                string id = xChan.Attribute("id").Value;
                                _dictChanCodeNew[xChan.Element("display-name").Value] = xChan.Attribute("id").Value;
                            }
                    }
                    else if (wr.SourceType == TypeProg.InterTV)
                    {
                        if (wr.WebResourceID == 3)
                            xDoc = ConverterProg.InterToXml(wr.FileName, _dictChanCodeOld);
                        else if (wr.WebResourceID == 4)
                            xDoc = ConverterProg.InterToXml(wr.FileName, _dictChanCodeNew);
                    }
                    wr.xmlDoc = xDoc.ToString();
                    _logger.Info("Каналы и тв-программа ID='{0}' подготовлены к запуску обработки в базе", wr.WebResourceID);
                    UpdaterAdapter ua = new UpdaterAdapter();
                    ua.ExecWebResource(wr.WebResourceID, wr.xmlDoc);
                    if (wr.WebResourceID == 2)
                    {
                        _logger.Info("Обработка каналов и тв-программы ID='{0}' завершена. Начало загрузки пиктограмм.", wr.WebResourceID);
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
                                    ua.UpdateIcon(Convert.ToInt32(id), src, id + ".gif", "image/gif", "gzip", 
                                        channelIcons);
                                }
                            }
                        }
                        swPict.Stop();
                        _logger.Info("Загрузка пиктограмм завершена. Потребовалось {0} сек.",  swPict.ElapsedMilliseconds/1000.0);
                    }
                    
                    swLoc.Stop();
                    _logger.Info("Обновление для ID='{0}' успешно завершено. Затрачено '{1}' сек.", wr.WebResourceID, swLoc.ElapsedMilliseconds/1000.0);
                }
                catch (Exception ex)
                {
                    _logger.Error("Обновление тв-программы прошло неуспешно: ErrMessage='{0}', StackTrace='{1}'", ex.Message, ex.StackTrace);
                }
            }
            sw.Stop();
            _logger.Info(" ========== Обновление завершено ========== (Затрачено: {0} сек.)", sw.ElapsedMilliseconds/1000.0);
        }

        /// <summary>
        /// Возвращает первый день недели по дате, используя текущую культуру
        /// </summary>
        /// <param name="dayInWeek"></param>
        /// <returns></returns>
        private static DateTime GetFirstDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        private static DateTime GetFirstDayOfWeek(DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime firstDayInWeek = dayInWeek.Date;

            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        private static DateTime GetFirstDayOfWeek(DateTime dayInWeek, string startDay)
        {
            DayOfWeek firstDay = ParseEnum<DayOfWeek>(startDay);
            return GetFirstDayOfWeek(dayInWeek, firstDay);
        }

        private static DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            return GetFirstDayOfWeek(dayInWeek, firstDay);
        }
        private static DateTime GetLastDayOfWeek(DateTime dayInWeek)
        {
            CultureInfo defaultCultureInfo = CultureInfo.CurrentCulture;
            return GetLastDayOfWeek(dayInWeek, defaultCultureInfo);
        }

        private static DateTime GetLastDayOfWeek(DateTime dayInWeek, DayOfWeek firstDay)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, firstDay);
            return firstDayInWeek.AddDays(7);
        }

        private static DateTime GetLastDayOfWeek(DateTime dayInWeek, string startDay)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, startDay);
            return firstDayInWeek.AddDays(7);
        }

        private static DateTime GetLastDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            DateTime firstDayInWeek = GetFirstDayOfWeek(dayInWeek, cultureInfo);
            return firstDayInWeek.AddDays(7);
        }

        private static Tuple<DateTime, DateTime> CampusVueDateRange(DateTime dayInWeek, string startDay)
        {
            DateTime firstDayOfWeek = GetFirstDayOfWeek(dayInWeek, startDay).AddSeconds(1);
            DateTime lastDayOfWeek = GetLastDayOfWeek(dayInWeek, startDay);

            return new Tuple<DateTime, DateTime>(firstDayOfWeek, lastDayOfWeek);
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
