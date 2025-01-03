﻿using System;
using System.Collections.Generic;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Media;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record TvChannelOverviewModel : BaseTvProgEntityModel
    {
        public TvChannelOverviewModel()
        {
            TvChannelPrice = new TvChannelPriceModel();
            PictureModels = new List<PictureModel>();
            TvChannelSpecificationModel = new TvChannelSpecificationModel();
            ReviewOverviewModel = new TvChannelReviewOverviewModel();
        }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }

        public string Sku { get; set; }

        public TvChannelType TvChannelType { get; set; }

        public bool MarkAsNew { get; set; }

        //price
        public TvChannelPriceModel TvChannelPrice { get; set; }
        //pictures
        public IList<PictureModel> PictureModels { get; set; }
        //specification attributes
        public TvChannelSpecificationModel TvChannelSpecificationModel { get; set; }
        //price
        public TvChannelReviewOverviewModel ReviewOverviewModel { get; set; }

		#region Nested Classes

        public partial record TvChannelPriceModel : BaseTvProgModel
        {
            public string OldPrice { get; set; }
            public decimal? OldPriceValue { get; set; }
            public string Price { get; set; }
            public decimal? PriceValue { get; set; }
            /// <summary>
            /// PAngV baseprice (used in Germany)
            /// </summary>
            public string BasePricePAngV { get; set; }
            public decimal? BasePricePAngVValue { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }
            public bool DisableAddToCompareListButton { get; set; }

            public bool AvailableForPreOrder { get; set; }
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

            public bool IsRental { get; set; }

            public bool ForceRedirectionAfterAddingToCart { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
        }

		#endregion
    }
}