using FluentValidation;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Validators;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.WebUI.Validators.Catalog
{
    public partial class TvChannelEmailAFriendValidator : BaseTvProgValidator<TvChannelEmailAFriendModel>
    {
        public TvChannelEmailAFriendValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.FriendEmail).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("TvChannels.EmailAFriend.FriendEmail.Required"));
            RuleFor(x => x.FriendEmail).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));

            RuleFor(x => x.YourEmailAddress).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("TvChannels.EmailAFriend.YourEmailAddress.Required"));
            RuleFor(x => x.YourEmailAddress).EmailAddress().WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
        }}
}