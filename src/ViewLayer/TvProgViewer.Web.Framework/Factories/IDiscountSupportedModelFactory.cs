﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Web.Framework.Factories
{
    /// <summary>
    /// Represents the discount supported model factory
    /// </summary>
    public partial interface IDiscountSupportedModelFactory
    {
        /// <summary>
        /// Prepare selected and all available discounts for the passed model
        /// </summary>
        /// <typeparam name="TModel">Discount supported model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="availableDiscounts">List of all available discounts</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<TModel> PrepareModelDiscountsAsync<TModel>(TModel model, IList<Discount> availableDiscounts) where TModel : IDiscountSupportedModel;

        /// <summary>
        /// Prepare selected and all available discounts for the passed model by entity applied discounts
        /// </summary>
        /// <typeparam name="TModel">Discount supported model type</typeparam>
        /// <typeparam name="TMapping">Discount supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="availableDiscounts">List of all available discounts</param>
        /// <param name="ignoreAppliedDiscounts">Whether to ignore existing applied discounts</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<TModel> PrepareModelDiscountsAsync<TModel, TMapping>(TModel model, IDiscountSupported<TMapping> entity,
            IList<Discount> availableDiscounts, bool ignoreAppliedDiscounts)
            where TModel : IDiscountSupportedModel where TMapping : DiscountMapping; 
    }
}