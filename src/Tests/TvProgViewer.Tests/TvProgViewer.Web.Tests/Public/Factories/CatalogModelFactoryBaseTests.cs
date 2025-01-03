﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Topics;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.Catalog;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class CatalogModelFactoryBaseTests : WebTest
    {
        private ICatalogModelFactory _catalogModelFactory;
        private Category _category;
        private TvChannel _tvChannel;
        private ICategoryService _categoryService;
        private Manufacturer _manufacturer;
        private Vendor _vendor;
        private ITopicService _topicService;
        private IHttpContextAccessor _httpContextAccessor;
        private TvChannelTag _tvChannelTag;
        private CatalogSettings _catalogSettings;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _catalogSettings = GetService<CatalogSettings>();
            _categoryService = GetService<ICategoryService>();
            _catalogModelFactory = GetService<ICatalogModelFactory>();
            _category = await _categoryService.GetCategoryByIdAsync(1);
            _tvChannel = await GetService<ITvChannelService>().GetTvChannelByIdAsync(1);
            _manufacturer = await GetService<IManufacturerService>().GetManufacturerByIdAsync(1);
            _vendor = await GetService<IVendorService>().GetVendorByIdAsync(1);
            _topicService = GetService<ITopicService>();
            _httpContextAccessor = GetService<IHttpContextAccessor>();

            _tvChannelTag = await GetService<ITvChannelTagService>().GetTvChannelTagByIdAsync(1);
        }

        [Test]
        public async Task CanPrepareSearchModel()
        {
            var model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogTvChannelsCommand());
            model.AvailableCategories.Any().Should().BeTrue();
            model.AvailableCategories.Count.Should().Be(17);

            model.AvailableManufacturers.Any().Should().BeTrue();
            model.AvailableManufacturers.Count.Should().Be(4);

            model.AvailableVendors.Any().Should().BeFalse();

            var queryString = _httpContextAccessor.HttpContext.Request.QueryString;
            _httpContextAccessor.HttpContext.Request.QueryString = new QueryString("?q=t");

            model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), new CatalogTvChannelsCommand());

            _httpContextAccessor.HttpContext.Request.QueryString = queryString;

            model.CatalogTvChannelsModel.WarningMessage.Should()
                .Be($"Search term minimum length is {_catalogSettings.TvChannelSearchTermMinimumLength} characters");
            model.CatalogTvChannelsModel.TvChannels.Count.Should().Be(0);

            _httpContextAccessor.HttpContext.Request.QueryString = new QueryString("?q=Lenovo");

            model = await _catalogModelFactory.PrepareSearchModelAsync(new SearchModel
            {
                q = "Lenovo"
            }, new CatalogTvChannelsCommand());
            _httpContextAccessor.HttpContext.Request.QueryString = queryString;

            model.CatalogTvChannelsModel.TvChannels.Count.Should().Be(2);
        }

        [Test]
        public async Task CanPrepareTopMenuModel()
        {
            var model = await _catalogModelFactory.PrepareTopMenuModelAsync();

            model.Categories.Count.Should().Be(7);
            model.Topics.Any().Should().BeFalse();
            model.NewTvChannelsEnabled.Should().BeTrue();
            model.BlogEnabled.Should().BeTrue();
            model.HasOnlyCategories.Should().BeTrue();

            var topic = await _topicService.GetTopicByIdAsync(1);

            topic.IncludeInTopMenu = true;
            await _topicService.UpdateTopicAsync(topic);
            model = await _catalogModelFactory.PrepareTopMenuModelAsync();
            topic.IncludeInTopMenu = false;

            await _topicService.UpdateTopicAsync(topic);
            model.Topics.Any().Should().BeTrue();
            model.Topics.Count.Should().Be(1);
        }
        
        [Test]
        public async Task CanPrepareCategoryModel()
        {
            var model = await _catalogModelFactory.PrepareCategoryModelAsync(_category, new CatalogTvChannelsCommand());

            model.Id.Should().Be(_category.Id);
            model.Name.Should().Be(_category.Name);
            model.Description.Should().Be(_category.Description);
            model.MetaKeywords.Should().Be(_category.MetaKeywords);
            model.MetaDescription.Should().Be(_category.MetaDescription);
            model.MetaTitle.Should().Be(_category.MetaTitle);

            model.CategoryBreadcrumb.Any().Should().BeTrue();
            model.CategoryBreadcrumb.FirstOrDefault()?.Name.Should().Be("Computers");
            model.SubCategories.Count.Should().Be(3);
        }
        
        [Test]
        public void PrepareCategoryModelShouldRaiseExceptionIfCategoryOrCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareCategoryModelAsync(null, new CatalogTvChannelsCommand()).Wait());

            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareCategoryModelAsync(_category, null).Wait());
        }
        
        [Test]
        public async Task CanPrepareCategoryTemplateViewPath()
        {
            var model = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(1);
            model.Should().Be("CategoryTemplate.TvChannelsInGridOrLines");

            model = await _catalogModelFactory.PrepareCategoryTemplateViewPathAsync(int.MaxValue);
            model.Should().Be("CategoryTemplate.TvChannelsInGridOrLines");
        }

        [Test]
        public async Task CanPrepareCategoryNavigationModel()
        {
            var model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(_category.Id, 0);

            model.Categories.Count.Should().Be(7);
            model.CurrentCategoryId.Should().Be(_category.Id);

            model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(0, _tvChannel.Id);
            model.Categories.Count.Should().Be(7);
            var tvChannelCategories = await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(_tvChannel.Id);
            model.CurrentCategoryId.Should().Be(tvChannelCategories.FirstOrDefault()?.CategoryId ?? 0);

            model = await _catalogModelFactory.PrepareCategoryNavigationModelAsync(_category.Id, _tvChannel.Id);

            model.Categories.Count.Should().Be(7);
            model.CurrentCategoryId.Should().Be(_category.Id);
        }

        [Test]
        public async Task CanPrepareHomepageCategoryModels()
        {
            var model = await _catalogModelFactory.PrepareHomepageCategoryModelsAsync();
            
            model.Any().Should().BeTrue();
            model.Count.Should().Be(3);
            
            var categories = new[] { "Electronics", "Apparel", "Digital downloads" };

            foreach (var categoryModel in model)
                categoryModel.Name.Should().BeOneOf(categories);
        }
        
        [Test]
        public async Task CanPrepareRootCategories()
        {
            var model = await _catalogModelFactory.PrepareRootCategoriesAsync();
            model.Any().Should().BeTrue();
            model.Count.Should().Be(7);
        }

        [Test]
        public async Task CanPrepareSubCategories()
        {
            var model = await _catalogModelFactory.PrepareSubCategoriesAsync(_category.Id);
            model.Any().Should().BeTrue();
            model.Count.Should().Be(3);

            var categories = new[] {"Desktops", "Notebooks", "Software"};

            foreach (var categorySimpleModel in model) 
                categorySimpleModel.Name.Should().BeOneOf(categories);
        }

        [Test]
        public async Task CanPrepareManufacturerModel()
        {
            var model = await _catalogModelFactory.PrepareManufacturerModelAsync(_manufacturer, new CatalogTvChannelsCommand());
            model.Id.Should().Be(_manufacturer.Id);
            model.Name.Should().Be(_manufacturer.Name);
            model.Description.Should().Be(_manufacturer.Description);
            model.MetaKeywords.Should().Be(_manufacturer.MetaKeywords);
            model.MetaDescription.Should().Be(_manufacturer.MetaDescription);
            model.MetaTitle.Should().Be(_manufacturer.MetaTitle);
        }

        [Test]
        public void PrepareManufacturerModelShouldRaiseExceptionIfManufacturerOrCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareManufacturerModelAsync(null, new CatalogTvChannelsCommand()).Wait());

            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareManufacturerModelAsync(_manufacturer, null).Wait());
        }

        [Test]
        public async Task CanPrepareManufacturerTemplateViewPath()
        {
            var model = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(1);
            model.Should().Be("ManufacturerTemplate.TvChannelsInGridOrLines");

            model = await _catalogModelFactory.PrepareManufacturerTemplateViewPathAsync(int.MaxValue);
            model.Should().Be("ManufacturerTemplate.TvChannelsInGridOrLines");
        }

        [Test]
        public async Task CanPrepareManufacturerAllModels()
        {
            var model = await _catalogModelFactory.PrepareManufacturerAllModelsAsync();
            model.Any().Should().BeTrue();
            model.Count.Should().Be(3);
            var manufacturers = new[] {"Apple", "HP", "Nike"};

            foreach (var manufacturerModel in model) 
                manufacturerModel.Name.Should().BeOneOf(manufacturers);
        }

        [Test]
        public async Task CanPrepareManufacturerNavigationModel()
        {
            var model = await _catalogModelFactory.PrepareManufacturerNavigationModelAsync(_manufacturer.Id);
            model.TotalManufacturers.Should().Be(3);
            model.Manufacturers.Any().Should().BeTrue();
            model.Manufacturers.Count.Should().Be(2);

            var manufacturers = new[] { "Apple", "HP" };

            foreach (var manufacturerModel in model.Manufacturers)
                manufacturerModel.Name.Should().BeOneOf(manufacturers);
        }

        [Test]
        public async Task CanPrepareVendorModel()
        {
            var model = await _catalogModelFactory.PrepareVendorModelAsync(_vendor, new CatalogTvChannelsCommand());

            model.Id.Should().Be(_vendor.Id);
            model.Name.Should().Be(_vendor.Name);
            model.Description.Should().Be(_vendor.Description);
            model.MetaKeywords.Should().Be(_vendor.MetaKeywords);
            model.MetaDescription.Should().Be(_vendor.MetaDescription);
            model.MetaTitle.Should().Be(_vendor.MetaTitle);
        }

        [Test]
        public void PrepareVendorModelShouldRaiseExceptionIfVendorOrCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareVendorModelAsync(null, new CatalogTvChannelsCommand()).Wait());

            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareVendorModelAsync(_vendor, null).Wait());
        }

        [Test]
        public async Task CanPrepareVendorAllModels()
        {
            var model = await _catalogModelFactory.PrepareVendorAllModelsAsync();

            model.Any().Should().BeTrue();
            model.Count.Should().Be(2);
            var vendors = new[] { "Vendor 1", "Vendor 2" };

            foreach (var manufacturerModel in model)
                manufacturerModel.Name.Should().BeOneOf(vendors);
        }

        [Test]
        public async Task CanPrepareTvChannelsByTagModel()
        {
            var model = await _catalogModelFactory.PrepareTvChannelsByTagModelAsync(_tvChannelTag, new CatalogTvChannelsCommand());

            model.Id.Should().Be(_tvChannelTag.Id);
            model.TagName.Should().Be(_tvChannelTag.Name);
            model.CatalogTvChannelsModel.TvChannels.Count.Should().Be(6);
        }

        [Test]
        public async Task CanPrepareSearchBoxModel()
        {
            var model = await _catalogModelFactory.PrepareSearchBoxModelAsync();

            model.AutoCompleteEnabled.Should().Be(_catalogSettings.TvChannelSearchAutoCompleteEnabled);
            model.ShowTvChannelImagesInSearchAutoComplete.Should().Be(_catalogSettings.ShowTvChannelImagesInSearchAutoComplete);
            model.SearchTermMinimumLength.Should().Be(_catalogSettings.TvChannelSearchTermMinimumLength);
            model.ShowSearchBox.Should().Be(_catalogSettings.TvChannelSearchEnabled);
        }

        [Test]
        public void PrepareVendorModelShouldRaiseExceptionIfTvChannelTagOrCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareTvChannelsByTagModelAsync(null, new CatalogTvChannelsCommand()).Wait());

            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareTvChannelsByTagModelAsync(_tvChannelTag, null).Wait());
        }

        [Test]
        public async Task CanPrepareTvChannelTagsAllModel()
        {
            var model = await _catalogModelFactory.PreparePopularTvChannelTagsModelAsync();
            model.Tags.Count.Should().Be(16);
        }

        [Test]
        public void PrepareSearchModelShouldRaiseExceptionIfSearchModelOrCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareSearchModelAsync(null, new CatalogTvChannelsCommand()).Wait());

            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareSearchModelAsync(new SearchModel(), null).Wait());
        }

        [Test]
        public async Task CanPrepareSortingOptions()
        {
            var model = new CatalogTvChannelsModel();
            var command = new CatalogTvChannelsCommand();
            await _catalogModelFactory.PrepareSortingOptionsAsync(model, command);

            model.AllowTvChannelSorting.Should().BeTrue();
            model.AvailableSortOptions.Count.Should().Be(4);
            command.OrderBy.Should().Be(0);
        }

        [Test]
        public void PrepareSortingOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareSortingOptionsAsync(null, new CatalogTvChannelsCommand()).Wait());

            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareSortingOptionsAsync(new CatalogTvChannelsModel(), null).Wait());
        }

        [Test]
        public async Task CanPrepareViewModes()
        {
            var model = new CatalogTvChannelsModel();
            await _catalogModelFactory.PrepareViewModesAsync(model, new CatalogTvChannelsCommand());

            model.AllowTvChannelViewModeChanging.Should().BeTrue();
            model.AvailableViewModes.Count.Should().Be(2);
            model.ViewMode.Should().Be("grid");
        }

        [Test]
        public void PrepareViewModesShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
        {
            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareViewModesAsync(null, new CatalogTvChannelsCommand()).Wait());

            Assert.Throws<AggregateException>(() =>
                _catalogModelFactory.PrepareViewModesAsync(new CatalogTvChannelsModel(), null).Wait());
        }

        [Test]
        public async Task CanPreparePageSizeOptions()
        {
            var pageSizes = "10, 20, 30";
            var model = new CatalogTvChannelsModel();
            var command = new CatalogTvChannelsCommand();
            await _catalogModelFactory.PreparePageSizeOptionsAsync(model, command, true, pageSizes, 0);

            model.AllowUsersToSelectPageSize.Should().BeTrue();
            model.PageSizeOptions.Count.Should().Be(3);

            foreach (var modelPageSizeOption in model.PageSizeOptions)
            {
                int.TryParse(modelPageSizeOption.Text, out _).Should().BeTrue();
                pageSizes.Contains(modelPageSizeOption.Text).Should().BeTrue();

                modelPageSizeOption.Value.Should().Be(modelPageSizeOption.Text);
            }

            command.PageSize.Should().Be(10);

            await _catalogModelFactory.PreparePageSizeOptionsAsync(model, command, false, "10, 20, 30", 15);

            model.AllowUsersToSelectPageSize.Should().BeFalse();
            command.PageSize.Should().Be(15);
            model.PageSizeOptions.Count.Should().Be(3);
        }

        [Test]
        public void PreparePageSizeOptionsShouldRaiseExceptionIfPagingFilteringModelOrCommandIsNull()
        {
            Assert.Throws<NullReferenceException>(() =>
                _catalogModelFactory.PreparePageSizeOptionsAsync(null, new CatalogTvChannelsCommand(), true, string.Empty, 15).Wait());

            Assert.Throws<NullReferenceException>(() =>
                _catalogModelFactory.PreparePageSizeOptionsAsync(new CatalogTvChannelsModel(), null, false, "10, 15, 20", 0).Wait());
        }

        [Test]
        public async Task CanPrepareCategorySimpleModels()
        {
            var model = await _catalogModelFactory.PrepareCategorySimpleModelsAsync();
            model.Any().Should().BeTrue();
            model.Count.Should().Be(7);

            model = await _catalogModelFactory.PrepareCategorySimpleModelsAsync(_category.Id);

            model.Any().Should().BeTrue();
            model.Count.Should().Be(3);

            var categories = new[] { "Desktops", "Notebooks", "Software" };

            foreach (var categoryModel in model)
                categoryModel.Name.Should().BeOneOf(categories);

            model = await _catalogModelFactory.PrepareCategorySimpleModelsAsync(_category.Id, false);

            model.Any().Should().BeTrue();
            model.Count.Should().Be(3);

            foreach (var categoryModel in model)
                categoryModel.Name.Should().BeOneOf(categories);
        }
    }
}
