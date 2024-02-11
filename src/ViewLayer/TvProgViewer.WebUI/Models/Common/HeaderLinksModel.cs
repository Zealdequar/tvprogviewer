using System.Collections.Generic;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record HeaderLinksModel : BaseTvProgModel
    {
        public HeaderLinksModel() { 
            Topics = new List<TopicModel>();
        }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        
        public bool ShoppingCartEnabled { get; set; }
        public int ShoppingCartItems { get; set; }
        
        public bool WishlistEnabled { get; set; }
        public int WishlistItems { get; set; }

        public bool AllowPrivateMessages { get; set; }
        public string UnreadPrivateMessages { get; set; }
        public string AlertMessage { get; set; }
        public UserRegistrationType RegistrationType { get; set; }
        public IList<TopicModel> Topics { get; set; }
        public bool DisplayHomepageMenuItem { get; set; }
        public bool DisplayTvChannelSearchMenuItem { get; set; }
        public bool DisplayContactUsMenuItem { get; set; }

        #region Вложенные классы
        public partial record TopicModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }
        }
        #endregion
    }
}