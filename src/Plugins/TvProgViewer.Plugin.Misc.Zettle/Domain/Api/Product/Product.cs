using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain.Api.TvChannel
{
    /// <summary>
    /// Represents the tvChannel details
    /// </summary>
    public class TvChannel : ApiResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the ETag
        /// </summary>
        [JsonProperty(PropertyName = "etag")]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the categories
        /// </summary>
        [JsonProperty(PropertyName = "categories")]
        public List<string> Categories { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        [JsonIgnore]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the external reference
        /// </summary>
        [JsonProperty(PropertyName = "externalReference")]
        public string ExternalReference { get; set; }

        /// <summary>
        /// Gets or sets the unit name
        /// </summary>
        [JsonProperty(PropertyName = "unitName")]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the VAT percentage
        /// </summary>
        [JsonProperty(PropertyName = "vatPercentage")]
        public decimal? VatPercentage { get; set; }

        /// <summary>
        /// Gets or sets the tax code
        /// </summary>
        [JsonProperty(PropertyName = "taxCode")]
        public string TaxCode { get; set; }

        /// <summary>
        /// Gets or sets the tax rates
        /// </summary>
        [JsonProperty(PropertyName = "taxRates")]
        public List<string> TaxRates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvChannel is tax exempt
        /// </summary>
        [JsonProperty(PropertyName = "taxExempt")]
        public bool? TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvChannel is create with default tax
        /// </summary>
        [JsonProperty(PropertyName = "createWithDefaultTax")]
        public bool? CreateWithDefaultTax { get; set; }

        /// <summary>
        /// Gets or sets the image lookup keys
        /// </summary>
        [JsonProperty(PropertyName = "imageLookupKeys")]
        public List<string> ImageLookupKeys { get; set; }

        /// <summary>
        /// Gets or sets the presentation
        /// </summary>
        [JsonProperty(PropertyName = "presentation")]
        public TvChannelPresentation Presentation { get; set; }

        /// <summary>
        /// Gets or sets the variants
        /// </summary>
        [JsonProperty(PropertyName = "variants")]
        public List<TvChannelVariant> Variants { get; set; }

        /// <summary>
        /// Gets or sets the online tvChannel info
        /// </summary>
        [JsonProperty(PropertyName = "online")]
        public TvChannelOnlineInfo Online { get; set; }

        /// <summary>
        /// Gets or sets the variant option definitions
        /// </summary>
        [JsonProperty(PropertyName = "variantOptionDefinitions")]
        public TvChannelVariantDefinitions VariantOptionDefinitions { get; set; }

        /// <summary>
        /// Gets or sets the category
        /// </summary>
        [JsonProperty(PropertyName = "category")]
        public TvChannelCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the metadata
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public TvChannelMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1 of a user who updated the tvChannel
        /// </summary>
        [JsonProperty(PropertyName = "updatedBy")]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the updated date
        /// </summary>
        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; set; }

        #endregion

        #region Nested classes

        /// <summary>
        /// Represents the tvChannel category details
        /// </summary>
        public class TvChannelCategory
        {
            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "uuid")]
            public string Uuid { get; set; }

            /// <summary>
            /// Gets or sets the name
            /// </summary>
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
        }

        /// <summary>
        /// Represents the tvChannel presentation details
        /// </summary>
        public class TvChannelPresentation
        {
            /// <summary>
            /// Gets or sets the image URL
            /// </summary>
            [JsonProperty(PropertyName = "imageUrl")]
            public string ImageUrl { get; set; }

            /// <summary>
            /// Gets or sets the background color
            /// </summary>
            [JsonProperty(PropertyName = "backgroundColor")]
            public string BackgroundColor { get; set; }

            /// <summary>
            /// Gets or sets the text color
            /// </summary>
            [JsonProperty(PropertyName = "textColor")]
            public string TextColor { get; set; }
        }

        /// <summary>
        /// Represents the tvChannel variant details
        /// </summary>
        public class TvChannelVariant
        {
            #region Properties

            /// <summary>
            /// Gets or sets the unique identifier as UUID version 1
            /// </summary>
            [JsonProperty(PropertyName = "uuid")]
            public string Uuid { get; set; }

            /// <summary>
            /// Gets or sets the name
            /// </summary>
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the description
            /// </summary>
            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the SKU
            /// </summary>
            [JsonProperty(PropertyName = "sku")]
            public string Sku { get; set; }

            /// <summary>
            /// Gets or sets the barcode
            /// </summary>
            [JsonProperty(PropertyName = "barcode")]
            public string Barcode { get; set; }

            /// <summary>
            /// Gets or sets the VAT percentage
            /// </summary>
            [JsonProperty(PropertyName = "vatPercentage")]
            public decimal? VatPercentage { get; set; }

            /// <summary>
            /// Gets or sets the price
            /// </summary>
            [JsonProperty(PropertyName = "price")]
            public TvChannelPrice Price { get; set; }

            /// <summary>
            /// Gets or sets the cost price
            /// </summary>
            [JsonProperty(PropertyName = "costPrice")]
            public TvChannelPrice CostPrice { get; set; }

            /// <summary>
            /// Gets or sets the options
            /// </summary>
            [JsonProperty(PropertyName = "options")]
            public List<TvChannelVariantOption> Options { get; set; }

            /// <summary>
            /// Gets or sets the presentation
            /// </summary>
            [JsonProperty(PropertyName = "presentation")]
            public TvChannelPresentation Presentation { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the tvChannel price details
            /// </summary>
            public class TvChannelPrice
            {
                /// <summary>
                /// Gets or sets the amount
                /// </summary>
                [JsonProperty(PropertyName = "amount")]
                public int? Amount { get; set; }

                /// <summary>
                /// Gets or sets the currency id
                /// </summary>
                [JsonProperty(PropertyName = "currencyId")]
                public string CurrencyId { get; set; }
            }

            /// <summary>
            /// Represents the tvChannel variant option property details
            /// </summary>
            public class TvChannelVariantOption
            {
                /// <summary>
                /// Gets or sets the name
                /// </summary>
                [JsonProperty(PropertyName = "name")]
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the value
                /// </summary>
                [JsonProperty(PropertyName = "value")]
                public string Value { get; set; }
            }

            #endregion
        }

        /// <summary>
        /// Represents the tvChannel variant options details
        /// </summary>
        public class TvChannelVariantDefinitions
        {
            #region Properties

            /// <summary>
            /// Gets or sets the tvChannel variant options
            /// </summary>
            [JsonProperty(PropertyName = "definitions")]
            public List<TvChannelVariantOptionDefinition> Definitions { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the tvChannel variant option details
            /// </summary>
            public class TvChannelVariantOptionDefinition
            {
                #region Properties

                /// <summary>
                /// Gets or sets the name
                /// </summary>
                [JsonProperty(PropertyName = "name")]
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the tvChannel variant option properties
                /// </summary>
                [JsonProperty(PropertyName = "properties")]
                public List<TvChannelVariantOptionProperty> Properties { get; set; }

                #endregion

                #region Nested classes

                /// <summary>
                /// Represents the tvChannel variant option property details
                /// </summary>
                public class TvChannelVariantOptionProperty
                {
                    /// <summary>
                    /// Gets or sets the value
                    /// </summary>
                    [JsonProperty(PropertyName = "value")]
                    public string Value { get; set; }

                    /// <summary>
                    /// Gets or sets the image URL
                    /// </summary>
                    [JsonProperty(PropertyName = "imageUrl")]
                    public string ImageUrl { get; set; }
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// Represents the tvChannel online info details
        /// </summary>
        public class TvChannelOnlineInfo
        {
            #region Properties

            /// <summary>
            /// Gets or sets the status
            /// </summary>
            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            /// <summary>
            /// Gets or sets the title
            /// </summary>
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }

            /// <summary>
            /// Gets or sets the description
            /// </summary>
            [JsonProperty(PropertyName = "description")]
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the shipping details
            /// </summary>
            [JsonProperty(PropertyName = "shipping")]
            public TvChannelShippingInfo Shipping { get; set; }

            /// <summary>
            /// Gets or sets the presentation
            /// </summary>
            [JsonProperty(PropertyName = "presentation")]
            public TvChannelOnlinePresentation Presentation { get; set; }

            /// <summary>
            /// Gets or sets the SEO details
            /// </summary>
            [JsonProperty(PropertyName = "seo")]
            public TvChannelSeo Seo { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the tvChannel shipping details
            /// </summary>
            public class TvChannelShippingInfo
            {
                #region Properties

                /// <summary>
                /// Gets or sets the shipping pricing model
                /// </summary>
                [JsonProperty(PropertyName = "shippingPricingModel")]
                public string ShippingPricingModel { get; set; }

                /// <summary>
                /// Gets or sets the weight (in grams)
                /// </summary>
                [JsonProperty(PropertyName = "weightInGrams")]
                public int? WeightInGrams { get; set; }

                /// <summary>
                /// Gets or sets the weight info
                /// </summary>
                [JsonProperty(PropertyName = "weight")]
                public TvChannelWeight Weight { get; set; }

                #endregion

                #region Nested classes

                /// <summary>
                /// Represents the tvChannel weight details
                /// </summary>
                public class TvChannelWeight
                {
                    /// <summary>
                    /// Gets or sets the weight
                    /// </summary>
                    [JsonProperty(PropertyName = "weight")]
                    public decimal? Weight { get; set; }

                    /// <summary>
                    /// Gets or sets the unit
                    /// </summary>
                    [JsonProperty(PropertyName = "unit")]
                    public string Unit { get; set; }
                }

                #endregion
            }

            /// <summary>
            /// Represents the tvChannel online presentation details
            /// </summary>
            public class TvChannelOnlinePresentation
            {
                /// <summary>
                /// Gets or sets the display image URL
                /// </summary>
                [JsonProperty(PropertyName = "displayImageUrl")]
                public string DisplayImageUrl { get; set; }

                /// <summary>
                /// Gets or sets the additional image URLs
                /// </summary>
                [JsonProperty(PropertyName = "additionalImageUrls")]
                public List<string> AdditionalImageUrls { get; set; }

                /// <summary>
                /// Gets or sets the media URLs
                /// </summary>
                [JsonProperty(PropertyName = "mediaUrls")]
                public List<string> MediaUrls { get; set; }
            }

            /// <summary>
            /// Represents the tvChannel SEO details
            /// </summary>
            public class TvChannelSeo
            {
                /// <summary>
                /// Gets or sets the title
                /// </summary>
                [JsonProperty(PropertyName = "title")]
                public string Title { get; set; }

                /// <summary>
                /// Gets or sets the meta description
                /// </summary>
                [JsonProperty(PropertyName = "metaDescription")]
                public string MetaDescription { get; set; }

                /// <summary>
                /// Gets or sets the slug
                /// </summary>
                [JsonProperty(PropertyName = "slug")]
                public string Slug { get; set; }
            }

            #endregion
        }

        /// <summary>
        /// Represents the tvChannel metadata details
        /// </summary>
        public class TvChannelMetadata
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether the tvChannel is in POS
            /// </summary>
            [JsonProperty(PropertyName = "inPos")]
            public bool? InPos { get; set; }

            /// <summary>
            /// Gets or sets the source
            /// </summary>
            [JsonProperty(PropertyName = "source")]
            public TvChannelSource Source { get; set; }

            #endregion

            #region Nested classes

            /// <summary>
            /// Represents the tvChannel source details
            /// </summary>
            public class TvChannelSource
            {
                /// <summary>
                /// Gets or sets the name
                /// </summary>
                [JsonProperty(PropertyName = "name")]
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets a value indicating whether the source of tvChannel is external
                /// </summary>
                [JsonProperty(PropertyName = "external")]
                public bool? External { get; set; }
            }

            #endregion
        }

        #endregion
    }
}