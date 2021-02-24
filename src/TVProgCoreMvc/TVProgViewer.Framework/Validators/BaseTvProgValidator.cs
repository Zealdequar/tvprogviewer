﻿using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using FluentValidation;
using LinqToDB;
using LinqToDB.Mapping;
using TVProgViewer.Core;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Data;
using TVProgViewer.Services.Localization;

namespace TVProgViewer.Web.Framework.Validators
{
    /// <summary>
    /// Base class for validators
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    public abstract class BaseTvProgValidator<TModel> : AbstractValidator<TModel> where TModel : class
    {
        #region Ctor

        protected BaseTvProgValidator()
        {
            PostInitialize();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
        }

        /// <summary>
        /// Sets validation rule(s) to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="filterStringPropertyNames">Properties to skip</param>
        protected virtual void SetDatabaseValidationRules<TEntity>(ITvProgDataProvider dataProvider, params string[] filterStringPropertyNames)
            where TEntity : BaseEntity
        {
            if (dataProvider is null)
                throw new ArgumentNullException(nameof(dataProvider));

            var entityDescriptor = dataProvider.GetEntityDescriptor<TEntity>();

            SetStringPropertiesMaxLength<TEntity>(entityDescriptor, filterStringPropertyNames);
            SetDecimalMaxValue<TEntity>(entityDescriptor);
        }

        /// <summary>
        /// Sets length validation rule(s) to string properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entityDescriptor">Entity descriptor</param>
        /// <param name="filterPropertyNames">Properties to skip</param>
        protected virtual void SetStringPropertiesMaxLength<TEntity>(EntityDescriptor entityDescriptor, params string[] filterPropertyNames)
            where TEntity : BaseEntity
        {
            if (entityDescriptor is null)
                return;

            //filter model properties for which need to get max lengths
            var modelPropertyNames = typeof(TModel).GetProperties()
                .Where(property => property.PropertyType == typeof(string) && !filterPropertyNames.Contains(property.Name))
                .Select(property => property.Name).ToList();

            //get max length of these properties
            var columnsMaxLengths = entityDescriptor.Columns.Where(column => 
                modelPropertyNames.Contains(column.ColumnName) && column.MemberType == typeof(string) && column.Length.HasValue);

            //create expressions for the validation rules
            var maxLengthExpressions = columnsMaxLengths.Select(property => new
            {
                MaxLength = property.Length.Value,
                Expression = DynamicExpressionParser.ParseLambda<TModel, string>(null, false, property.ColumnName)
            }).ToList();

            //define string length validation rules
            foreach (var expression in maxLengthExpressions)
            {
                RuleFor(expression.Expression).Length(0, expression.MaxLength);
            }
        }

        /// <summary>
        /// Sets max value validation rule(s) to decimal properties according to appropriate database model
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        protected virtual void SetDecimalMaxValue<TEntity>(EntityDescriptor entityDescriptor) where TEntity : BaseEntity
        {
            if (entityDescriptor is null)
                return;

            //filter model properties for which need to get max values
            var modelPropertyNames = typeof(TModel).GetProperties()
                .Where(property => property.PropertyType == typeof(decimal))
                .Select(property => property.Name).ToList();

            //get max values of these properties
            var decimalColumnsMaxValues = entityDescriptor.Columns.Where(column => 
                modelPropertyNames.Contains(column.ColumnName) && 
                column.DataType == DataType.Decimal && column.Precision.HasValue && column.Scale.HasValue);

            //create expressions for the validation rules
            var maxValueExpressions = decimalColumnsMaxValues.Select(column => new
            {
                MaxValue = (decimal) Math.Pow(10, column.Precision.Value - column.Scale.Value),
                Expression = DynamicExpressionParser.ParseLambda<TModel, decimal>(null, false, column.ColumnName)
            }).ToList();

            //define decimal validation rules
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            foreach (var expression in maxValueExpressions)
            {
                RuleFor(expression.Expression).IsDecimal(expression.MaxValue)
                    .WithMessageAwait(localizationService.GetResourceAsync("TVProgViewer.Web.Framework.Validators.MaxDecimal"), expression.MaxValue - 1);
            }
        }

        #endregion
    }
}