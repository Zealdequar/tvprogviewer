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
    /// Represents the tvchannel attribute model factory implementation
    /// </summary>
    public partial class TvChannelAttributeModelFactory : ITvChannelAttributeModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly ITvChannelService _tvchannelService;

        #endregion

        #region Ctor

        public TvChannelAttributeModelFactory(ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ITvChannelAttributeService tvchannelAttributeService,
            ITvChannelService tvchannelService)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _tvchannelAttributeService = tvchannelAttributeService;
            _tvchannelService = tvchannelService;
        }

        #endregion

        #region Utilities
        
        /// <summary>
        /// Prepare predefined tvchannel attribute value search model
        /// </summary>
        /// <param name="searchModel">Predefined tvchannel attribute value search model</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>Predefined tvchannel attribute value search model</returns>
        protected virtual PredefinedTvChannelAttributeValueSearchModel PreparePredefinedTvChannelAttributeValueSearchModel(
            PredefinedTvChannelAttributeValueSearchModel searchModel, TvChannelAttribute tvchannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelAttribute == null)
                throw new ArgumentNullException(nameof(tvchannelAttribute));

            searchModel.TvChannelAttributeId = tvchannelAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare search model of tvchannels that use the tvchannel attribute
        /// </summary>
        /// <param name="searchModel">Search model of tvchannels that use the tvchannel attribute</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>Search model of tvchannels that use the tvchannel attribute</returns>
        protected virtual TvChannelAttributeTvChannelSearchModel PrepareTvChannelAttributeTvChannelSearchModel(TvChannelAttributeTvChannelSearchModel searchModel,
            TvChannelAttribute tvchannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelAttribute == null)
                throw new ArgumentNullException(nameof(tvchannelAttribute));

            searchModel.TvChannelAttributeId = tvchannelAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare tvchannel attribute search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute search model
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
        /// Prepare paged tvchannel attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute list model
        /// </returns>
        public virtual async Task<TvChannelAttributeListModel> PrepareTvChannelAttributeListModelAsync(TvChannelAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvchannel attributes
            var tvchannelAttributes = await _tvchannelAttributeService
                .GetAllTvChannelAttributesAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new TvChannelAttributeListModel().PrepareToGrid(searchModel, tvchannelAttributes, () =>
            {
                //fill in model values from the entity
                return tvchannelAttributes.Select(attribute => attribute.ToModel<TvChannelAttributeModel>());
                
            });

            return model;
        }

        /// <summary>
        /// Prepare tvchannel attribute model
        /// </summary>
        /// <param name="model">TvChannel attribute model</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute model
        /// </returns>
        public virtual async Task<TvChannelAttributeModel> PrepareTvChannelAttributeModelAsync(TvChannelAttributeModel model,
            TvChannelAttribute tvchannelAttribute, bool excludeProperties = false)
        {
            Func<TvChannelAttributeLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvchannelAttribute != null)
            {
                //fill in model values from the entity
                model ??= tvchannelAttribute.ToModel<TvChannelAttributeModel>();

                //prepare nested search models
                PreparePredefinedTvChannelAttributeValueSearchModel(model.PredefinedTvChannelAttributeValueSearchModel, tvchannelAttribute);
                PrepareTvChannelAttributeTvChannelSearchModel(model.TvChannelAttributeTvChannelSearchModel, tvchannelAttribute);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvchannelAttribute, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(tvchannelAttribute, entity => entity.Description, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged predefined tvchannel attribute value list model
        /// </summary>
        /// <param name="searchModel">Predefined tvchannel attribute value search model</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined tvchannel attribute value list model
        /// </returns>
        public virtual async Task<PredefinedTvChannelAttributeValueListModel> PreparePredefinedTvChannelAttributeValueListModelAsync(
            PredefinedTvChannelAttributeValueSearchModel searchModel, TvChannelAttribute tvchannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelAttribute == null)
                throw new ArgumentNullException(nameof(tvchannelAttribute));

            //get predefined tvchannel attribute values
            var values = (await _tvchannelAttributeService.GetPredefinedTvChannelAttributeValuesAsync(tvchannelAttribute.Id)).ToPagedList(searchModel);

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
        /// Prepare predefined tvchannel attribute value model
        /// </summary>
        /// <param name="model">Predefined tvchannel attribute value model</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <param name="tvchannelAttributeValue">Predefined tvchannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined tvchannel attribute value model
        /// </returns>
        public virtual async Task<PredefinedTvChannelAttributeValueModel> PreparePredefinedTvChannelAttributeValueModelAsync(PredefinedTvChannelAttributeValueModel model,
            TvChannelAttribute tvchannelAttribute, PredefinedTvChannelAttributeValue tvchannelAttributeValue, bool excludeProperties = false)
        {
            if (tvchannelAttribute == null)
                throw new ArgumentNullException(nameof(tvchannelAttribute));

            Func<PredefinedTvChannelAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (tvchannelAttributeValue != null)
            {
                //fill in model values from the entity
                if (model == null) 
                {
                    model = tvchannelAttributeValue.ToModel<PredefinedTvChannelAttributeValueModel>();
                }

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(tvchannelAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.TvChannelAttributeId = tvchannelAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged list model of tvchannels that use the tvchannel attribute
        /// </summary>
        /// <param name="searchModel">Search model of tvchannels that use the tvchannel attribute</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list model of tvchannels that use the tvchannel attribute
        /// </returns>
        public virtual async Task<TvChannelAttributeTvChannelListModel> PrepareTvChannelAttributeTvChannelListModelAsync(TvChannelAttributeTvChannelSearchModel searchModel,
            TvChannelAttribute tvchannelAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (tvchannelAttribute == null)
                throw new ArgumentNullException(nameof(tvchannelAttribute));

            //get tvchannels
            var tvchannels = await _tvchannelService.GetTvChannelsByTvChannelAttributeIdAsync(tvchannelAttributeId: tvchannelAttribute.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new TvChannelAttributeTvChannelListModel().PrepareToGrid(searchModel, tvchannels, () =>
            {
                //fill in model values from the entity
                return tvchannels.Select(tvchannel =>
                {
                    var tvchannelAttributeTvChannelModel = tvchannel.ToModel<TvChannelAttributeTvChannelModel>();
                    tvchannelAttributeTvChannelModel.TvChannelName = tvchannel.Name;
                    return tvchannelAttributeTvChannelModel;
                });
            });

            return model;
        }

        #endregion
    }
}