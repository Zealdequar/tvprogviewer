using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record TvChannelEmailAFriendModel : BaseTvProgModel
    {
        public int TvChannelId { get; set; }

        public string TvChannelName { get; set; }

        public string TvChannelSeName { get; set; }

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("TvChannels.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [DataType(DataType.EmailAddress)]
        [TvProgResourceDisplayName("TvChannels.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [TvProgResourceDisplayName("TvChannels.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}