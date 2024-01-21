using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Drawing;
using NLog;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Linq;
using System.Diagnostics;



namespace TvProgViewer.Services.TvProgMain
{
    public class WebPart
    {

        private static Logger _logger = LogManager.GetLogger("webupdate");
        /// <summary>
        /// Загрузка программы телепередач из интернета
        /// </summary>
        /// <param name="url">URL программы телепередач</param>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public static bool GetWebTVProgramm(Uri uri, ref string outputFileName)
        {
            _logger.Info("Начало получения телеканалов с тв-программой\nurid='{0}';\noutputfile='{1}'", 
                uri, outputFileName);
            Stopwatch sw = Stopwatch.StartNew();
            string curDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", DateTime.Now.ToString("yyyy-MM-dd"));
            _logger.Trace("Определен каталог для сохранения: '{0}'", curDir);
            if (!System.IO.Directory.Exists(curDir))
                System.IO.Directory.CreateDirectory(curDir);
            outputFileName = Path.Combine(curDir, outputFileName);
            /*bool isDownload = bool.Parse(ConfigurationSettings.AppSettings["IsDownload"]);
            if (isDownload)*/
                _logger.Trace("Требуется загрузка из интернета");
            /*else 
                _logger.Trace("Загрузка из интернета не требуется");*/
            /*if (isDownload)
            {*/
                byte[] buffer = new byte[4096]; // - буффер чтения/записи
                int read = 0; // - для определения считывания данных
                Stream stream = null; // - поток для распаковки
                HttpWebResponse hwResp; // - для ответа сервера
                try
                {
                    _logger.Info("Начало запроса ресурса.");
                    HttpWebRequest hwReq = (HttpWebRequest)HttpWebRequest.Create(uri);
                    hwResp = (HttpWebResponse)hwReq.GetResponse();
                    stream = hwResp.GetResponseStream();
                    _logger.Trace("Ресурс успешно получен в поток.");
                    if (!string.IsNullOrEmpty(hwResp.ContentEncoding))
                    {
                        _logger.Trace("Content-Encoding = '{0}'", hwResp.ContentEncoding);
                        switch (hwResp.ContentEncoding.ToLower())
                        {
                            case "gzip":
                                stream = new GZipStream(stream, CompressionMode.Decompress);
                                _logger.Trace("Ресурс успешно распакован методом gzip.");
                                break;
                            case "deflate":
                                stream = new DeflateStream(stream, CompressionMode.Decompress);
                                _logger.Trace("Ресурс успешно распакован методом deflate.");
                                break;
                        }
                        using (FileStream fs = new FileStream(outputFileName, FileMode.Create))
                        {
                            while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                fs.Write(buffer, 0, read);
                            }
                            fs.Flush();
                            fs.Close();
                            _logger.Trace("Файл с содержимым '{0}' успешно создан.", outputFileName);
                        }
                    }
                    else
                    {
                        _logger.Info("Content-Encoding неопределен, пробуем распаковать по заголовку ContentType:");
                        _logger.Trace("Content-Type = '{0}'", hwResp.ContentType.ToLower());
                        switch (hwResp.ContentType.ToLower())
                        {
                            case "application/zip":
                                UnZipFile(stream, outputFileName);
                                _logger.Trace("Zip-архив успешно распакован.");
                                break;
                            case "application/x-gzip":
                                stream = new GZipStream(stream, CompressionMode.Decompress);
                                using (var fs = new FileStream(outputFileName, FileMode.Create))
                                {
                                    while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        fs.Write(buffer, 0, read);
                                    }
                                    fs.Flush();
                                    fs.Close();
                                }
                                _logger.Trace("GZip-архив успешно распакован.");
                                break;
                            case "application/octet-stream":
                                stream = new GZipStream(stream, CompressionMode.Decompress);
                                using (var fs = new FileStream(outputFileName, FileMode.Create))
                                {
                                    while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                                    {
                                        fs.Write(buffer, 0, read);
                                    }
                                    fs.Flush();
                                    fs.Close();
                                };
                                _logger.Trace("GZip-архив octet-stream успешно распакован.");
                                break;
                        }
                    }
                    sw.Stop();
                    _logger.Info("Получение каналов и тв-программы успешно завершено. Затрачено времени '{0}' секунд", sw.ElapsedMilliseconds / 1000.0);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error("Получение каналов и тв-программы завершилось с ошибкой: Message = '{0}'\nStackTrace = '{1}'.", ex.Message, ex.StackTrace);
                    return false;
                }
            //}
            return true;
        }

        public static byte[] GetAndSaveIcon(string id, string src)
        {
            if (!(string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(src)))
            {
               // Загрузка и сохранение значка канала:
               byte[] buffer = new byte[4096]; // - буффер чтения/записи
               int read = 0; // - для определения считывания данных
               Stream stream = null; // - поток для распаковки
               HttpWebResponse hwResp; // - для ответа сервера
                
               if (!String.IsNullOrEmpty(src))
               {
                  try
                  {
                     HttpWebRequest hwReq = (HttpWebRequest) HttpWebRequest.Create(src);
                     hwResp = (HttpWebResponse) hwReq.GetResponse();
                     stream = hwResp.GetResponseStream();
                     Image.GetThumbnailImageAbort myCallback =
                     new Image.GetThumbnailImageAbort(ThumbnailCallback);
                     byte[] pictOrigBytes;
                     Image pict = Image.FromStream(stream);
                     using (MemoryStream ms = new MemoryStream())
                     {
                         pict.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                         if (!System.IO.Directory.Exists("imgs"))
                           System.IO.Directory.CreateDirectory("imgs");
                                
                         if (!System.IO.Directory.Exists(@"\imgs\large"))
                            System.IO.Directory.CreateDirectory(@"imgs\large");

                         pict.Save(string.Concat(@"imgs\large\", id + ".gif"));
                         pictOrigBytes = ms.ToArray();
                     }
                     using (MemoryStream ms = new MemoryStream())
                     {
                         if (!System.IO.Directory.Exists(@"\imgs\small"))
                             System.IO.Directory.CreateDirectory(@"imgs\small");

                            pict.GetThumbnailImage(25, 25, myCallback, IntPtr.Zero).Save(string.Concat(@"imgs\small\", id + ".gif"));
                            pict.GetThumbnailImage(25, 25, myCallback, IntPtr.Zero).Save(ms,
                            System.Drawing.Imaging.ImageFormat.Gif);
                     }
                     return pictOrigBytes;
                 }
                 catch (Exception ex)
                 {
                     _logger.Error("Ошибка сохранения файла пиктограммы");
                 }
               }
            }
            return null;
        }
        /// <summary>
        /// Для обратного вызова
        /// </summary>
        /// <returns></returns>
        private static bool ThumbnailCallback()
        {
            return false;
        }
        /// <summary>
        /// Распаковщик zip
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="output_file"></param>
        /// <returns></returns>
        public static bool UnZipFile(Stream stream, string output_file)
        {
            bool ret = true;
            try
            {
                using (ZipInputStream ZipStream = new ZipInputStream(stream))
                {
                    ZipEntry theEntry;
                    while ((theEntry = ZipStream.GetNextEntry()) != null)
                    {
                        if (theEntry.IsFile)
                        {
                            if (theEntry.Name != "")
                            {
                                using (FileStream streamWriter = File.Create(output_file))
                                {
                                    int size = 2048;
                                    byte[] data = new byte[2048];
                                    while (true)
                                    {
                                        size = ZipStream.Read(data, 0, data.Length);
                                        if (size > 0)
                                            streamWriter.Write(data, 0, size);
                                        else
                                            break;
                                    }
                                    streamWriter.Close();
                                }
                            }
                        }
                        else if (theEntry.IsDirectory)
                        {
                        }
                    }
                    ZipStream.Close();
                }
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public static XDocument ReformatXDoc(XDocument xDoc)
        {
             XDocument reformatXDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xDoc.LastNode);
             reformatXDoc.Root.FirstAttribute.Remove();
          //   reformatXDoc.Root.LastAttribute.Remove();
             return reformatXDoc;
        }
    }
}
