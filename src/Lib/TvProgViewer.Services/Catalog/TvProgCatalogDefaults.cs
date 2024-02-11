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
        /// Gets a template of tvchannel name on copying
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel name
        /// </remarks>
        public static string TvChannelCopyNameTemplate => "Copy of {0}";

        /// <summary>
        /// Gets default prefix for tvchannel
        /// </summary>
        public static string TvChannelAttributePrefix => "tvchannel_attribute_";

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
        /// {0} : tvchannel ID
        /// {1} : show hidden records?
        /// {2} : current user ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey TvChannelCategoriesByTvChannelCacheKey => new("TvProg.tvchannelcategory.bytvchannel.{0}-{1}-{2}-{3}", TvChannelCategoriesByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string TvChannelCategoriesByTvChannelPrefix => "TvProg.tvchannelcategory.bytvchannel.{0}";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : user roles ID hash
        /// {1} : current store ID
        /// {2} : categories ID hash
        /// </remarks>
        public static CacheKey CategoryTvChannelsNumberCacheKey => new("TvProg.tvchannelcategory.tvchannels.number.{0}-{1}-{2}", CategoryTvChannelsNumberPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryTvChannelsNumberPrefix => "TvProg.tvchannelcategory.tvchannels.number.";

        #endregion

        #region Manufacturers

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// {1} : show hidden records?
        /// {2} : current user ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey TvChannelManufacturersByTvChannelCacheKey => new("TvProg.tvchannelmanufacturer.bytvchannel.{0}-{1}-{2}-{3}", TvChannelManufacturersByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// </remarks>
        public static string TvChannelManufacturersByTvChannelPrefix => "TvProg.tvchannelmanufacturer.bytvchannel.{0}";

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
        /// Key for "related" tvchannel displayed on the tvchannel details page
        /// </summary>
        /// <remarks>
        /// {0} : current tvchannel id
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey RelatedTvChannelsCacheKey => new("TvProg.relatedtvchannel.bytvchannel.{0}-{1}", RelatedTvChannelsPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// </remarks>
        public static string RelatedTvChannelsPrefix => "TvProg.relatedtvchannel.bytvchannel.{0}";

        /// <summary>
        /// Key for "related" tvchannel identifiers displayed on the tvchannel details page
        /// </summary>
        /// <remarks>
        /// {0} : current tvchannel id
        /// </remarks>
        public static CacheKey TierPricesByTvChannelCacheKey => new("TvProg.tierprice.bytvchannel.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey TvChannelsHomepageCacheKey => new("TvProg.tvchannel.homepage.");

        /// <summary>
        /// Key for caching identifiers of category featured tvchannels
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : user role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey CategoryFeaturedTvChannelsIdsKey => new("TvProg.tvchannel.featured.bycategory.{0}-{1}-{2}", CategoryFeaturedTvChannelsIdsPrefix, FeaturedTvChannelIdsPrefix);
        public static string CategoryFeaturedTvChannelsIdsPrefix => "TvProg.tvchannel.featured.bycategory.{0}";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured tvchannels
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : user role Ids
        /// {2} : current store ID
        /// </remarks>
        public static CacheKey ManufacturerFeaturedTvChannelIdsKey => new("TvProg.tvchannel.featured.bymanufacturer.{0}-{1}-{2}", ManufacturerFeaturedTvChannelIdsPrefix, FeaturedTvChannelIdsPrefix);
        public static string ManufacturerFeaturedTvChannelIdsPrefix => "TvProg.tvchannel.featured.bymanufacturer.{0}";

        public static string FeaturedTvChannelIdsPrefix => "TvProg.tvchannel.featured.";

        /// <summary>
        /// Gets a key for tvchannel prices
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel id
        /// {1} : overridden tvchannel price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey TvChannelPriceCacheKey => new("TvProg.totals.tvchannelprice.{0}-{1}-{2}-{3}-{4}-{5}-{6}", TvChannelPricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel id
        /// </remarks>
        public static string TvChannelPricePrefix => "TvProg.totals.tvchannelprice.{0}";

        /// <summary>
        /// Gets a key for tvchannel multiple prices
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel id
        /// {1} : user role ids
        /// {2} : store id
        /// </remarks>
        public static CacheKey TvChannelMultiplePriceCacheKey => new("TvProg.totals.tvchannelprice.multiple.{0}-{1}-{2}", TvChannelMultiplePricePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel id
        /// </remarks>
        public static string TvChannelMultiplePricePrefix => "TvProg.totals.tvchannelprice.multiple.{0}";

        #endregion

        #region TvChannel attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// </remarks>
        public static CacheKey TvChannelAttributeMappingsByTvChannelCacheKey => new("TvProg.tvchannelattributemapping.bytvchannel.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel attribute mapping ID
        /// </remarks>
        public static CacheKey TvChannelAttributeValuesByAttributeCacheKey => new("TvProg.tvchannelattributevalue.byattribute.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// </remarks>
        public static CacheKey TvChannelAttributeCombinationsByTvChannelCacheKey => new("TvProg.tvchannelattributecombination.bytvchannel.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : TvChannel attribute ID
        /// </remarks>
        public static CacheKey PredefinedTvChannelAttributeValuesByAttributeCacheKey => new("TvProg.predefinedtvchannelattributevalue.byattribute.{0}");

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
        public static CacheKey TvChannelTagCountCacheKey => new("TvProg.tvchanneltag.count.{0}-{1}-{2}", TvProgEntityCacheDefaults<TvChannelTag>.Prefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// </remarks>
        public static CacheKey TvChannelTagsByTvChannelCacheKey => new("TvProg.tvchanneltag.bytvchannel.{0}", TvProgEntityCacheDefaults<TvChannelTag>.Prefix);

        #endregion

        #region Review type

        /// <summary>
        /// Key for caching tvchannel review and review type mapping
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel review ID
        /// </remarks>
        public static CacheKey TvChannelReviewTypeMappingByReviewTypeCacheKey => new("TvProg.tvchannelreviewreviewtypemapping.byreviewtype.{0}");

        #endregion

        #region Specification attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// {1} : specification attribute option ID
        /// {2} : allow filtering
        /// {3} : show on tvchannel page
        /// {4} : specification attribute group ID
        /// </remarks>
        public static CacheKey TvChannelSpecificationAttributeByTvChannelCacheKey => new("TvProg.tvchannelspecificationattribute.bytvchannel.{0}-{1}-{2}-{3}-{4}", TvChannelSpecificationAttributeByTvChannelPrefix, TvChannelSpecificationAttributeAllByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// </remarks>
        public static string TvChannelSpecificationAttributeByTvChannelPrefix => "TvProg.tvchannelspecificationattribute.bytvchannel.{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {1} (not 0, see the <ref>TvChannelSpecificationAttributeAllByTvChannelIdCacheKey</ref>) :specification attribute option ID
        /// </remarks>
        public static string TvChannelSpecificationAttributeAllByTvChannelPrefix => "TvProg.tvchannelspecificationattribute.bytvchannel.";

        /// <summary>
        /// Key for specification attributes caching (tvchannel details page)
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
        /// Gets a key for specification attribute groups caching by tvchannel id
        /// </summary>
        /// <remarks>
        /// {0} : tvchannel ID
        /// </remarks>
        public static CacheKey SpecificationAttributeGroupByTvChannelCacheKey => new("TvProg.specificationattributegroup.bytvchannel.{0}", SpecificationAttributeGroupByTvChannelPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SpecificationAttributeGroupByTvChannelPrefix => "TvProg.specificationattributegroup.bytvchannel.";

        #endregion

        #endregion
    }
}