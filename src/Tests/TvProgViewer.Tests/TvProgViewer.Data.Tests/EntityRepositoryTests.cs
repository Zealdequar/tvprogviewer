using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Data;

namespace TvProgViewer.Tests.TvProgViewer.Data.Tests
{
    [TestFixture]
    public class EntityRepositoryTests : BaseTvProgTest
    {
        private IStaticCacheManager _cacheManager;
        private CacheKey _cacheKey;

        [OneTimeSetUp]
        public void SetUp()
        {
            _cacheManager = GetService<IStaticCacheManager>();
            _cacheKey = new CacheKey("EntityRepositoryTestsCacheKey");
        }

        [TearDown]
        public async Task TearDown()
        {
            try
            {
                var tvChannelRepository = GetService<IRepository<TvChannel>>();
                var tvChannel = await tvChannelRepository.GetByIdAsync(2);
                tvChannel.Deleted = false;
                await tvChannelRepository.UpdateAsync(tvChannel);
                await _cacheManager.ClearAsync();
            }
            catch
            {
               //ignore 
            }

            SetDataProviderType(DataProviderType.Unknown);
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanGetById(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var tvChannelRepository = GetService<IRepository<TvChannel>>();

            var tvChannel = await tvChannelRepository.GetByIdAsync(1);
                    tvChannel.Should().NotBeNull();

            tvChannel = await tvChannelRepository.GetByIdAsync(2);
            tvChannel.Deleted = true;
            await tvChannelRepository.UpdateAsync(tvChannel);

            tvChannel = await tvChannelRepository.GetByIdAsync(2);
            tvChannel.Should().NotBeNull();
            tvChannel = await tvChannelRepository.GetByIdAsync(2, includeDeleted:false);
            tvChannel.Should().BeNull();

            tvChannel = await _cacheManager.GetAsync(_cacheKey, default(TvChannel));
            tvChannel.Should().BeNull();

            await tvChannelRepository.GetByIdAsync(1, _ => _cacheKey);
            tvChannel = await _cacheManager.GetAsync(_cacheKey, default(TvChannel));
            tvChannel.Should().NotBeNull();
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanGetByIds(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var tvChannelRepository = GetService<IRepository<TvChannel>>();

            var tvChannel = await tvChannelRepository.GetByIdAsync(2);
            tvChannel.Deleted = true;
            await tvChannelRepository.UpdateAsync(tvChannel);

            var ids = new List<int> {1, 2, 3};

            var tvChannels = await tvChannelRepository.GetByIdsAsync(ids);
            tvChannels.Count.Should().Be(3);

            tvChannels = await tvChannelRepository.GetByIdsAsync(ids, includeDeleted: false);
            tvChannels.Count.Should().Be(2);

            tvChannels = await _cacheManager.GetAsync(_cacheKey, default(IList<TvChannel>));
            tvChannels.Should().BeNull();

            await tvChannelRepository.GetByIdsAsync(ids, _ => _cacheKey);
            tvChannels = await _cacheManager.GetAsync(_cacheKey, default(IList<TvChannel>));
            tvChannels.Count.Should().Be(3);
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanGetAll(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var tvChannelRepository = GetService<IRepository<TvChannel>>();

            var tvChannel = await tvChannelRepository.GetByIdAsync(2);
            tvChannel.Deleted = true;
            await tvChannelRepository.UpdateAsync(tvChannel);

            var asyncRez = await tvChannelRepository.GetAllAsync(query => query, null);
            asyncRez.Count.Should().BeGreaterThan(0);

            var taskRez = await tvChannelRepository.GetAllAsync(Task.FromResult);
            taskRez.Count.Should().BeGreaterThan(0);

            var rez = tvChannelRepository.GetAll(query => query, manager => _cacheKey);
            rez.Count.Should().BeGreaterThan(0);

            rez = tvChannelRepository.GetAll(query => query);
            rez.Count.Should().BeGreaterThan(0);

            asyncRez.Count.Should().Be(rez.Count);
            rez.Count.Should().Be(taskRez.Count);

            rez = await tvChannelRepository.GetAllAsync(Task.FromResult, manager => Task.FromResult(_cacheKey));
            rez.Count.Should().BeGreaterThan(0);
            var fullCount = rez.Count;

            await _cacheManager.RemoveAsync(_cacheKey);

            rez = await tvChannelRepository.GetAllAsync(Task.FromResult, manager => Task.FromResult(default(CacheKey)), false);
            rez.Count.Should().BeGreaterThan(0);
            rez.Count.Should().BeLessThan(fullCount);
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanGetAllPaged(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var tvChannelRepository = GetService<IRepository<TvChannel>>();

            var tvChannel = await tvChannelRepository.GetByIdAsync(2);
            tvChannel.Deleted = true;
            await tvChannelRepository.UpdateAsync(tvChannel);

            var rez = await tvChannelRepository.GetAllPagedAsync(query => query);
            rez.Count.Should().BeGreaterThan(0);
            var fullCount = rez.Count;

            rez = await tvChannelRepository.GetAllPagedAsync(query => query, includeDeleted: false);
            rez.Count.Should().BeGreaterThan(0);
            rez.Count.Should().BeLessThan(fullCount);
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanInsert(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var taxCategoryRepository = GetService<IRepository<TaxCategory>>();
            var taxCategory = new TaxCategory {DisplayOrder = 10, Name = "test tax category"};
            taxCategory.Id.Should().Be(0);

            await taxCategoryRepository.InsertAsync(taxCategory);
            await taxCategoryRepository.DeleteAsync(taxCategory);

            taxCategory.Id.Should().BeGreaterThan(0);

            Assert.Throws<AggregateException>(() => taxCategoryRepository.InsertAsync(default(TaxCategory)).Wait());
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanUpdate(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var taxCategoryRepository = GetService<IRepository<TaxCategory>>();
            var taxCategory = new TaxCategory { DisplayOrder = 10, Name = "test tax category" };
            taxCategory.Id.Should().Be(0);

            await taxCategoryRepository.InsertAsync(taxCategory);
            await taxCategoryRepository.UpdateAsync(new TaxCategory
            {
                Id = taxCategory.Id,
                Name = "Updated test tax category"
            });
            var updatedTaxCategory = await taxCategoryRepository.GetByIdAsync(taxCategory.Id);
            await taxCategoryRepository.DeleteAsync(taxCategory);

            taxCategory.Id.Should().BeGreaterThan(0);
            updatedTaxCategory.Name.Should().NotBeEquivalentTo(taxCategory.Name);
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanDelete(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var taxCategoryRepository = GetService<IRepository<TaxCategory>>();
            var taxCategory = new TaxCategory { DisplayOrder = 10, Name = "test tax category" };
            await taxCategoryRepository.InsertAsync(taxCategory);
            await taxCategoryRepository.DeleteAsync(taxCategory);
            taxCategory.Id.Should().BeGreaterThan(0);
            taxCategory = await taxCategoryRepository.GetByIdAsync(taxCategory.Id);
            taxCategory.Should().BeNull();

            Assert.Throws<AggregateException>(() => taxCategoryRepository.DeleteAsync(default(TaxCategory)).Wait());
            Assert.Throws<AggregateException>(() => taxCategoryRepository.DeleteAsync(default(IList<TaxCategory>)).Wait());
            Assert.Throws<AggregateException>(() => taxCategoryRepository.DeleteAsync((Expression<Func<TaxCategory, bool>>)null).Wait());
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanLoadOriginalCopy(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var tvChannelRepository = GetService<IRepository<TvChannel>>();
            var tvChannel = await tvChannelRepository.GetByIdAsync(1, _ => default);
            tvChannel.Name = "test name";
            var tvChannelNew = await tvChannelRepository.GetByIdAsync(1, _ => default);
            var tvChannelOld = await tvChannelRepository.LoadOriginalCopyAsync(tvChannel);

            tvChannelOld.Name.Should().NotBeEquivalentTo(tvChannelNew.Name);
        }

        [Test]
        [TestCase(DataProviderType.Unknown)]
        [TestCase(DataProviderType.SqlServer)]
        [TestCase(DataProviderType.MySql)]
        [TestCase(DataProviderType.PostgreSQL)]
        public async Task CanTruncate(DataProviderType type)
        {
            if (!SetDataProviderType(type))
                return;

            var gdprConsentRepository = GetService<IRepository<GdprConsent>>();

            await gdprConsentRepository.InsertAsync(new List<GdprConsent>
            {
                new() {Message = "Test message 1"},
                new() {Message = "Test message 2"},
                new() {Message = "Test message 3"},
                new() {Message = "Test message 4"},
                new() {Message = "Test message 5"}
            });

            var rezWithContent = await gdprConsentRepository.GetAllAsync(query => query);
            await gdprConsentRepository.TruncateAsync();
            var rezWithoutContent = await gdprConsentRepository.GetAllAsync(query => query);

            var gdprConsent1 = new GdprConsent { Message = "Test message 1"};
            var gdprConsent2 = new GdprConsent { Message = "Test message 2" };

            await gdprConsentRepository.InsertAsync(gdprConsent1);
            await gdprConsentRepository.TruncateAsync(true);
            await gdprConsentRepository.InsertAsync(gdprConsent2);
            await gdprConsentRepository.TruncateAsync(true);

            rezWithContent.Count.Should().Be(5);
            rezWithoutContent.Count.Should().Be(0);
            gdprConsent1.Id.Should().NotBe(1);
            gdprConsent2.Id.Should().Be(1);
        }
    }
}
