using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Services.Cms;
using TvProgViewer.Services.Users;
using TvProgViewer.Web.Framework.Themes;
using TvProgViewer.WebUI.Infrastructure.Cache;
using TvProgViewer.WebUI.Models.Cms;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the widget model factory
    /// </summary>
    public partial class WidgetModelFactory : IWidgetModelFactory
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IWidgetPluginManager _widgetPluginManager;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetModelFactory(IUserService userService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext)
        {
            _userService = userService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _themeContext = themeContext;
            _widgetPluginManager = widgetPluginManager;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the render widget models
        /// </summary>
        /// <param name="widgetZone">Name of widget zone</param>
        /// <param name="additionalData">Additional data object</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the render widget models
        /// </returns>
        public virtual async Task<List<RenderWidgetModel>> PrepareRenderWidgetModelAsync(string widgetZone, object additionalData = null)
        {
            var theme = await _themeContext.GetWorkingThemeNameAsync();
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            var store = await _storeContext.GetCurrentStoreAsync();

            var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(TvProgModelCacheDefaults.WidgetModelKey,
                userRoleIds, store, widgetZone, theme);

            var cachedModels = await _staticCacheManager.GetAsync(cacheKey, async () =>
                (await _widgetPluginManager.LoadActivePluginsAsync(user, store.Id, widgetZone))
                .Select(widget => new RenderWidgetModel
                {
                    WidgetViewComponent = widget.GetWidgetViewComponent(widgetZone),
                    WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone }
                }));

            //"WidgetViewComponentArguments" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var models = cachedModels.Select(renderModel => new RenderWidgetModel
            {
                WidgetViewComponent = renderModel.WidgetViewComponent,
                WidgetViewComponentArguments = new RouteValueDictionary { ["widgetZone"] = widgetZone, ["additionalData"] = additionalData }
            }).ToList();

            return models;
        }

        #endregion
    }
}