﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Media;

namespace TvProgViewer.WebUI.Models.ShoppingCart
{
    public partial record WishlistModel : BaseTvProgModel
    {
        public WishlistModel()
        {
            Items = new List<ShoppingCartItemModel>();
            Warnings = new List<string>();
        }

        public Guid UserGuid { get; set; }
        public string UserFullname { get; set; }

        public bool EmailWishlistEnabled { get; set; }

        public bool ShowSku { get; set; }

        public bool ShowProductImages { get; set; }

        public bool IsEditable { get; set; }

        public bool DisplayAddToCart { get; set; }

        public bool DisplayTaxShippingInfo { get; set; }

        public IList<ShoppingCartItemModel> Items { get; set; }

        public IList<string> Warnings { get; set; }
        
		#region Nested Classes

        public partial record ShoppingCartItemModel : BaseTvProgEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
                AllowedQuantities = new List<SelectListItem>();
                Warnings = new List<string>();
            }

            public string Sku { get; set; }

            public PictureModel Picture {get;set;}

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }
            public decimal UnitPriceValue { get; set; }

            public string SubTotal { get; set; }
            public decimal SubTotalValue { get; set; }

            public string Discount { get; set; }
            public decimal DiscountValue { get; set; }
            public int? MaximumDiscountedQty { get; set; }

            public int Quantity { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }

            public string AttributeInfo { get; set; }

            public string RecurringInfo { get; set; }

            public string RentalInfo { get; set; }

            public bool AllowItemEditing { get; set; }

            public IList<string> Warnings { get; set; }
        }

		#endregion
    }
}