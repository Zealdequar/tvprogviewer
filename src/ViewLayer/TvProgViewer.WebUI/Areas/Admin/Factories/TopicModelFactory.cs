﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Topics;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Topics;
using TvProgViewer.Web.Framework.Extensions;
using TvProgViewer.Web.Framework.Factories;
using TvProgViewer.Web.Framework.Models.Extensions;
using TvProgViewer.Web.Framework.Mvc.Routing;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the topic model factory implementation
    /// </summary>
    public partial class TopicModelFactory : ITopicModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ITvProgUrlHelper _nopUrlHelper;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly ITopicService _topicService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public TopicModelFactory(CatalogSettings catalogSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ITvProgUrlHelper nopUrlHelper,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
        {
            _catalogSettings = catalogSettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _nopUrlHelper = nopUrlHelper;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _topicService = topicService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare topic search model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic search model
        /// </returns>
        public virtual async Task<TopicSearchModel> PrepareTopicSearchModelAsync(TopicSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged topic list model
        /// </summary>
        /// <param name="searchModel">Topic search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic list model
        /// </returns>
        public virtual async Task<TopicListModel> PrepareTopicListModelAsync(TopicSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get topics
            var topics = await _topicService.GetAllTopicsAsync(showHidden: true,
                keywords: searchModel.SearchKeywords,
                storeId: searchModel.SearchStoreId,
                ignoreAcl: true);

            var pagedTopics = topics.ToPagedList(searchModel);

            //prepare grid model
            var model = await new TopicListModel().PrepareToGridAsync(searchModel, pagedTopics, () =>
            {
                return pagedTopics.SelectAwait(async topic =>
                {
                    //fill in model values from the entity
                    var topicModel = topic.ToModel<TopicModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    topicModel.Body = string.Empty;

                    topicModel.SeName = await _urlRecordService.GetSeNameAsync(topic, 0, true, false);

                    if (!string.IsNullOrEmpty(topicModel.SystemName))
                        topicModel.TopicName = topicModel.SystemName;
                    else
                        topicModel.TopicName = topicModel.Title;

                    return topicModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare topic model
        /// </summary>
        /// <param name="model">Topic model</param>
        /// <param name="topic">Topic</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic model
        /// </returns>
        public virtual async Task<TopicModel> PrepareTopicModelAsync(TopicModel model, Topic topic, bool excludeProperties = false)
        {
            Func<TopicLocalizedModel, int, Task> localizedModelConfiguration = null;

            if (topic != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = topic.ToModel<TopicModel>();
                    model.SeName = await _urlRecordService.GetSeNameAsync(topic, 0, true, false);
                }

                model.Url = await _nopUrlHelper
                    .RouteGenericUrlAsync<Topic>(new { SeName = await _urlRecordService.GetSeNameAsync(topic) }, _webHelper.GetCurrentRequestProtocol());

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Title = await _localizationService.GetLocalizedAsync(topic, entity => entity.Title, languageId, false, false);
                    locale.Body = await _localizationService.GetLocalizedAsync(topic, entity => entity.Body, languageId, false, false);
                    locale.MetaKeywords = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = await _localizationService.GetLocalizedAsync(topic, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = await _urlRecordService.GetSeNameAsync(topic, languageId, false, false);
                };
            }

            //set default values for the new model
            if (topic == null)
            {
                model.DisplayOrder = 1;
                model.Published = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            //prepare available topic templates
            await _baseAdminModelFactory.PrepareTopicTemplatesAsync(model.AvailableTopicTemplates, false);

            //prepare model user roles
            await _aclSupportedModelFactory.PrepareModelUserRolesAsync(model, topic, excludeProperties);

            //prepare model stores
            await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, topic, excludeProperties);

            return model;
        }

        #endregion
    }
}