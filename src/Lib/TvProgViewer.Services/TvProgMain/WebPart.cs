using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using NLog;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Класс для работы с веб-ресурсами: загрузка и обработка телепрограмм
    /// </summary>
    public class WebPart
    {
        private static readonly Logger _logger = LogManager.GetLogger("webupdate");
        private static readonly HttpClient _httpClient;
        
        // Константы для размеров буферов
        private const int DefaultBufferSize = 4096;
        private const int ZipBufferSize = 2048;
        private const int RequestTimeoutSeconds = 300; // 5 минут

        /// <summary>
        /// Статический конструктор для инициализации HttpClient
        /// </summary>
        static WebPart()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds)
            };
        }
        /// <summary>
        /// Загрузка программы телепередач из интернета
        /// </summary>
        /// <param name="uri">URI программы телепередач</param>
        /// <param name="outputFileName">Имя выходного файла (будет изменено на полный путь)</param>
        /// <returns>Кортеж: (успех операции, полный путь к выходному файлу)</returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public static async Task<(bool Success, string FullFileName)> GetWebTVProgramm(Uri uri, string outputFileName)
        {
            string fullOutputFileName = outputFileName;

            if (uri == null)
            {
                _logger.Error("URI не может быть null");
                return (false, outputFileName);
            }

            if (string.IsNullOrWhiteSpace(outputFileName))
            {
                _logger.Error("Имя выходного файла не может быть пустым");
                return (false, outputFileName);
            }

            _logger.Info("Начало получения телеканалов с тв-программой\nurid='{0}';\noutputfile='{1}'", 
                uri, outputFileName);
            
            var sw = Stopwatch.StartNew();
            
            try
            {
                // Определение каталога для сохранения
                var curDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", DateTime.Now.ToString("yyyy-MM-dd"));
                _logger.Trace("Определен каталог для сохранения: '{0}'", curDir);
                
                if (!System.IO.Directory.Exists(curDir))
                {
                    System.IO.Directory.CreateDirectory(curDir);
                }
                
                fullOutputFileName = Path.Combine(curDir, outputFileName);
                _logger.Trace("Требуется загрузка из интернета");

                // Создание HTTP запроса
                _logger.Info("Начало запроса ресурса.");
                
                using (var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Error("HTTP запрос завершился с ошибкой. Статус код: {0}, Причина: {1}", 
                            response.StatusCode, response.ReasonPhrase);
                        return (false, fullOutputFileName);
                    }

                    // Получение заголовков
                    var contentType = response.Content.Headers.ContentType?.MediaType?.ToLowerInvariant() ?? string.Empty;
                    var contentEncodingValue = response.Content.Headers.ContentEncoding?.FirstOrDefault() ?? string.Empty;
                    
                    _logger.Trace("Ресурс успешно получен. StatusCode: {0}, Content-Encoding: '{1}', Content-Type: '{2}'", 
                        response.StatusCode,
                        contentEncodingValue ?? "не указан",
                        contentType ?? "не указан");

                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        // Специальная обработка для ZIP архивов
                        if (contentType == "application/zip")
                        {
                            _logger.Trace("Обнаружен ZIP архив, используем специальный метод распаковки");
                            var zipSuccess = await UnZipFileAsync(responseStream, fullOutputFileName);
                            sw.Stop();
                            if (zipSuccess)
                            {
                                _logger.Info("Получение каналов и тв-программы успешно завершено. Затрачено времени '{0}' секунд", 
                                    sw.ElapsedMilliseconds / 1000.0);
                                return (true, fullOutputFileName);
                            }
                            return (false, fullOutputFileName);
                        }

                        // Обработка других типов сжатия
                        using (var decompressionStream = GetDecompressionStream(responseStream, contentEncodingValue, contentType))
                        {
                            // Сохранение в файл
                            await SaveStreamToFileAsync(decompressionStream, fullOutputFileName);
                        }
                    }
                }
                
                sw.Stop();
                _logger.Info("Получение каналов и тв-программы успешно завершено. Затрачено времени '{0}' секунд", 
                    sw.ElapsedMilliseconds / 1000.0);
                return (true, fullOutputFileName);
            }
            catch (HttpRequestException ex)
            {
                _logger.Error(ex, "Ошибка при выполнении HTTP запроса: Message='{0}'", ex.Message);
                return (false, fullOutputFileName);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.InnerException is TimeoutException)
                {
                    _logger.Error(ex, "Превышено время ожидания ответа от сервера");
                }
                else
                {
                    _logger.Error(ex, "Запрос был отменен");
                }
                return (false, fullOutputFileName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Получение каналов и тв-программы завершилось с ошибкой");
                return (false, fullOutputFileName);
            }
        }

        /// <summary>
        /// Загрузка программы телепередач из интернета (синхронная обёртка для обратной совместимости)
        /// </summary>
        /// <param name="uri">URI программы телепередач</param>
        /// <param name="outputFileName">Имя выходного файла (будет изменено на полный путь)</param>
        /// <returns>True если загрузка успешна, иначе False</returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public static bool GetWebTVProgrammSync(Uri uri, ref string outputFileName)
        {
            var (success, fullFileName) = GetWebTVProgramm(uri, outputFileName).GetAwaiter().GetResult();
            outputFileName = fullFileName;
            return success;
        }

        /// <summary>
        /// Загрузка программы телепередач из интернета (синхронная версия с ref параметром для обратной совместимости)
        /// </summary>
        /// <param name="uri">URI программы телепередач</param>
        /// <param name="outputFileName">Имя выходного файла (будет изменено на полный путь через ref)</param>
        /// <returns>True если загрузка успешна, иначе False</returns>
        [Obfuscation(Feature = "all", ApplyToMembers = true)]
        public static bool GetWebTVProgramm(Uri uri, ref string outputFileName)
        {
            return GetWebTVProgrammSync(uri, ref outputFileName);
        }

        /// <summary>
        /// Получение потока декомпрессии на основе заголовков ответа
        /// </summary>
        private static Stream GetDecompressionStream(Stream responseStream, string contentEncoding, string contentType)
        {
            // Приоритет: Content-Encoding
            if (!string.IsNullOrEmpty(contentEncoding))
            {
                var encoding = contentEncoding.ToLowerInvariant();
                _logger.Trace("Content-Encoding = '{0}'", encoding);
                
                switch (encoding)
                {
                    case "gzip":
                    case "application/gzip":
                        _logger.Trace("Распаковка методом gzip по Content-Encoding");
                        return new GZipStream(responseStream, CompressionMode.Decompress, leaveOpen: false);
                    
                    case "deflate":
                        _logger.Trace("Распаковка методом deflate по Content-Encoding");
                        return new DeflateStream(responseStream, CompressionMode.Decompress, leaveOpen: false);
                }
            }

            // Если Content-Encoding не указан, пробуем по Content-Type
            if (!string.IsNullOrEmpty(contentType))
            {
                var type = contentType.ToLowerInvariant();
                _logger.Trace("Content-Encoding неопределен, пробуем по Content-Type: '{0}'", type);
                
                switch (type)
                {
                    case "application/x-gzip":
                    case "application/gzip":
                    case "application/octet-stream":
                        _logger.Trace("Распаковка GZip архива по Content-Type: '{0}'", type);
                        return new GZipStream(responseStream, CompressionMode.Decompress, leaveOpen: false);
                }
            }

            // Если ничего не подошло, возвращаем исходный поток
            _logger.Trace("Сжатие не обнаружено, используется исходный поток");
            return responseStream;
        }

        /// <summary>
        /// Сохранение потока в файл (асинхронная версия)
        /// </summary>
        private static async Task SaveStreamToFileAsync(Stream sourceStream, string outputFileName)
        {
            var buffer = new byte[DefaultBufferSize];
            
            using (var fileStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.None, 
                bufferSize: DefaultBufferSize, useAsync: true))
            {
                int bytesRead;
                while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                }
                await fileStream.FlushAsync();
            }
            
            var fileInfo = new FileInfo(outputFileName);
            _logger.Trace("Файл с содержимым '{0}' успешно создан. Размер: {1} байт", 
                outputFileName, fileInfo.Length);
        }

        /// <summary>
        /// Распаковка ZIP архива из потока (асинхронная версия)
        /// </summary>
        /// <param name="stream">Поток с ZIP архивом</param>
        /// <param name="outputFile">Путь к выходному файлу</param>
        /// <returns>True если распаковка успешна, иначе False</returns>
        public static async Task<bool> UnZipFileAsync(Stream stream, string outputFile)
        {
            if (stream == null)
            {
                _logger.Error("Поток для распаковки ZIP не может быть null");
                return false;
            }

            if (string.IsNullOrWhiteSpace(outputFile))
            {
                _logger.Error("Имя выходного файла для ZIP не может быть пустым");
                return false;
            }

            try
            {
                using (var zipStream = new ZipInputStream(stream))
                {
                    ZipEntry entry;
                    bool fileExtracted = false;

                    while ((entry = zipStream.GetNextEntry()) != null)
                    {
                        if (entry.IsFile && !string.IsNullOrWhiteSpace(entry.Name))
                        {
                            // Извлекаем только первый файл из архива
                            if (!fileExtracted)
                            {
                                await ExtractZipEntryAsync(zipStream, outputFile);
                                fileExtracted = true;
                                _logger.Trace("Извлечен файл '{0}' из ZIP архива в '{1}'", entry.Name, outputFile);
                            }
                            else
                            {
                                _logger.Warn("В архиве обнаружено несколько файлов, извлечен только первый: '{0}'", entry.Name);
                            }
                        }
                        // Директории пропускаются
                    }

                    if (!fileExtracted)
                    {
                        _logger.Warn("В ZIP архиве не найдено файлов для извлечения");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при распаковке ZIP файла в '{0}'", outputFile);
                return false;
            }
        }

        /// <summary>
        /// Извлечение записи из ZIP архива (асинхронная версия)
        /// </summary>
        /// <remarks>
        /// ZipInputStream.Read синхронный, поэтому чтение выполняется синхронно,
        /// но запись в файл асинхронная для лучшей производительности
        /// </remarks>
        private static async Task ExtractZipEntryAsync(ZipInputStream zipStream, string outputFile)
        {
            var buffer = new byte[ZipBufferSize];
            
            using (var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None, 
                bufferSize: ZipBufferSize, useAsync: true))
            {
                int bytesRead;
                // ZipInputStream.Read синхронный, но мы используем асинхронную запись
                while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                }
                await fileStream.FlushAsync();
            }
        }

        /// <summary>
        /// Реформатирование XML документа (удаление первого атрибута корневого элемента)
        /// </summary>
        /// <param name="xDoc">Исходный XML документ</param>
        /// <returns>Реформированный XML документ</returns>
        public static XDocument ReformatXDoc(XDocument xDoc)
        {
            if (xDoc == null)
            {
                _logger.Error("XML документ не может быть null");
                throw new ArgumentNullException(nameof(xDoc));
            }

            if (xDoc.Root == null)
            {
                _logger.Error("XML документ не содержит корневого элемента");
                throw new ArgumentException("XML документ должен содержать корневой элемент", nameof(xDoc));
            }

            try
            {
                var reformatXDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xDoc.LastNode);
                
                // Удаляем первый атрибут корневого элемента, если он существует
                if (reformatXDoc.Root?.FirstAttribute != null)
                {
                    reformatXDoc.Root.FirstAttribute.Remove();
                    _logger.Trace("Первый атрибут корневого элемента удален из XML документа");
                }
                
                return reformatXDoc;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка при реформатировании XML документа");
                throw;
            }
        }
    }
}
