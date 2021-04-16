using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;

namespace TVProgViewer.TVProgUpdaterV2
{
    public class ConverterProg
    {
        private static Logger _logger = LogManager.GetLogger("convertupdate");
       
        /// <summary>
        /// Конвертер Интер-ТВ телепрограммы в XMLTV
        /// </summary>
        /// <param name="interFile">Файл с Интер-ТВ программы</param>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public static XDocument InterToXml(string interFile, Dictionary<string, string> dictChans)
        {
            _logger.Info("Начало конвертации Интер-ТВ в XMLTV");
            Stopwatch sw = Stopwatch.StartNew(); 
            Dictionary<string, string> dictMonths = new Dictionary<string, string>() // - словарь подстановок для месяцев
                                                        {
                                                            {"января", "01"},
                                                            {"февраля", "02"},
                                                            {"марта", "03"},
                                                            {"апреля", "04"},
                                                            {"мая", "05"},
                                                            {"июня", "06"},
                                                            {"июля", "07"},
                                                            {"августа", "08"},
                                                            {"сентября", "09"},
                                                            {"октября", "10"},
                                                            {"ноября", "11"},
                                                            {"декабря", "12"}
                                                        };
            int j = 0;
            try
            {
                List<string> chans = new List<string>(); // - список для каналов

                _logger.Trace("Открываем файл с телепрограммой в формате Интер-ТВ: '{0}'", interFile);
                FileStream fs = new FileStream(interFile, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("Windows-1251"), true); // - поток чтения
                int i = 0; // - для определения идентификаторов
                XDocument xdoc = new XDocument();  // - создать xml-файл для выгрузки конвертации в XML TV
                XElement xtv = new XElement("tv");
                xdoc.Add(xtv);
                _logger.Trace("Парсинг каналов");
                
                while (!sr.EndOfStream)
                { // Читаем до конца потока:
                    string strLine = sr.ReadLine();
                    if (!String.IsNullOrEmpty(strLine))
                    {
                        if (Regex.IsMatch(strLine, "понедельник")) // Формат: ПОНЕДЕЛЬНИК. Дата: День Месяц. Имя канала. Продолжение имени канала
                        {
                            string[] title = strLine.Split('.'); // - парсить для извлечения имени канала
                            if (title.Length == 3)
                            {
                                chans.Add(title[2].Trim());
                            }
                            else if (title.Length == 4)
                            {
                                chans.Add((title[2] + title[3]).Trim());
                            }
                        }
                    }
                }
                chans = chans.Distinct().ToList(); // - удалить дубликаты
                foreach (string chan in chans)
                { // Назаначить id и записать канал в xml 
                    i++;
                    XElement xChannel = new XElement("channel");
                    xtv.Add(xChannel);
                    string chanId = String.Empty;
                    dictChans.TryGetValue(chan.Trim(), out chanId);
                    xChannel.Add(new XAttribute("id",
                                                String.IsNullOrEmpty(chanId)
                                                    ? ((!dictChans.ContainsValue(i.ToString()))
                                                           ? i.ToString()
                                                           : (dictChans.Values.Max() + 1).ToString())
                                                    : chanId));
                    xChannel.Add(new XElement("display-name", chan));
                    dictChans[chan] = String.IsNullOrEmpty(chanId)
                                         ? ((!dictChans.ContainsValue(i.ToString()))
                                                ? i.ToString()
                                                : (dictChans.Values.Max() + 1).ToString())
                                         : chanId;
                }
                sr.BaseStream.Position = 0; // - каретку в начало

                string date = String.Empty;  // - для сохранения даты
                DateTime datetime = DateTime.MinValue;    // - начальная дата
                DateTime curDateTime = DateTime.MinValue; // - текущая дата
                string id = String.Empty;                 // - для хранения идентификатора канала
                string[] strArr = new string[2];   // - для работы со строками
                string[] chanName = new string[2]; // - для охвата каналов
                long ticks = 0;                    // - тики для приращения времени
                bool prNotice = false;             // - флаг для определения сохранения анонсов
                string preNotice = String.Empty;
                string titleEnd = String.Empty;
                StringBuilder noticeStr = new StringBuilder(); // - для хранения анонсов
                XElement xProgramme = null;        // - элемент для программы        
                _logger.Trace("Парсинг тв-программ");
                
                while (!sr.EndOfStream)
                {
                    j++;
                    if (prNotice && !String.IsNullOrEmpty(preNotice))
                    {
                        strArr[0] = preNotice; // - сохранить анонс в строке
                        prNotice = false;
                    }
                    else
                    {
                        strArr[0] = strArr[1];  // - буфферная строка
                        strArr[1] = sr.ReadLine();
                    }
                    if (!String.IsNullOrEmpty(strArr[0]))
                    {
                        if (Regex.IsMatch(strArr[0], "понедельник|вторник|среда|четверг|пятница|суббота|воскресенье", RegexOptions.Compiled))
                        {

                            string[] title = strArr[0].Split('.'); // - парсить строку-заголовок
                            if (title.Length == 3)
                            {
                                string[] strDate = title[1].Split(' '); // - парсить для извлечения даты
                                if (strDate.Length == 3)
                                {
                                    chanName[0] = chanName[1];
                                    chanName[1] = title[2];
                                    if (chanName[0] != chanName[1]) // - в рамках одного и того же канала 
                                    {
                                        ticks = 0;      // - обнулить тики
                                        // Начальная дата (от неё и отталкиваемся):
                                        datetime = new DateTime(DateTime.Now.Year, int.Parse(dictMonths[strDate[2]]),
                                                                int.Parse(strDate[1]));

                                        dictChans.TryGetValue(title[2].Trim(), out id); // - извлечение идентифкатора канала
                                    }
                                }
                            }
                            if (title.Length == 4)
                            { // Название канала длиннее
                                string[] strDate = title[1].Split(' ');
                                if (strDate.Length == 3)
                                {
                                    chanName[0] = chanName[1];
                                    chanName[1] = title[2] + title[3];
                                    if (chanName[0] != chanName[1])
                                    {
                                        ticks = 0;
                                        date = DateTime.Now.Year.ToString() + dictMonths[strDate[2]] + strDate[1];
                                        dictChans.TryGetValue((title[2] + title[3]).Trim(), out id);
                                    }
                                }
                            }
                        }
                    }
                    // Реальная строка с захватом, буфферная пуста
                    if (!String.IsNullOrEmpty(strArr[1]) && 
                        Regex.IsMatch(strArr[1], @"^\s*\d{2}:\d{2}", RegexOptions.Compiled) && 
                        String.IsNullOrEmpty(strArr[0]))
                    {//Конечная передача есть:
                        if (!String.IsNullOrEmpty(titleEnd))
                        {// - записать в элемент xml
                            xProgramme = new XElement("programme");
                            xtv.Add(xProgramme);
                            if (ticks == 0)
                            {
                                ticks = (new TimeSpan(0, int.Parse(titleEnd.Trim().Split(' ')[0].Split(':')[0]),
                                                      int.Parse(titleEnd.Trim().Split(' ')[0].Split(':')[1]), 0).Ticks);
                                curDateTime = datetime.AddTicks(ticks);
                                string strStartDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("start", strStartDate));
                                TimeSpan tsend = new TimeSpan(0, int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[0]),
                                                              int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[1]), 0);
                                TimeSpan tsstart = new TimeSpan(0, int.Parse(titleEnd.Trim().Split(' ')[0].Split(':')[0]),
                                                                int.Parse(titleEnd.Trim().Split(' ')[0].Split(':')[1]), 0);
                                if (tsend < tsstart)
                                {
                                    tsend += (new TimeSpan(1, 0, 0, 0));
                                }
                                ticks = (tsend.Ticks - tsstart.Ticks);

                                curDateTime = curDateTime.AddTicks(ticks);
                                string strEndDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("stop", strEndDate));
                            }
                            else
                            {
                                string strStartDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("start", strStartDate));
                                TimeSpan tsend = new TimeSpan(0, int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[0]),
                                                              int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[1]), 0);
                                TimeSpan tsstart = new TimeSpan(0, int.Parse(titleEnd.Trim().Split(' ')[0].Split(':')[0]),
                                                                int.Parse(titleEnd.Trim().Split(' ')[0].Split(':')[1]), 0);
                                if (tsend < tsstart)
                                {
                                    tsend += (new TimeSpan(1, 0, 0, 0));
                                }
                                ticks = (tsend.Ticks - tsstart.Ticks);
                                curDateTime = curDateTime.AddTicks(ticks);
                                string strEndDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("stop", strEndDate));
                            }
                            if (xProgramme != null)
                            {
                                xProgramme.Add(new XAttribute("channel", id.ToString()));
                                xProgramme.Add(new XElement("title", titleEnd.Substring(6)));
                            }
                            if (noticeStr.Length > 0)
                            {
                                if (xProgramme != null)
                                {
                                    xProgramme.Add(new XElement("desc", noticeStr.ToString()));
                                    //  Debug.WriteLine(noticeStr.ToString());
                                    noticeStr.Clear();
                                }
                            }
                            preNotice = String.Empty;
                            prNotice = false;
                            titleEnd = String.Empty;
                        }
                    }
                    // Есть и буфферная и реальная строка
                    if (!String.IsNullOrEmpty(strArr[0]) && !String.IsNullOrEmpty(strArr[1]))
                    {
                        if (Regex.IsMatch(strArr[0], @"^\s*\d{2}:\d{2}\s", RegexOptions.Compiled) && 
                            Regex.IsMatch(strArr[1], @"^\s*\d{2}:\d{2}\s", RegexOptions.Compiled))
                        {
                            xProgramme = new XElement("programme");
                            xtv.Add(xProgramme);
                            if (ticks == 0)
                            {
                                ticks = (new TimeSpan(0, int.Parse(strArr[0].Trim().Split(' ')[0].Split(':')[0]),
                                                          int.Parse(strArr[0].Trim().Split(' ')[0].Split(':')[1]), 0).Ticks);
                                curDateTime = datetime.AddTicks(ticks);
                                string strStartDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("start", strStartDate));
                                TimeSpan tsend = new TimeSpan(0, int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[0]),
                                                              int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[1]), 0);
                                TimeSpan tsstart = new TimeSpan(0, int.Parse(strArr[0].Trim().Split(' ')[0].Split(':')[0]),
                                                                int.Parse(strArr[0].Trim().Split(' ')[0].Split(':')[1]), 0);
                                if (tsend < tsstart)
                                {
                                    tsend += (new TimeSpan(1, 0, 0, 0));
                                }
                                ticks = (tsend.Ticks - tsstart.Ticks);

                                curDateTime = curDateTime.AddTicks(ticks);
                                string strEndDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("stop", strEndDate));
                            }
                            else
                            {
                                string strStartDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("start", strStartDate));
                                TimeSpan tsend = new TimeSpan(0, int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[0]),
                                                                 int.Parse(strArr[1].Trim().Split(' ')[0].Split(':')[1]), 0);
                                TimeSpan tsstart = new TimeSpan(0, int.Parse(strArr[0].Trim().Split(' ')[0].Split(':')[0]),
                                                                   int.Parse(strArr[0].Trim().Split(' ')[0].Split(':')[1]), 0);
                                if (tsend < tsstart) // зашло за полуночь
                                {
                                    tsend += (new TimeSpan(1, 0, 0, 0));   // - приращение дня
                                }
                                ticks = (tsend.Ticks - tsstart.Ticks);
                                curDateTime = curDateTime.AddTicks(ticks); // - добавить разность тиков
                                string strEndDate = curDateTime.ToString("yyyyMMddHHmmss") + " +0300";
                                xProgramme.Add(new XAttribute("stop", strEndDate));
                            }
                            xProgramme.Add(new XAttribute("channel", id.ToString()));
                            /*Debug.WriteLine(strArr[0].Split(' ')[0] + " - " + strArr[1].Split(' ')[0] + " " +
                                              strArr[0].Substring(6));*/
                            xProgramme.Add(new XElement("title", strArr[0].Substring(6)));
                            //Debug.WriteLine(strArr[0].Substring(6));
                            if (noticeStr.Length > 0)
                            {
                                if (xProgramme != null)
                                {
                                    xProgramme.Add(new XElement("desc", noticeStr.ToString()));
                                    //  Debug.WriteLine(noticeStr.ToString());
                                    noticeStr.Clear();
                                }
                            }
                            preNotice = String.Empty;
                            prNotice = false;

                        }          // Буфферная совпала с входжением времени
                        else if (Regex.IsMatch(strArr[0], @"^\s*\d{2}:\d{2}\s", RegexOptions.Compiled))
                        {
                            if (noticeStr.Length > 0)
                            {
                                if (xProgramme != null)
                                {
                                    xProgramme.Add(new XElement("desc", noticeStr.ToString()));
                                    noticeStr.Clear();
                                }
                            }
                            preNotice = String.Empty;
                            prNotice = false;
                            preNotice = strArr[0];
                            noticeStr.AppendLine(strArr[1]);
                        }
                        else
                        {  // Записываем анонс пока реальная строка дает отличный от пустой строки набор символов и нет вхождения по времени
                            while (!String.IsNullOrEmpty(strArr[1]) && 
                                !Regex.IsMatch(strArr[1], @"^\s*\d{2}:\d{2}\s", RegexOptions.Compiled))
                            {
                                prNotice = true;
                                // strArr[0] = preNotice;
                                noticeStr.AppendLine(strArr[1]);
                                strArr[1] = sr.ReadLine();
                            }
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(strArr[0]) && 
                            Regex.IsMatch(strArr[0], @"^\s*\d{2}:\d{2}\s", RegexOptions.Compiled) && 
                            String.IsNullOrEmpty(strArr[1]))
                        {
                            titleEnd = strArr[0];
                        }
                    }
                }
                // Выгрузить (освободить) файлы:
                fs.Flush();
                fs.Close();
                sr.Close();
                sw.Stop();
                _logger.Trace("Парсинг каналов и тв-программы прошёл успешно. Затрачено '{0}' сек.", sw.ElapsedMilliseconds / 1000.0);
                return xdoc;
            }
            catch (Exception ex)
            {
                _logger.Error("При парсинге каналов и тв-программы произошло исключение: №{0}, ErrMessage='{1}',\nStackTrace='{2}'", j, ex.Message, ex.StackTrace);
            }
            return new XDocument();
        }
    }
}
