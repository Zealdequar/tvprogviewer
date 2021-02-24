using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record HeaderLinksModel : BaseTvProgModel
    {
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
    }
}