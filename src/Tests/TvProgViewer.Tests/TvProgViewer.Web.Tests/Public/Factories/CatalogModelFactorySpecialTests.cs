using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Configuration;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.Catalog;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class CatalogModelFactorySpecialTests: WebTest
    {
        private ISettingService _settingsService;
        private CatalogSettings _catalogSettings;
        private Category _category;
        private VendorSettings _vendorSettings;
        private ICatalogModelFactory _catalogModelFactory;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _settingsService = GetService<ISettingService>();

            var categoryService = GetService<ICategoryService>();
            _category = await categoryService.GetCategoryByIdAsync(1);

            _vendorSettings = GetService<VendorSettings>();
            _vendorSettings.VendorsBlockItemsToDisplay = 1;
            _vendorSettings.AllowSearchByVendor = true;

            await _settingsService.SaveSettingAsync(_vendorSettings);
            
            _catalogSettings = GetService<CatalogSettings>();

            _catalogSettings.AllowTvChannelViewModeChanging = false;
            _catalogSettings.CategoryBreadcrumbEnabled = false;
            _catalogSettings.ShowTvChannelsFromSubcategories = true;
            _catalogSettings.ShowCategoryTvChannelNumber = true;
            _catalogSettings.ShowCategoryTvChannelNumberIncludingSubcategories = true;
            _catalogSettings.NumberOfTvChannelTags = 20;

            await _settingsService.SaveSettingAsync(_catalogSettings);

            _catalogModelFactory = GetService<ICatalogModelFactory>();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            _vendorSettings.VendorsBlockItemsToDisplay = 0;
            _vendorSettings.AllowSearchByVendor = false;
            await _settingsService.SaveSettingAsync(_vendorSettings);

            _catalogSettings.AllowTvChannelViewModeChanging = true;
            _catalogSettings.CategoryBreadcrumbEnabled = true;
            _catalogSettings.ShowTvChannelsFromSubcategories = false;
            _catalogSettings.ShowCategoryTvChannelNumber = false;
            _catalogSettings.ShowCategoryTvChannelNumberIncludingSubcategories = false;
            _catalogSettings.NumberOfTvChannelTags = 15;
            await _settingsService.SaveSettingAsync(_catalogSettings);
        }

        [Test]
        public async Task CanPrepareVendorNavigationModel()
        {
            var model = await _catalogModelFactory.PrepareVendorNavigationModelAsync();
            model.TotalVendors.Should().Be(2);
            model.Vendors.Any().Should().BeTrue();
            model.Vendors.Count.Should().Be(1);
            model.Vendors[0].Name.Should().Be("Vendor 1");
        }

        [Test]
        public async Task PrepareSearchModelShouldDependOnSettings()
        {
            var model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogTvChannelsCommand());
            
            model.AvailableVendors.Any().Should().BeTrue();
            model.AvailableVendors.Count.Should().Be(3);
        }
        
        [Test]
        public async Task PrepareCategoryModelShouldDependOnSettings()
        {
            var model = await _catalogModelFactory.PrepareCategoryModelAsync(_category, new CatalogTvChannelsCommand());
           
            model.CategoryBreadcrumb.Any().Should().BeFalse();
            model.SubCategories.Count.Should().Be(3);
            model.CatalogTvChannelsModel.TvChannels.Count.Should().Be(6);
        }
        
        [Test]
        public async Task CanPreparePopularTvChannelTagsModel()
        {
            var model = await _catalogModelFactory.PreparePopularTvChannelTagsModelAsync(_catalogSettings.NumberOfTvChannelTags);

            model.Tags.Count.Should().Be(16);
            model.TotalTags.Should().Be(16);
        }

        [Test]
        public async Task PrepareViewModesShouldDependOnSettings()
        {
            var model = new CatalogTvChannelsModel();
            await _catalogModelFactory.PrepareViewModesAsync(model, new CatalogTvChannelsCommand
            {
                ViewMode = "list"
            });

            model.AllowTvChannelViewModeChanging.Should().BeFalse();
            model.AvailableViewModes.Count.Should().Be(0);
            model.ViewMode.Should().Be("list");
        }

        [Test]
        public async Task PrepareCategorySimpleModelsShouldDependOnSettings()
        {
            var model = await _catalogModelFactory.PrepareCategorySimpleModelsAsync();

            var numberOfTvChannels = model
                .FirstOrDefault(p => p.Id == _category.Id)?.NumberOfTvChannels;

            numberOfTvChannels.Should().Be(12);
        }
    }
}