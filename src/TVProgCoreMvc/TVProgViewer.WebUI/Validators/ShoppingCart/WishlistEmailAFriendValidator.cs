using FluentValidation;
using TVProgViewer.Services.Localization;
using TVProgViewer.Web.Framework.Validators;
using TVProgViewer.WebUI.Models.ShoppingCart;

namespace TVProgViewer.WebUI.Validators.ShoppingCart
{
    public partial class WishlistEmailAFriendValidator : BaseTvProgValidator<WishlistEmailAFriendModel>
    {
        public WishlistEmailAFriendValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendEmail).NotEmpty().WithMessage(localizationService.GetResource("Wishlist.EmailAFriend.FriendEmail.Required"));
            RuleFor(x => x.FriendEmail).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));

            RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessage(localizationService.GetResource("Wishlist.EmailAFriend.YourEmailAddress.Required"));
            RuleFor(x => x.YourEmailAddress).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}