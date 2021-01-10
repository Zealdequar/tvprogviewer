using LinqToDB;
using LinqToDB.Mapping;
using TVProgViewer.Core;

namespace TVProgViewer.Data.Extensions
{
    /// <summary>
    /// PropertyMappingBuilder extensions
    /// </summary>
    public static partial class DataMappingExtensions
    {
        #region Methods

        /// <summary>
        /// Установка LINQ Decimal для типа БД для текущей колонки
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности</typeparam>
        /// <param name="builder">Строитель маппинга свойств</param>
        /// <param name="precision">Размер</param>
        /// <param name="scale">Масштаб</param>
        /// <returns></returns>
        public static PropertyMappingBuilder<TEntity, decimal> HasDecimal<TEntity>(this PropertyMappingBuilder<TEntity, decimal> builder,
            int precision = 18,
            int scale = 4) where TEntity : BaseEntity
        {
            return builder
                .HasDataType(DataType.Decimal)
                .HasPrecision(precision)
                .HasScale(scale);
        }

        /// <summary>
        /// Установка LINQ Decimal Nullable для типа БД для текущей колонки
        /// </summary>
        /// <typeparam name="TEntity">Тип сущности</typeparam>
        /// <param name="builder">Строитель маппинга свойств</param>
        /// <param name="precision">Размер</param>
        /// <param name="scale">Масштаб</param>
        public static PropertyMappingBuilder<TEntity, decimal?> HasDecimalNullable<TEntity>(this PropertyMappingBuilder<TEntity, decimal?> builder,
            int precision = 18,
            int scale = 4) where TEntity : BaseEntity
        {
            return builder
                .HasDataType(DataType.Decimal)
                .HasPrecision(precision)
                .HasScale(scale);
        }

        #endregion
    }
}
