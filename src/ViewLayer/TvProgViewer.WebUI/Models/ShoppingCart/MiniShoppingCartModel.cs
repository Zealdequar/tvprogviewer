using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Media;

namespace TvProgViewer.WebUI.Models.ShoppingCart
{
    public partial record MiniShoppingCartModel : BaseTvProgModel
    {
        public MiniShoppingCartModel()
        {
            Items = new List<ShoppingCartItemModel>();
        }

        public IList<ShoppingCartItemModel> Items { get; set; }
        public int TotalTvChannels { get; set; }
        public string SubTotal { get; set; }
        public decimal SubTotalValue { get; set; }
        public bool DisplayShoppingCartButton { get; set; }
        public bool DisplayCheckoutButton { get; set; }
        public bool CurrentUserIsGuest { get; set; }
        public bool AnonymousCheckoutAllowed { get; set; }
        public bool ShowTvChannelImages { get; set; }

        #region Nested Classes

        public partial record ShoppingCartItemModel : BaseTvProgEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
            }

            public int TvChannelId { get; set; }

            public string TvChannelName { get; set; }

            public string TvChannelSeName { get; set; }

            public int Quantity { get; set; }

            public string UnitPrice { get; set; }
            public decimal UnitPriceValue { get; set; }

            public string AttributeInfo { get; set; }

            public PictureModel Picture { get; set; }
        }

        #endregion
    }
}