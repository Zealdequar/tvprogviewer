﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Services.Tax;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Tax;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tax model factory implementation
    /// </summary>
    public partial class TaxModelFactory : ITaxModelFactory
    {
        #region Fields

        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public TaxModelFactory(
            ITaxCategoryService taxCategoryService,
            ITaxPluginManager taxPluginManager)
        {
            _taxCategoryService = taxCategoryService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Prepare tax provider search model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax provider search model
        /// </returns>
        public virtual Task<TaxProviderSearchModel> PrepareTaxProviderSearchModelAsync(TaxProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged tax provider list model
        /// </summary>
        /// <param name="searchModel">Tax provider search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax provider list model
        /// </returns>
        public virtual async Task<TaxProviderListModel> PrepareTaxProviderListModelAsync(TaxProviderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tax providers
            var taxProviders = (await _taxPluginManager.LoadAllPluginsAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new TaxProviderListModel().PrepareToGrid(searchModel, taxProviders, () =>
            {
                return taxProviders.Select(provider =>
                {
                    //fill in model values from the entity
                    var taxProviderModel = provider.ToPluginModel<TaxProviderModel>();

                    //fill in additional values (not existing in the entity)
                    taxProviderModel.ConfigurationUrl = provider.GetConfigurationPageUrl();
                    taxProviderModel.IsPrimaryTaxProvider = _taxPluginManager.IsPluginActive(provider);

                    return taxProviderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare tax category search model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax category search model
        /// </returns>
        public virtual Task<TaxCategorySearchModel> PrepareTaxCategorySearchModelAsync(TaxCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged tax category list model
        /// </summary>
        /// <param name="searchModel">Tax category search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax category list model
        /// </returns>
        public virtual async Task<TaxCategoryListModel> PrepareTaxCategoryListModelAsync(TaxCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tax categories
            var taxCategories = (await _taxCategoryService.GetAllTaxCategoriesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new TaxCategoryListModel().PrepareToGrid(searchModel, taxCategories, () =>
            {
                //fill in model values from the entity
                return taxCategories.Select(taxCategory => taxCategory.ToModel<TaxCategoryModel>());
            });

            return model;
        }

        #endregion
    }
}