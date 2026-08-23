using System;
using System.Linq;
using DynamicExpression.Extensions;
using DynamicExpression.Entities;
using DynamicExpression.Enums;
using TvProgViewer.Data.TvProgMain.ProgObjs;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Границы доступного периода телепрограммы, разбитого на три части
    /// с точкой отсчёта 05:45
    /// </summary>
    /// <param name="TsMin">Начало первой части периода</param>
    /// <param name="Middle1">Конец первой части периода</param>
    /// <param name="Middle2">Конец второй части периода</param>
    /// <param name="TsMax">Конец третьей части периода</param>
    public readonly record struct ProgrammePeriodBounds(DateTime TsMin, DateTime Middle1, DateTime Middle2, DateTime TsMax)
    {
        /// <summary>
        /// Время суток, определяющее границу телепрограммного дня
        /// </summary>
        public const int DayBoundaryHour = 5;
        public const int DayBoundaryMinute = 45;
    }

    /// <summary>
    /// Расширения
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Вычисляет границы трёх частей периода действия телепрограммы:
        /// начало приводится к 05:45, первая часть занимает 7 дней, вторая - ещё 7,
        /// третья простирается до последнего дня программы
        /// </summary>
        /// <param name="period">Период действия программы</param>
        /// <returns>Границы частей периода</returns>
        public static ProgrammePeriodBounds GetPeriodBounds(this ProgPeriod period)
        {
            var fallback = DateTime.UtcNow;

            var tsMin = period.dtStart.HasValue ? period.dtStart.Value.DateTime : fallback;
            tsMin = new DateTime(tsMin.Year, tsMin.Month, tsMin.Day,
                ProgrammePeriodBounds.DayBoundaryHour, ProgrammePeriodBounds.DayBoundaryMinute, 0);

            var middle1 = tsMin.AddDays(6);
            var middle2 = middle1.AddDays(7);

            var tsMax = period.dtEnd.HasValue ? period.dtEnd.Value.DateTime : fallback;
            tsMax = tsMax.AddDays(-1);
            tsMax = new DateTime(tsMax.Year, tsMax.Month, tsMax.Day,
                ProgrammePeriodBounds.DayBoundaryHour, ProgrammePeriodBounds.DayBoundaryMinute, 0);

            return new ProgrammePeriodBounds(tsMin, middle1, middle2, tsMax);
        }

        /// <summary>
        /// Возвращает короткое имя дня недели, используемое в именах файлов пиктограмм
        /// (например, "Mon" для понедельника)
        /// </summary>
        /// <param name="day">День недели</param>
        /// <returns>Короткое имя дня недели</returns>
        public static string GetDayIconName(this DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Mon",
                DayOfWeek.Tuesday => "Tue",
                DayOfWeek.Wednesday => "Wen",
                DayOfWeek.Thursday => "Ths",
                DayOfWeek.Friday => "Fri",
                DayOfWeek.Saturday => "Sat",
                _ => "Sun"
            };
        }

        /// <summary>
        /// Содержит любую строку
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="values">Массив строк</param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (!string.IsNullOrWhiteSpace(str) || values.Length > 0)
            {
                foreach (var _ in from string value in values
                                  where str.Contains(value)
                                  select new { })
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Динамическая сортировка
        /// </summary>
        /// <typeparam name="T">Класс коллекции</typeparam>
        /// <param name="source">Источник-запрос</param>
        /// <param name="property">Поле</param>
        /// <param name="sord">Порядок сортировки</param>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string property, string sord) where T : class
        {
            Ordering ordering = new()
            {
                By = property
            };
            
            if (sord == "asc")
            {
                ordering.Direction = OrderingDirection.Asc;
                return source.Order(ordering);
            }
            else if (sord == "desc")
            {
                ordering.Direction = OrderingDirection.Desc;
                return source.Order(ordering);
            } 

            return null;
        }

        /// <summary>
        /// Динамическое постраничное ограничение и сортировка
        /// </summary>
        /// <typeparam name="T">Класс коллекции</typeparam>
        /// <param name="source">Источник-запрос</param>
        /// <param name="page">Страница</param>
        /// <param name="rows">Количество страниц</param>
        /// <param name="property">Поле</param>
        /// <param name="sord">Порядок сортировки</param>
        public static IQueryable<T> LimitAndOrderBy<T>(this IQueryable<T> source, int page, int rows, string property, string sord) where T : class
        {
            Pagination pagination = new()
            {
                Number = page,
                Count = rows
            };
            return source.OrderBy(property, sord).Limit(pagination);
        }
    }
}