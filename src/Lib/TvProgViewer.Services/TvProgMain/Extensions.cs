using System.Linq;
using DynamicExpression.Extensions;
using DynamicExpression.Entities;
using DynamicExpression.Enums;

namespace TvProgViewer.Services.TvProgMain
{
    /// <summary>
    /// Расширения
    /// </summary>
    public static class Extensions
    {
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