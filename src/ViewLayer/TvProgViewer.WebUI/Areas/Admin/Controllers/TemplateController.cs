using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Topics;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Templates;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class TemplateController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IPermissionService _permissionService;
        private readonly ITvChannelTemplateService _tvChannelTemplateService;
        private readonly ITemplateModelFactory _templateModelFactory;
        private readonly ITopicTemplateService _topicTemplateService;

        #endregion

        #region Ctor

        public TemplateController(ICategoryTemplateService categoryTemplateService,
            ILocalizationService localizationService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPermissionService permissionService,
            ITvChannelTemplateService tvChannelTemplateService,
            ITemplateModelFactory templateModelFactory,
            ITopicTemplateService topicTemplateService)
        {
            _categoryTemplateService = categoryTemplateService;
            _localizationService = localizationService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _permissionService = permissionService;
            _tvChannelTemplateService = tvChannelTemplateService;
            _templateModelFactory = templateModelFactory;
            _topicTemplateService = topicTemplateService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //prepare model
            var model = await _templateModelFactory.PrepareTemplatesModelAsync(new TemplatesModel());

            return View(model);
        }

        #region Category templates        

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplates(CategoryTemplateSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _templateModelFactory.PrepareCategoryTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplateUpdate(CategoryTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a category template with the specified id
            var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await _categoryTemplateService.UpdateCategoryTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplateAdd(CategoryTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new CategoryTemplate();
            template = model.ToEntity(template);
            await _categoryTemplateService.InsertCategoryTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryTemplateDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if ((await _categoryTemplateService.GetAllCategoryTemplatesAsync()).Count == 1)
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

            //try to get a category template with the specified id
            var template = await _categoryTemplateService.GetCategoryTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await _categoryTemplateService.DeleteCategoryTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #region Manufacturer templates        

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplates(ManufacturerTemplateSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _templateModelFactory.PrepareManufacturerTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplateUpdate(ManufacturerTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a manufacturer template with the specified id
            var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await _manufacturerTemplateService.UpdateManufacturerTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplateAdd(ManufacturerTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new ManufacturerTemplate();
            template = model.ToEntity(template);
            await _manufacturerTemplateService.InsertManufacturerTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerTemplateDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if ((await _manufacturerTemplateService.GetAllManufacturerTemplatesAsync()).Count == 1)
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

            //try to get a manufacturer template with the specified id
            var template = await _manufacturerTemplateService.GetManufacturerTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await _manufacturerTemplateService.DeleteManufacturerTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #region TvChannel templates
                
        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTemplates(TvChannelTemplateSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _templateModelFactory.PrepareTvChannelTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTemplateUpdate(TvChannelTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a tvChannel template with the specified id
            var template = await _tvChannelTemplateService.GetTvChannelTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await _tvChannelTemplateService.UpdateTvChannelTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTemplateAdd(TvChannelTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new TvChannelTemplate();
            template = model.ToEntity(template);
            await _tvChannelTemplateService.InsertTvChannelTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelTemplateDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if ((await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync()).Count == 1)
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

            //try to get a tvChannel template with the specified id
            var template = await _tvChannelTemplateService.GetTvChannelTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await _tvChannelTemplateService.DeleteTvChannelTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #region Topic templates
        
        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplates(TopicTemplateSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _templateModelFactory.PrepareTopicTemplateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplateUpdate(TopicTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a topic template with the specified id
            var template = await _topicTemplateService.GetTopicTemplateByIdAsync(model.Id)
                ?? throw new ArgumentException("No template found with the specified id");

            template = model.ToEntity(template);
            await _topicTemplateService.UpdateTopicTemplateAsync(template);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplateAdd(TopicTemplateModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var template = new TopicTemplate();
            template = model.ToEntity(template);
            await _topicTemplateService.InsertTopicTemplateAsync(template);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TopicTemplateDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            if ((await _topicTemplateService.GetAllTopicTemplatesAsync()).Count == 1)
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.System.Templates.NotDeleteOnlyOne"));

            //try to get a topic template with the specified id
            var template = await _topicTemplateService.GetTopicTemplateByIdAsync(id)
                ?? throw new ArgumentException("No template found with the specified id");

            await _topicTemplateService.DeleteTopicTemplateAsync(template);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}