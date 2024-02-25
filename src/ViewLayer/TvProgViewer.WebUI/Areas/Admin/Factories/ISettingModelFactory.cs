using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.WebUI.Areas.Admin.Models.Settings;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the setting model factory
    /// </summary>
    public partial interface ISettingModelFactory
    {
        /// <summary>
        /// Prepare app settings model
        /// </summary>
        /// <param name="model">AppSettings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the app settings model
        /// </returns>
        Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null);

        /// <summary>
        /// Prepare blog settings model
        /// </summary>
        /// <param name="model">Blog settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the blog settings model
        /// </returns>
        Task<BlogSettingsModel> PrepareBlogSettingsModelAsync(BlogSettingsModel model = null);

        /// <summary>
        /// Prepare vendor settings model
        /// </summary>
        /// <param name="model">Vendor settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor settings model
        /// </returns>
        Task<VendorSettingsModel> PrepareVendorSettingsModelAsync(VendorSettingsModel model = null);

        /// <summary>
        /// Prepare forum settings model
        /// </summary>
        /// <param name="model">Forum settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the forum settings model
        /// </returns>
        Task<ForumSettingsModel> PrepareForumSettingsModelAsync(ForumSettingsModel model = null);

        /// <summary>
        /// Prepare news settings model
        /// </summary>
        /// <param name="model">News settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the news settings model
        /// </returns>
        Task<NewsSettingsModel> PrepareNewsSettingsModelAsync(NewsSettingsModel model = null);

        /// <summary>
        /// Prepare shipping settings model
        /// </summary>
        /// <param name="model">Shipping settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping settings model
        /// </returns>
        Task<ShippingSettingsModel> PrepareShippingSettingsModelAsync(ShippingSettingsModel model = null);

        /// <summary>
        /// Prepare tax settings model
        /// </summary>
        /// <param name="model">Tax settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax settings model
        /// </returns>
        Task<TaxSettingsModel> PrepareTaxSettingsModelAsync(TaxSettingsModel model = null);

        /// <summary>
        /// Prepare catalog settings model
        /// </summary>
        /// <param name="model">Catalog settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the catalog settings model
        /// </returns>
        Task<CatalogSettingsModel> PrepareCatalogSettingsModelAsync(CatalogSettingsModel model = null);

        /// <summary>
        /// Prepare paged sort option list model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sort option list model
        /// </returns>
        Task<SortOptionListModel> PrepareSortOptionListModelAsync(SortOptionSearchModel searchModel);

        /// <summary>
        /// Prepare reward points settings model
        /// </summary>
        ///<param name="model">Reward points settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reward points settings model
        /// </returns>
        Task<RewardPointsSettingsModel> PrepareRewardPointsSettingsModelAsync(RewardPointsSettingsModel model = null);

        /// <summary>
        /// Prepare order settings model
        /// </summary>
        /// <param name="model">Order settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order settings model
        /// </returns>
        Task<OrderSettingsModel> PrepareOrderSettingsModelAsync(OrderSettingsModel model = null);

        /// <summary>
        /// Prepare shopping cart settings model
        /// </summary>
        /// <param name="model">Shopping cart settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart settings model
        /// </returns>
        Task<ShoppingCartSettingsModel> PrepareShoppingCartSettingsModelAsync(ShoppingCartSettingsModel model = null);

        /// <summary>
        /// Prepare media settings model
        /// </summary>
        /// <param name="model">Media settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the media settings model
        /// </returns>
        Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null);

        /// <summary>
        /// Prepare user user settings model
        /// </summary>
        /// <param name="model">User user settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user user settings model
        /// </returns>
        Task<UserUserSettingsModel> PrepareUserUserSettingsModelAsync(UserUserSettingsModel model = null);

        /// <summary>
        /// Prepare GDPR settings model
        /// </summary>
        /// <param name="model">Gdpr settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR settings model
        /// </returns>
        Task<GdprSettingsModel> PrepareGdprSettingsModelAsync(GdprSettingsModel model = null);

        /// <summary>
        /// Prepare paged GDPR consent list model
        /// </summary>
        /// <param name="searchModel">GDPR search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR consent list model
        /// </returns>
        Task<GdprConsentListModel> PrepareGdprConsentListModelAsync(GdprConsentSearchModel searchModel);

        /// <summary>
        /// Prepare GDPR consent model
        /// </summary>
        /// <param name="model">GDPR consent model</param>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR consent model
        /// </returns>
        Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsentModel model, GdprConsent gdprConsent, bool excludeProperties = false);

        /// <summary>
        /// Prepare general and common settings model
        /// </summary>
        /// <param name="model">General common settings model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the general and common settings model
        /// </returns>
        Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null);

        /// <summary>
        /// Prepare tvchannel editor settings model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel editor settings model
        /// </returns>
        Task<TvChannelEditorSettingsModel> PrepareTvChannelEditorSettingsModelAsync();

        /// <summary>
        /// Prepare setting search model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the setting search model
        /// </returns>
        Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare paged setting list model
        /// </summary>
        /// <param name="searchModel">Setting search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the setting list model
        /// </returns>
        Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel);

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the setting mode model
        /// </returns>
        Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName);

        /// <summary>
        /// Prepare store scope configuration model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the store scope configuration model
        /// </returns>
        Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync();
    }
}