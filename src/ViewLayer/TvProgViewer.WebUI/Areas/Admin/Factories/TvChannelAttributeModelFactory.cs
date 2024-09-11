using System;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Localization;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvChannel attribute model factory implementation
    /// </summary>
    public partial class TvChannelAttributeModelFactory : ITvChannelAttributeModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;

        #endregion

        #region Ctor

        public TvChannelAttributeModelFactory(ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
        }

        #endregion

        #region Utilities
        
        /// <summary>
        /// Prepare predefined tvChannel attribute value search model
        /// </summary>
        /// <param name="searchModel">Predefined tvChannel attribute value search model</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Predefined tvChannel attribute value search model</returns>
        protected virtual PredefinedTvChannelAttributeValueSearchModel PreparePredefinedTvChannelAttributeValueSearchModel(
            PredefinedTvChannelAttributeValueSearchModel searchModel, TvChannelAttribute tvChannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelAttribute == null)
                throw new ArgumentNullException(nameof(tvChannelAttribute));

            searchModel.TvChannelAttributeId = tvChannelAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare search model of tvChannels that use the tvChannel attribute
        /// </summary>
        /// <param name="searchModel">Search model of tvChannels that use the tvChannel attribute</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>Search model of tvChannels that use the tvChannel attribute</returns>
        protected virtual TvChannelAttributeTvChannelSearchModel PrepareTvChannelAttributeTvChannelSearchModel(TvChannelAttributeTvChannelSearchModel searchModel,
            TvChannelAttribute tvChannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelAttribute == null)
                throw new ArgumentNullException(nameof(tvChannelAttribute));

            searchModel.TvChannelAttributeId = tvChannelAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tvChannel attribute search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute search model
        /// </returns>
        public virtual Task<TvChannelAttributeSearchModel> PrepareTvChannelAttributeSearchModelAsync(TvChannelAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged tvChannel attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute list model
        /// </returns>
        public virtual async Task<TvChannelAttributeListModel> PrepareTvChannelAttributeListModelAsync(TvChannelAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvChannel attributes
            var tvChannelAttributes = await _tvChannelAttributeService
                .GetAllTvChannelAttributesAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new TvChannelAttributeListModel().PrepareToGrid(searchModel, tvChannelAttributes, () =>
            {
                //fill in model values from the entity
                return tvChannelAttributes.Select(attribute => attribute.ToModel<TvChannelAttributeModel>());
                
            });

            return model;
        }

        /// <summary>
        /// Prepare tvChannel attribute model
        /// </summary>
        /// <param name="model">TvChannel attribute model</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute model
        /// </returns>
        public virtual async Task<TvChannelAttributeModel> PrepareTvChannelAttributeModelAsync(TvChannelAttributeModel model,
            TvChannelAttribute tvChannelAttribute, bool excludeProperties = false)
        {
            Func<TvChannelAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvChannelAttribute != null)
            {
                //fill in model values from the entity
                model ??= tvChannelAttribute.ToModel<TvChannelAttributeModel>();

                //prepare nested search models
                PreparePredefinedTvChannelAttributeValueSearchModel(model.PredefinedTvChannelAttributeValueSearchModel, tvChannelAttribute);
                PrepareTvChannelAttributeTvChannelSearchModel(model.TvChannelAttributeTvChannelSearchModel, tvChannelAttribute);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvChannelAttribute, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(tvChannelAttribute, entity => entity.Description, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged predefined tvChannel attribute value list model
        /// </summary>
        /// <param name="searchModel">Predefined tvChannel attribute value search model</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the predefined tvChannel attribute value list model
        /// </returns>
        public virtual async Task<PredefinedTvChannelAttributeValueListModel> PreparePredefinedTvChannelAttributeValueListModelAsync(
            PredefinedTvChannelAttributeValueSearchModel searchModel, TvChannelAttribute tvChannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelAttribute == null)
                throw new ArgumentNullException(nameof(tvChannelAttribute));

            //get predefined tvChannel attribute values
            var values = (await _tvChannelAttributeService.GetPredefinedTvChannelAttributeValuesAsync(tvChannelAttribute.Id)).ToPagedList(searchModel);

            //prepare list model
            var model = new PredefinedTvChannelAttributeValueListModel().PrepareToGrid(searchModel, values, () =>
            {
                return values.Select(value =>
                {
                    //fill in model values from the entity
                    var predefinedTvChannelAttributeValueModel = value.ToModel<PredefinedTvChannelAttributeValueModel>();

                    //fill in additional values (not existing in the entity)
                    predefinedTvChannelAttributeValueModel.WeightAdjustmentStr = value.WeightAdjustment.ToString("G29");
                    predefinedTvChannelAttributeValueModel.PriceAdjustmentStr = value.PriceAdjustment
                        .ToString("G29") + (value.PriceAdjustmentUsePercentage ? " %" : string.Empty);

                    return predefinedTvChannelAttributeValueModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare predefined tvChannel attribute value model
        /// </summary>
        /// <param name="model">Predefined tvChannel attribute value model</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <param name="tvChannelAttributeValue">Predefined tvChannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the predefined tvChannel attribute value model
        /// </returns>
        public virtual async Task<PredefinedTvChannelAttributeValueModel> PreparePredefinedTvChannelAttributeValueModelAsync(PredefinedTvChannelAttributeValueModel model,
            TvChannelAttribute tvChannelAttribute, PredefinedTvChannelAttributeValue tvChannelAttributeValue, bool excludeProperties = false)
        {
            if (tvChannelAttribute == null)
                throw new ArgumentNullException(nameof(tvChannelAttribute));

            Func<PredefinedTvChannelAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvChannelAttributeValue != null)
            {
                //fill in model values from the entity
                if (model == null) 
                {
                    model = tvChannelAttributeValue.ToModel<PredefinedTvChannelAttributeValueModel>();
                }

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvChannelAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.TvChannelAttributeId = tvChannelAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged list model of tvChannels that use the tvChannel attribute
        /// </summary>
        /// <param name="searchModel">Search model of tvChannels that use the tvChannel attribute</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list model of tvChannels that use the tvChannel attribute
        /// </returns>
        public virtual async Task<TvChannelAttributeTvChannelListModel> PrepareTvChannelAttributeTvChannelListModelAsync(TvChannelAttributeTvChannelSearchModel searchModel,
            TvChannelAttribute tvChannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvChannelAttribute == null)
                throw new ArgumentNullException(nameof(tvChannelAttribute));

            //get tvChannels
            var tvChannels = await _tvChannelService.GetTvChannelsByTvChannelAttributeIdAsync(tvChannelAttributeId: tvChannelAttribute.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new TvChannelAttributeTvChannelListModel().PrepareToGrid(searchModel, tvChannels, () =>
            {
                //fill in model values from the entity
                return tvChannels.Select(tvChannel =>
                {
                    var tvChannelAttributeTvChannelModel = tvChannel.ToModel<TvChannelAttributeTvChannelModel>();
                    tvChannelAttributeTvChannelModel.TvChannelName = tvChannel.Name;
                    return tvChannelAttributeTvChannelModel;
                });
            });

            return model;
        }

        #endregion
    }
}