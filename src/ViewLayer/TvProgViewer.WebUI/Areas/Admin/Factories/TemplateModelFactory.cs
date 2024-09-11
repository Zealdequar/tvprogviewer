using System;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Topics;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Templates;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the template model factory implementation
    /// </summary>
    public partial class TemplateModelFactory : ITemplateModelFactory
    {
        #region Fields

        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly ITvChannelTemplateService _tvChannelTemplateService;
        private readonly ITopicTemplateService _topicTemplateService;

        #endregion

        #region Ctor

        public TemplateModelFactory(ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            ITvChannelTemplateService tvChannelTemplateService,
            ITopicTemplateService topicTemplateService)
        {
            _categoryTemplateService = categoryTemplateService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _tvChannelTemplateService = tvChannelTemplateService;
            _topicTemplateService = topicTemplateService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Prepare templates model
        /// </summary>
        /// <param name="model">Templates model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the mplates model
        /// </returns>
        public virtual async Task<TemplatesModel> PrepareTemplatesModelAsync(TemplatesModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare nested search models
            await PrepareCategoryTemplateSearchModelAsync(model.TemplatesCategory);
            await PrepareManufacturerTemplateSearchModelAsync(model.TemplatesManufacturer);
            await PrepareTvChannelTemplateSearchModelAsync(model.TemplatesTvChannel);
            await PrepareTopicTemplateSearchModelAsync(model.TemplatesTopic);

            return model;
        }
        
        /// <summary>
        /// Prepare paged category template list model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category template list model
        /// </returns>
        public virtual async Task<CategoryTemplateListModel> PrepareCategoryTemplateListModelAsync(CategoryTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get category templates
            var categoryTemplates = (await _categoryTemplateService.GetAllCategoryTemplatesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new CategoryTemplateListModel().PrepareToGrid(searchModel, categoryTemplates,
                () => categoryTemplates.Select(template => template.ToModel<CategoryTemplateModel>()));

            return model;
        }
        
        /// <summary>
        /// Prepare paged manufacturer template list model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer template list model
        /// </returns>
        public virtual async Task<ManufacturerTemplateListModel> PrepareManufacturerTemplateListModelAsync(ManufacturerTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturer templates
            var manufacturerTemplates = (await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new ManufacturerTemplateListModel().PrepareToGrid(searchModel, manufacturerTemplates,
                () => manufacturerTemplates.Select(template => template.ToModel<ManufacturerTemplateModel>()));
            
            return model;
        }
        
        /// <summary>
        /// Prepare paged tvChannel template list model
        /// </summary>
        /// <param name="searchModel">TvChannel template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel template list model
        /// </returns>
        public virtual async Task<TvChannelTemplateListModel> PrepareTvChannelTemplateListModelAsync(TvChannelTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get tvChannel templates
            var tvChannelTemplates = (await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new TvChannelTemplateListModel().PrepareToGrid(searchModel, tvChannelTemplates,
                () => tvChannelTemplates.Select(template => template.ToModel<TvChannelTemplateModel>()));

            return model;
        }

        /// <summary>
        /// Prepare paged topic template list model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic template list model
        /// </returns>
        public virtual async Task<TopicTemplateListModel> PrepareTopicTemplateListModelAsync(TopicTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get topic templates
            var topicTemplates = (await _topicTemplateService.GetAllTopicTemplatesAsync()).ToPagedList(searchModel);

            //prepare grid model
            var model = new TopicTemplateListModel().PrepareToGrid(searchModel, topicTemplates,
                () => topicTemplates.Select(template => template.ToModel<TopicTemplateModel>()));

            return model;
        }
        
        /// <summary>
        /// Prepare category template search model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category template search model
        /// </returns>
        public virtual Task<CategoryTemplateSearchModel> PrepareCategoryTemplateSearchModelAsync(CategoryTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare manufacturer template search model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer template search model
        /// </returns>
        public virtual Task<ManufacturerTemplateSearchModel> PrepareManufacturerTemplateSearchModelAsync(ManufacturerTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare tvChannel template search model
        /// </summary>
        /// <param name="searchModel">TvChannel template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel template search model
        /// </returns>
        public virtual Task<TvChannelTemplateSearchModel> PrepareTvChannelTemplateSearchModelAsync(TvChannelTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare topic template search model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic template search model
        /// </returns>
        public virtual Task<TopicTemplateSearchModel> PrepareTopicTemplateSearchModelAsync(TopicTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        #endregion
    }
}