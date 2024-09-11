using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Represents default values related to catalog services
    /// </summary>
    public static partial class TvProgCatalogDefaults
    {
        #region Common

        /// <summary>
        /// Gets a default price range 'from'
        /// </summary>
        public static decimal DefaultPriceRangeFrom => 0;

        /// <summary>
        /// Gets a default price range 'to'
        /// </summary>
        public static decimal DefaultPriceRangeTo => 10000;

        #endregion

        #region TvChannels

        /// <summary>
        /// Gets a template of tvChannel name on copying
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel name
        /// </remarks>
        public static string TvChannelCopyNameTemplate => "Copy of {0}";

        /// <summary>
        /// Gets default prefix for tvChannel
        /// </summary>
        public static string TvChannelAttributePrefix => "tvChannel_attribute_";

        #endregion

        #region Caching defaults

        #region Categories

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// {2} : current user ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey CategoriesByParentCategoryCacheKey => new("TvProg.category.byparent.{0}-{1}-{2}-{3}", CategoriesByParentCategoryPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesByParentCategoryPrefix => "TvProg.category.byparent.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesChildIdsCacheKey => new("TvProg.category.childids.{0}-{1}-{2}-{3}", CategoriesChildIdsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesChildIdsPrefix => "TvProg.category.childids.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey CategoriesHomepageCacheKey => new("TvProg.category.homepage.", CategoriesHomepagePrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// </remarks>
        public static CacheKey CategoriesHomepageWithoutHiddenCacheKey => new("TvProg.category.homepage.withouthidden-{0}-{1}", CategoriesHomepagePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesHomepagePrefix => "TvProg.category.homepage.";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static CacheKey CategoryBreadcrumbCacheKey => new("TvProg.category.breadcrumb.{0}-{1}-{2}-{3}", CategoryBreadcrumbPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryBreadcrumbPrefix => "TvProg.category.breadcrumb.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesAllCacheKey => new("TvProg.category.all.{0}-{1}-{2}", TvProgEntityCacheDefaults<Category>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// {1} : show hidden records?
        /// {2} : current user ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey TvChannelCategoriesByTvChannelCacheKey => new("TvProg.tvChannelcategory.bytvChannel.{0}-{1}-{2}-{3}", TvChannelCategoriesByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string TvChannelCategoriesByTvChannelPrefix => "TvProg.tvChannelcategory.bytvChannel.{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user roles ID hash
        /// {1} : current store ID
        /// {2} : categories ID hash
        /// </remarks>
        public static CacheKey CategoryTvChannelsNumberCacheKey => new("TvProg.tvChannelcategory.tvChannels.number.{0}-{1}-{2}", CategoryTvChannelsNumberPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryTvChannelsNumberPrefix => "TvProg.tvChannelcategory.tvChannels.number.";

        #endregion

        #region Manufacturers

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// {1} : show hidden records?
        /// {2} : current user ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey TvChannelManufacturersByTvChannelCacheKey => new("TvProg.tvChannelmanufacturer.bytvChannel.{0}-{1}-{2}-{3}", TvChannelManufacturersByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// </remarks>
        public static string TvChannelManufacturersByTvChannelPrefix => "TvProg.tvChannelmanufacturer.bytvChannel.{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        public static CacheKey ManufacturersByCategoryCacheKey => new("TvProg.manufacturer.bycategory.{0}", ManufacturersByCategoryPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ManufacturersByCategoryPrefix => "TvProg.manufacturer.bycategory.";

        #endregion

        #region TvChannels

        /// <summary>
        /// Key for "related" tvChannel displayed on the tvChannel details page
        /// </summary>
        /// <remarks>
        /// {0} : current tvChannel id
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey RelatedTvChannelsCacheKey => new("TvProg.relatedtvChannel.bytvChannel.{0}-{1}", RelatedTvChannelsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// </remarks>
        public static string RelatedTvChannelsPrefix => "TvProg.relatedtvChannel.bytvChannel.{0}";

        /// <summary>
        /// Key for "related" tvChannel identifiers displayed on the tvChannel details page
        /// </summary>
        /// <remarks>
        /// {0} : current tvChannel id
        /// </remarks>
        public static CacheKey TierPricesByTvChannelCacheKey => new("TvProg.tierprice.bytvChannel.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey TvChannelsHomepageCacheKey => new("TvProg.tvChannel.homepage.");

        /// <summary>
        /// Key for caching identifiers of category featured tvChannels
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : user role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryFeaturedTvChannelsIdsKey => new("TvProg.tvChannel.featured.bycategory.{0}-{1}-{2}", CategoryFeaturedTvChannelsIdsPrefix, FeaturedTvChannelIdsPrefix);
        public static string CategoryFeaturedTvChannelsIdsPrefix => "TvProg.tvChannel.featured.bycategory.{0}";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured tvChannels
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : user role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerFeaturedTvChannelIdsKey => new("TvProg.tvChannel.featured.bymanufacturer.{0}-{1}-{2}", ManufacturerFeaturedTvChannelIdsPrefix, FeaturedTvChannelIdsPrefix);
        public static string ManufacturerFeaturedTvChannelIdsPrefix => "TvProg.tvChannel.featured.bymanufacturer.{0}";

        public static string FeaturedTvChannelIdsPrefix => "TvProg.tvChannel.featured.";

        /// <summary>
        /// Gets a key for tvChannel prices
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel id
        /// {1} : overridden tvChannel price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey TvChannelPriceCacheKey => new("TvProg.totals.tvChannelprice.{0}-{1}-{2}-{3}-{4}-{5}-{6}", TvChannelPricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel id
        /// </remarks>
        public static string TvChannelPricePrefix => "TvProg.totals.tvChannelprice.{0}";

        /// <summary>
        /// Gets a key for tvChannel multiple prices
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel id
        /// {1} : user role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey TvChannelMultiplePriceCacheKey => new("TvProg.totals.tvChannelprice.multiple.{0}-{1}-{2}", TvChannelMultiplePricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel id
        /// </remarks>
        public static string TvChannelMultiplePricePrefix => "TvProg.totals.tvChannelprice.multiple.{0}";

        #endregion

        #region TvChannel attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// </remarks>
        public static CacheKey TvChannelAttributeMappingsByTvChannelCacheKey => new("TvProg.tvChannelattributemapping.bytvChannel.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel attribute mapping ID
        /// </remarks>
        public static CacheKey TvChannelAttributeValuesByAttributeCacheKey => new("TvProg.tvChannelattributevalue.byattribute.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// </remarks>
        public static CacheKey TvChannelAttributeCombinationsByTvChannelCacheKey => new("TvProg.tvChannelattributecombination.bytvChannel.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : TvChannel attribute ID
        /// </remarks>
        public static CacheKey PredefinedTvChannelAttributeValuesByAttributeCacheKey => new("TvProg.predefinedtvChannelattributevalue.byattribute.{0}");

        #endregion

        #region TvChannel tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of user roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey TvChannelTagCountCacheKey => new("TvProg.tvChanneltag.count.{0}-{1}-{2}", TvProgEntityCacheDefaults<TvChannelTag>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// </remarks>
        public static CacheKey TvChannelTagsByTvChannelCacheKey => new("TvProg.tvChanneltag.bytvChannel.{0}", TvProgEntityCacheDefaults<TvChannelTag>.Prefix);

        #endregion

        #region Review type

        /// <summary>
        /// Key for caching tvChannel review and review type mapping
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel review ID
        /// </remarks>
        public static CacheKey TvChannelReviewTypeMappingByReviewTypeCacheKey => new("TvProg.tvChannelreviewreviewtypemapping.byreviewtype.{0}");

        #endregion

        #region Specification attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// {1} : specification attribute option ID
        /// {2} : allow filtering
        /// {3} : show on tvChannel page
        /// {4} : specification attribute group ID
        /// </remarks>
        public static CacheKey TvChannelSpecificationAttributeByTvChannelCacheKey => new("TvProg.tvChannelspecificationattribute.bytvChannel.{0}-{1}-{2}-{3}-{4}", TvChannelSpecificationAttributeByTvChannelPrefix, TvChannelSpecificationAttributeAllByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// </remarks>
        public static string TvChannelSpecificationAttributeByTvChannelPrefix => "TvProg.tvChannelspecificationattribute.bytvChannel.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {1} (not 0, see the <ref>TvChannelSpecificationAttributeAllByTvChannelIdCacheKey</ref>) :specification attribute option ID
        /// </remarks>
        public static string TvChannelSpecificationAttributeAllByTvChannelPrefix => "TvProg.tvChannelspecificationattribute.bytvChannel.";

        /// <summary>
        /// Key for specification attributes caching (tvChannel details page)
        /// </summary>
        public static CacheKey SpecificationAttributesWithOptionsCacheKey => new("TvProg.specificationattribute.withoptions.");

        /// <summary>
        /// Key for specification attributes caching
        /// </summary>
        /// <remarks>
        /// {0} : specification attribute ID
        /// </remarks>
        public static CacheKey SpecificationAttributeOptionsCacheKey => new("TvProg.specificationattributeoption.byattribute.{0}");

        /// <summary>
        /// Key for specification attribute options by category ID caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// </remarks>
        public static CacheKey SpecificationAttributeOptionsByCategoryCacheKey => new("TvProg.specificationattributeoption.bycategory.{0}", FilterableSpecificationAttributeOptionsPrefix);

        /// <summary>
        /// Key for specification attribute options by manufacturer ID caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// </remarks>
        public static CacheKey SpecificationAttributeOptionsByManufacturerCacheKey => new("TvProg.specificationattributeoption.bymanufacturer.{0}", FilterableSpecificationAttributeOptionsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string FilterableSpecificationAttributeOptionsPrefix => "TvProg.specificationattributeoption";

        /// <summary>
        /// Gets a key for specification attribute groups caching by tvChannel id
        /// </summary>
        /// <remarks>
        /// {0} : tvChannel ID
        /// </remarks>
        public static CacheKey SpecificationAttributeGroupByTvChannelCacheKey => new("TvProg.specificationattributegroup.bytvChannel.{0}", SpecificationAttributeGroupByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SpecificationAttributeGroupByTvChannelPrefix => "TvProg.specificationattributegroup.bytvChannel.";

        #endregion

        #endregion
    }
}