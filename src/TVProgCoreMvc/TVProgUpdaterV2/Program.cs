using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using TVProgViewer.Services.Configuration;

namespace TVProgViewer.TVProgUpdaterV2
{
    /// <summary>
    /// Консолька для обновления ТВ-программы
    /// </summary>
    class Program
    {
        private static Logger _logger = LogManager.GetLogger("dbupdate");
        private static Dictionary<string, string> _dictChanCodeOld = new Dictionary<string, string>();
        private static Dictionary<string, string> _dictChanCodeNew = new Dictionary<string, string>();
        
        /// <summary>
        /// Дата и время правильные
        /// </summary>
        /// <param name="s">Строка с датой</param>
        private static bool IsDateTimeValid(string str)
        {
            CultureInfo provider = new CultureInfo("ru-Ru");
            string dateTimeString = string.Concat(str.Replace('.', ' '), DateTime.Now.Year);
            if (DateTime.Now <= DateTime.ParseExact(dateTimeString, "dd MMMM yyyy", provider))
            {
                return true;
            }
            else
            {
                _logger.Info(string.Concat("Актуальная программа есть только за ", dateTimeString));
                return false;
            }
        }


        /*static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddTransient<IProgrammeService, ProgrammeService>()
                            .AddTransient<IUpdaterService, UpdaterService>());*/

        

        static async Task Main(string[] args)
        {
           await Host.CreateDefaultBuilder(args)
                 .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                 .ConfigureWebHostDefaults(webBuilder => webBuilder
                     .ConfigureAppConfiguration(configuration => configuration.AddJsonFile(TvProgConfigurationDefaults.AppSettingsFilePath, true, true))
                     .UseStartup<Startup>())
                 .Build()
                 .RunAsync();
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
