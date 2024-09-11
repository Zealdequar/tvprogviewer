using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel picture model
    /// </summary>
    public partial record TvChannelPictureModel : BaseTvProgEntityModel
    {
        #region Properties

        public int TvChannelId { get; set; }

        [UIHint("MultiPicture")]
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.Picture")]
        public int PictureId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.Picture")]
        public string PictureUrl { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.OverrideAltAttribute")]
        public string OverrideAltAttribute { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.OverrideTitleAttribute")]
        public string OverrideTitleAttribute { get; set; }

        #endregion
    }
}