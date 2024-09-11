using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.WebUI.Factories
{
    public partial interface ICatalogModelFactory
    {
        #region Categories

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category model
        /// </returns>
        Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepare category template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category template view path
        /// </returns>
        Task<string> PrepareCategoryTemplateViewPathAsync(int templateId);

        /// <summary>
        /// Prepare category navigation model
        /// </summary>
        /// <param name="currentCategoryId">Current category identifier</param>
        /// <param name="currentTvChannelId">Current tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category navigation model
        /// </returns>
        Task<CategoryNavigationModel> PrepareCategoryNavigationModelAsync(int currentCategoryId,
            int currentTvChannelId);

        /// <summary>
        /// Prepare top menu model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the op menu model
        /// </returns>
        Task<TopMenuModel> PrepareTopMenuModelAsync();

        /// <summary>
        /// Prepare homepage category models
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of homepage category models
        /// </returns>
        Task<List<CategoryModel>> PrepareHomepageCategoryModelsAsync();

        /// <summary>
        /// Prepare root categories for menu
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of category (simple) models
        /// </returns>
        Task<List<CategorySimpleModel>> PrepareRootCategoriesAsync();

        /// <summary>
        /// Prepare subcategories for menu
        /// </summary>
        /// <param name="id">Id of category to get subcategory</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        Task<List<CategorySimpleModel>> PrepareSubCategoriesAsync(int id);

        /// <summary>
        /// Prepares the category tvChannels model
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category tvChannels model
        /// </returns>
        Task<CatalogTvChannelsModel> PrepareCategoryTvChannelsModelAsync(Category category, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of category (simple) models
        /// </returns>
        Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync();

        /// <summary>
        /// Prepare category (simple) models
        /// </summary>
        /// <param name="rootCategoryId">Root category identifier</param>
        /// <param name="loadSubCategories">A value indicating whether subcategories should be loaded</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of category (simple) models
        /// </returns>
        Task<List<CategorySimpleModel>> PrepareCategorySimpleModelsAsync(int rootCategoryId, bool loadSubCategories = true);

        /// <summary>
        /// Prepare category (simple) xml document
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the xml document of category (simple) models
        /// </returns>
        Task<XDocument> PrepareCategoryXmlDocumentAsync();

        #endregion

        #region Manufacturers

        /// <summary>
        /// Prepare manufacturer model
        /// </summary>
        /// <param name="manufacturer">Manufacturer identifier</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer model
        /// </returns>
        Task<ManufacturerModel> PrepareManufacturerModelAsync(Manufacturer manufacturer, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepares the manufacturer tvChannels model
        /// </summary>
        /// <param name="manufacturer">Manufacturer</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer tvChannels model
        /// </returns>
        Task<CatalogTvChannelsModel> PrepareManufacturerTvChannelsModelAsync(Manufacturer manufacturer, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepare manufacturer template view path
        /// </summary>
        /// <param name="templateId">Template identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer template view path
        /// </returns>
        Task<string> PrepareManufacturerTemplateViewPathAsync(int templateId);

        /// <summary>
        /// Prepare manufacturer all models
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of manufacturer models
        /// </returns>
        Task<List<ManufacturerModel>> PrepareManufacturerAllModelsAsync();

        /// <summary>
        /// Prepare manufacturer navigation model
        /// </summary>
        /// <param name="currentManufacturerId">Current manufacturer identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer navigation model
        /// </returns>
        Task<ManufacturerNavigationModel> PrepareManufacturerNavigationModelAsync(int currentManufacturerId);

        #endregion

        #region Vendors

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor model
        /// </returns>
        Task<VendorModel> PrepareVendorModelAsync(Vendor vendor, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepares the vendor tvChannels model
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor tvChannels model
        /// </returns>
        Task<CatalogTvChannelsModel> PrepareVendorTvChannelsModelAsync(Vendor vendor, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepare vendor all models
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of vendor models
        /// </returns>
        Task<List<VendorModel>> PrepareVendorAllModelsAsync();

        /// <summary>
        /// Prepare vendor navigation model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vendor navigation model
        /// </returns>
        Task<VendorNavigationModel> PrepareVendorNavigationModelAsync();

        #endregion

        #region TvChannel tags

        /// <summary>
        /// Prepare popular tvChannel tags model
        /// </summary>
        /// <param name="numberTagsToReturn">The number of tags to be returned; pass 0 to get all tags</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tags model
        /// </returns>
        Task<PopularTvChannelTagsModel> PreparePopularTvChannelTagsModelAsync(int numberTagsToReturn = 0);

        /// <summary>
        /// Prepare tvChannels by tag model
        /// </summary>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels by tag model
        /// </returns>
        Task<TvChannelsByTagModel> PrepareTvChannelsByTagModelAsync(TvChannelTag tvChannelTag, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepares the tag tvChannels model
        /// </summary>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ag tvChannels model
        /// </returns>
        Task<CatalogTvChannelsModel> PrepareTagTvChannelsModelAsync(TvChannelTag tvChannelTag, CatalogTvChannelsCommand command);

        #endregion

        #region New tvChannels

        /// <summary>
        /// Prepare new tvChannels model
        /// </summary>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the new tvChannels model
        /// </returns>
        Task<CatalogTvChannelsModel> PrepareNewTvChannelsModelAsync(CatalogTvChannelsCommand command);

        #endregion

        #region Searching

        /// <summary>
        /// Prepare search model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the search model
        /// </returns>
        Task<SearchModel> PrepareSearchModelAsync(SearchModel model, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepares the search tvChannels model
        /// </summary>
        /// <param name="model">Search model</param>
        /// <param name="command">Model to get the catalog tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the search tvChannels model
        /// </returns>
        Task<CatalogTvChannelsModel> PrepareSearchTvChannelsModelAsync(SearchModel searchModel, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepare search box model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the search box model
        /// </returns>
        Task<SearchBoxModel> PrepareSearchBoxModelAsync();

        #endregion

        #region Common

        /// <summary>
        /// Prepare sorting options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PrepareSortingOptionsAsync(CatalogTvChannelsModel pagingFilteringModel, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepare view modes
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PrepareViewModesAsync(CatalogTvChannelsModel pagingFilteringModel, CatalogTvChannelsCommand command);

        /// <summary>
        /// Prepare page size options
        /// </summary>
        /// <param name="pagingFilteringModel">Catalog paging filtering model</param>
        /// <param name="command">Catalog paging filtering command</param>
        /// <param name="allowUsersToSelectPageSize">Are users allowed to select page size?</param>
        /// <param name="pageSizeOptions">Page size options</param>
        /// <param name="fixedPageSize">Fixed page size</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PreparePageSizeOptionsAsync(CatalogTvChannelsModel pagingFilteringModel, CatalogTvChannelsCommand command,
            bool allowUsersToSelectPageSize, string pageSizeOptions, int fixedPageSize);

        #endregion
    }
}