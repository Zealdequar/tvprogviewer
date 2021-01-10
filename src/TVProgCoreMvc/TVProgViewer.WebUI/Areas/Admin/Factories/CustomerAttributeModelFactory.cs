using System;
using System.Linq;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Localization;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Users;
using TVProgViewer.Web.Framework.Factories;
using TVProgViewer.Web.Framework.Models.Extensions;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the user attribute model factory implementation
    /// </summary>
    public partial class UserAttributeModelFactory : IUserAttributeModelFactory
    {
        #region Fields

        private readonly IUserAttributeService _userAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;

        #endregion

        #region Ctor

        public UserAttributeModelFactory(IUserAttributeService userAttributeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory)
        {
            _userAttributeService = userAttributeService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare user attribute value search model
        /// </summary>
        /// <param name="searchModel">User attribute value search model</param>
        /// <param name="userAttribute">User attribute</param>
        /// <returns>User attribute value search model</returns>
        protected virtual UserAttributeValueSearchModel PrepareUserAttributeValueSearchModel(UserAttributeValueSearchModel searchModel,
            UserAttribute userAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            searchModel.UserAttributeId = userAttribute.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare user attribute search model
        /// </summary>
        /// <param name="searchModel">User attribute search model</param>
        /// <returns>User attribute search model</returns>
        public virtual UserAttributeSearchModel PrepareUserAttributeSearchModel(UserAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged user attribute list model
        /// </summary>
        /// <param name="searchModel">User attribute search model</param>
        /// <returns>User attribute list model</returns>
        public virtual UserAttributeListModel PrepareUserAttributeListModel(UserAttributeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get user attributes
            var userAttributes = _userAttributeService.GetAllUserAttributes().ToPagedList(searchModel);

            //prepare list model
            var model = new UserAttributeListModel().PrepareToGrid(searchModel, userAttributes, () =>
            {
                return userAttributes.Select(attribute =>
                {
                    //fill in model values from the entity
                    var attributeModel = attribute.ToModel<UserAttributeModel>();

                    //fill in additional values (not existing in the entity)
                    attributeModel.AttributeControlTypeName = _localizationService.GetLocalizedEnum(attribute.AttributeControlType);

                    return attributeModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare user attribute model
        /// </summary>
        /// <param name="model">User attribute model</param>
        /// <param name="userAttribute">User attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User attribute model</returns>
        public virtual UserAttributeModel PrepareUserAttributeModel(UserAttributeModel model,
            UserAttribute userAttribute, bool excludeProperties = false)
        {
            Action<UserAttributeLocalizedModel, int> localizedModelConfiguration = null;

            if (userAttribute != null)
            {
                //fill in model values from the entity
                model ??= userAttribute.ToModel<UserAttributeModel>();

                //prepare nested search model
                PrepareUserAttributeValueSearchModel(model.UserAttributeValueSearchModel, userAttribute);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(userAttribute, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged user attribute value list model
        /// </summary>
        /// <param name="searchModel">User attribute value search model</param>
        /// <param name="userAttribute">User attribute</param>
        /// <returns>User attribute value list model</returns>
        public virtual UserAttributeValueListModel PrepareUserAttributeValueListModel(UserAttributeValueSearchModel searchModel,
            UserAttribute userAttribute)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            //get user attribute values
            var userAttributeValues = _userAttributeService
                .GetUserAttributeValues(userAttribute.Id).ToPagedList(searchModel);

            //prepare list model
            var model = new UserAttributeValueListModel().PrepareToGrid(searchModel, userAttributeValues, () =>
            {
                //fill in model values from the entity
                return userAttributeValues.Select(value => value.ToModel<UserAttributeValueModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare user attribute value model
        /// </summary>
        /// <param name="model">User attribute value model</param>
        /// <param name="userAttribute">User attribute</param>
        /// <param name="userAttributeValue">User attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>User attribute value model</returns>
        public virtual UserAttributeValueModel PrepareUserAttributeValueModel(UserAttributeValueModel model,
            UserAttribute userAttribute, UserAttributeValue userAttributeValue, bool excludeProperties = false)
        {
            if (userAttribute == null)
                throw new ArgumentNullException(nameof(userAttribute));

            Action<UserAttributeValueLocalizedModel, int> localizedModelConfiguration = null;

            if (userAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= userAttributeValue.ToModel<UserAttributeValueModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(userAttributeValue, entity => entity.Name, languageId, false, false);
                };
            }

            model.UserAttributeId = userAttribute.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}