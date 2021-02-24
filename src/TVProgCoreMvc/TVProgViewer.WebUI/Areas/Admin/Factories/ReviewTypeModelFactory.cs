using System;
using System.Linq;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Factories;
using TVProgViewer.Web.Framework.Models.Extensions;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents a review type model factory implementation
    /// </summary>
    public partial class ReviewTypeModelFactory : IReviewTypeModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IReviewTypeService _reviewTypeService;

        #endregion

        #region Ctor

        public ReviewTypeModelFactory(ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IReviewTypeService reviewTypeService)
        {
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _reviewTypeService = reviewTypeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare review type search model
        /// </summary>
        /// <param name="searchModel">Review type search model</param>
        /// <returns>Review type search model</returns>
        public virtual Task<ReviewTypeSearchModel> PrepareReviewTypeSearchModelAsync(ReviewTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged review type list model
        /// </summary>
        /// <param name="searchModel">Review type search model</param>
        /// <returns>Review type list model</returns>
        public virtual async Task<ReviewTypeListModel> PrepareReviewTypeListModelAsync(ReviewTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get review types
            var reviewTypes = (await _reviewTypeService.GetAllReviewTypesAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = new ReviewTypeListModel().PrepareToGrid(searchModel, reviewTypes, () =>
            {
                //fill in model values from the entity
                return reviewTypes.Select(reviewType => reviewType.ToModel<ReviewTypeModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare review type model
        /// </summary>
        /// <param name="model">Review type model</param>
        /// <param name="reviewType">Review type</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Review type model</returns>
        public virtual async Task<ReviewTypeModel> PrepareReviewTypeModelAsync(ReviewTypeModel model,
            ReviewType reviewType, bool excludeProperties = false)
        {
            Action<ReviewTypeLocalizedModel, int> localizedModelConfiguration = null;

            if (reviewType != null)
            {
                //fill in model values from the entity
                model ??= reviewType.ToModel<ReviewTypeModel>();

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name, languageId, false, false);
                    locale.Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}
