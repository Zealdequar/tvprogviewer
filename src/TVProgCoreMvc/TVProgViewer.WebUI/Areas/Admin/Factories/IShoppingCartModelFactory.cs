using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.WebUI.Areas.Admin.Models.ShoppingCart;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory
    /// </summary>
    public partial interface IShoppingCartModelFactory
    {
        /// <summary>
        /// Prepare shopping cart search model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>Shopping cart search model</returns>
        Task<ShoppingCartSearchModel> PrepareShoppingCartSearchModelAsync(ShoppingCartSearchModel searchModel);

        /// <summary>
        /// Prepare paged shopping cart list model
        /// </summary>
        /// <param name="searchModel">Shopping cart search model</param>
        /// <returns>Shopping cart list model</returns>
        Task<ShoppingCartListModel> PrepareShoppingCartListModelAsync(ShoppingCartSearchModel searchModel);

        /// <summary>
        /// Prepare paged shopping cart item list model
        /// </summary>
        /// <param name="searchModel">Shopping cart item search model</param>
        /// <param name="user">User</param>
        /// <returns>Shopping cart item list model</returns>
        Task<ShoppingCartItemListModel> PrepareShoppingCartItemListModelAsync(ShoppingCartItemSearchModel searchModel, User user);
    }
}