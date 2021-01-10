using TVProgViewer.Core.Caching;

namespace TVProgViewer.Services.Caching.CachingDefaults
{
    /// <summary>
    /// Represents default values related to catalog services
    /// </summary>
    public static partial class TvProgCatalogCachingDefaults
    {
        #region Categories

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey CategoriesByParentCategoryIdCacheKey => new CacheKey("TVProgViewer.category.byparent-{0}-{1}-{2}-{3}", CategoriesByParentCategoryPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesByParentCategoryPrefixCacheKey => "TVProgViewer.category.byparent-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// {3} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesChildIdentifiersCacheKey => new CacheKey("TVProgViewer.category.childidentifiers-{0}-{1}-{2}-{3}", CategoriesChildIdentifiersPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : parent category ID
        /// </remarks>
        public static string CategoriesChildIdentifiersPrefixCacheKey => "TVProgViewer.category.childidentifiers-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey CategoriesAllDisplayedOnHomepageCacheKey => new CacheKey("TVProgViewer.category.homepage.all", CategoriesDisplayedOnHomepagePrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : roles of the current user
        /// </remarks>
        public static CacheKey CategoriesDisplayedOnHomepageWithoutHiddenCacheKey => new CacheKey("TVProgViewer.category.homepage.withouthidden-{0}-{1}", CategoriesDisplayedOnHomepagePrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesDisplayedOnHomepagePrefixCacheKey => "TVProgViewer.category.homepage";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public static CacheKey CategoryBreadcrumbCacheKey => new CacheKey("TVProgViewer.category.breadcrumb-{0}-{1}-{2}-{3}", CategoryBreadcrumbPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryBreadcrumbPrefixCacheKey => "TVProgViewer.category.breadcrumb";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : comma separated list of customer roles
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey CategoriesAllCacheKey => new CacheKey("TVProgViewer.category.all-{0}-{1}-{2}", CategoriesAllPrefixCacheKey);
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoriesAllPrefixCacheKey => "TVProgViewer.category.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : category ID
        /// {1} : show hidden records?
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static CacheKey ProductCategoriesAllByCategoryIdCacheKey => new CacheKey("TVProgViewer.productcategory.allbycategoryid-{0}-{1}-{2}-{3}-{4}-{5}", ProductCategoriesByCategoryPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoriesByCategoryPrefixCacheKey => "TVProgViewer.productcategory.allbycategoryid-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey ProductCategoriesAllByProductIdCacheKey => new CacheKey("TVProgViewer.productcategory.allbyproductid-{0}-{1}-{2}-{3}", ProductCategoriesByProductPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductCategoriesByProductPrefixCacheKey => "TVProgViewer.productcategory.allbyproductid-{0}";
        
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer roles ID hash
        /// {1} : current store ID
        /// {2} : categories ID hash
        /// </remarks>
        public static CacheKey CategoryNumberOfProductsCacheKey => new CacheKey("TVProgViewer.productcategory.numberofproducts-{0}-{1}-{2}", CategoryNumberOfProductsPrefixCacheKey);
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CategoryNumberOfProductsPrefixCacheKey => "TVProgViewer.productcategory.numberofproducts";

        #endregion

        #region Manufacturers

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// {1} : show hidden records?
        /// {2} : page index
        /// {3} : page size
        /// {4} : current customer ID
        /// {5} : store ID
        /// </remarks>
        public static CacheKey ProductManufacturersAllByManufacturerIdCacheKey => new CacheKey("TVProgViewer.productmanufacturer.allbymanufacturerid-{0}-{1}-{2}-{3}-{4}-{5}", ProductManufacturersByManufacturerPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer ID
        /// </remarks>
        public static string ProductManufacturersByManufacturerPrefixCacheKey => "TVProgViewer.productmanufacturer.allbymanufacturerid-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : show hidden records?
        /// {2} : current customer ID
        /// {3} : store ID
        /// </remarks>
        public static CacheKey ProductManufacturersAllByProductIdCacheKey => new CacheKey("TVProgViewer.productmanufacturer.allbyproductid-{0}-{1}-{2}-{3}", ProductManufacturersByProductPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductManufacturersByProductPrefixCacheKey => "TVProgViewer.productmanufacturer.allbyproductid-{0}";

        #endregion

        #region Products

        /// <summary>
        /// Key for "related" product displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : show hidden records?
        /// </remarks>
        public static CacheKey ProductsRelatedCacheKey => new CacheKey("TVProgViewer.product.related-{0}-{1}", ProductsRelatedPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductsRelatedPrefixCacheKey => "TVProgViewer.product.related-{0}";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// </remarks>
        public static CacheKey ProductTierPricesCacheKey => new CacheKey("TVProgViewer.product.tierprices-{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductsAllDisplayedOnHomepageCacheKey => new CacheKey("TVProgViewer.product.homepage");

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product IDs hash
        /// </remarks>
        public static CacheKey ProductsByIdsCacheKey => new CacheKey("TVProgViewer.product.ids-{0}", ProductsByIdsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductsByIdsPrefixCacheKey => "TVProgViewer.product.ids";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product atribute ID
        /// </remarks>
        public static CacheKey ProductsByProductAtributeCacheKey => new CacheKey("TVProgViewer.product.productatribute-{0}");
        
        /// <summary>
        /// Gets a key for product prices
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : overridden product price
        /// {2} : additional charge
        /// {3} : include discounts (true, false)
        /// {4} : quantity
        /// {5} : roles of the current user
        /// {6} : current store ID
        /// </remarks>
        public static CacheKey ProductPriceCacheKey => new CacheKey("TVProgViewer.totals.productprice-{0}-{1}-{2}-{3}-{4}-{5}-{6}", ProductPricePrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public static string ProductPricePrefixCacheKey => "TVProgViewer.totals.productprice-{0}";
        
        #endregion

        #region Product attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : page index
        /// {1} : page size
        /// </remarks>
        public static CacheKey ProductAttributesAllCacheKey => new CacheKey("TVProgViewer.productattribute.all-{0}-{1}", ProductAttributesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributesAllPrefixCacheKey => "TVProgViewer.productattribute.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeMappingsAllCacheKey => new CacheKey("TVProgViewer.productattributemapping.all-{0}", ProductAttributeMappingsPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeMappingsPrefixCacheKey => "TVProgViewer.productattributemapping.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product attribute mapping ID
        /// </remarks>
        public static CacheKey ProductAttributeValuesAllCacheKey => new CacheKey("TVProgViewer.productattributevalue.all-{0}", ProductAttributeValuesAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeValuesAllPrefixCacheKey => "TVProgViewer.productattributevalue.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductAttributeCombinationsAllCacheKey => new CacheKey("TVProgViewer.productattributecombination.all-{0}", ProductAttributeCombinationsAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductAttributeCombinationsAllPrefixCacheKey => "TVProgViewer.productattributecombination.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : Product attribute ID
        /// </remarks>
        public static CacheKey PredefinedProductAttributeValuesAllCacheKey => new CacheKey("TVProgViewer.predefinedproductattributevalues.all-{0}");

        #endregion

        #region Product tags

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductTagAllCacheKey => new CacheKey("TVProgViewer.producttag.all", ProductTagPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store ID
        /// {1} : hash of list of customer roles IDs
        /// {2} : show hidden records?
        /// </remarks>
        public static CacheKey ProductTagCountCacheKey => new CacheKey("TVProgViewer.producttag.all.count-{0}-{1}-{2}", ProductTagPrefixCacheKey);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static CacheKey ProductTagAllByProductIdCacheKey => new CacheKey("TVProgViewer.producttag.allbyproductid-{0}", ProductTagPrefixCacheKey);
        
        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductTagPrefixCacheKey => "TVProgViewer.producttag.";

        #endregion

        #region Review type

        /// <summary>
        /// Key for caching all review types
        /// </summary>
        public static CacheKey ReviewTypeAllCacheKey => new CacheKey("TVProgViewer.reviewType.all");
        
        /// <summary>
        /// Key for caching product review and review type mapping
        /// </summary>
        /// <remarks>
        /// {0} : product review ID
        /// </remarks>
        public static CacheKey ProductReviewReviewTypeMappingAllCacheKey => new CacheKey("TVProgViewer.productReviewReviewTypeMapping.all-{0}", ProductReviewReviewTypeMappingAllPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ProductReviewReviewTypeMappingAllPrefixCacheKey => "TVProgViewer.productReviewReviewTypeMapping.all";

        #endregion

        #region Specification attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// {1} : specification attribute option ID
        /// {2} : allow filtering
        /// {3} : show on product page
        /// </remarks>
        public static CacheKey ProductSpecificationAttributeAllByProductIdCacheKey => new CacheKey("TVProgViewer.productspecificationattribute.allbyproductid-{0}-{1}-{2}-{3}", ProductSpecificationAttributeAllByProductPrefixCacheKey);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        public static string ProductSpecificationAttributeAllByProductPrefixCacheKey => "TVProgViewer.TVProgViewer.productspecificationattribute.allbyproductid-{0}";
        
        /// <summary>
        /// Key for specification attributes caching (product details page)
        /// </summary>
        public static CacheKey SpecAttributesWithOptionsCacheKey => new CacheKey("TVProgViewer.productspecificationattribute.with.options");

        /// <summary>
        /// Key for specification attributes caching
        /// </summary>
        public static CacheKey SpecAttributesAllCacheKey => new CacheKey("TVProgViewer.productspecificationattribute.all");

        /// <summary>
        /// Key for specification attributes caching
        /// <remarks>
        /// {0} : specification attribute ID
        /// </remarks>
        /// </summary>
        public static CacheKey SpecAttributesOptionsCacheKey => new CacheKey("TVProgViewer.productspecificationattribute.options-{0}");
        
        #endregion

        #region Category template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey CategoryTemplatesAllCacheKey => new CacheKey("TVProgViewer.categorytemplate.all");

        #endregion

        #region Manufacturer template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ManufacturerTemplatesAllCacheKey => new CacheKey("TVProgViewer.manufacturertemplate.all");

        #endregion

        #region Product template

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static CacheKey ProductTemplatesAllCacheKey => new CacheKey("TVProgViewer.producttemplates.all");

        #endregion
    }
}