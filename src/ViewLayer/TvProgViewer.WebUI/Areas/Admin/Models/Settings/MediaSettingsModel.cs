using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a media settings model
    /// </summary>
    public partial record MediaSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.PicturesStoredIntoDatabase")]
        public bool PicturesStoredIntoDatabase { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.AvatarPictureSize")]
        public int AvatarPictureSize { get; set; }
        public bool AvatarPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.TvChannelThumbPictureSize")]
        public int TvChannelThumbPictureSize { get; set; }
        public bool TvChannelThumbPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.TvChannelDetailsPictureSize")]
        public int TvChannelDetailsPictureSize { get; set; }
        public bool TvChannelDetailsPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.TvChannelThumbPictureSizeOnTvChannelDetailsPage")]
        public int TvChannelThumbPictureSizeOnTvChannelDetailsPage { get; set; }
        public bool TvChannelThumbPictureSizeOnTvChannelDetailsPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.AssociatedTvChannelPictureSize")]
        public int AssociatedTvChannelPictureSize { get; set; }
        public bool AssociatedTvChannelPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.TvChannelDefaultImage")]
        [UIHint("Picture")]
        public int TvChannelDefaultImageId { get; set; }
        public bool TvChannelDefaultImageId_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.CategoryThumbPictureSize")]
        public int CategoryThumbPictureSize { get; set; }
        public bool CategoryThumbPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.ManufacturerThumbPictureSize")]
        public int ManufacturerThumbPictureSize { get; set; }
        public bool ManufacturerThumbPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.VendorThumbPictureSize")]
        public int VendorThumbPictureSize { get; set; }
        public bool VendorThumbPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.CartThumbPictureSize")]
        public int CartThumbPictureSize { get; set; }
        public bool CartThumbPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.OrderThumbPictureSize")]
        public int OrderThumbPictureSize { get; set; }
        public bool OrderThumbPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.MiniCartThumbPictureSize")]
        public int MiniCartThumbPictureSize { get; set; }
        public bool MiniCartThumbPictureSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.MaximumImageSize")]
        public int MaximumImageSize { get; set; }
        public bool MaximumImageSize_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.MultipleThumbDirectories")]
        public bool MultipleThumbDirectories { get; set; }
        public bool MultipleThumbDirectories_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.DefaultImageQuality")]
        public int DefaultImageQuality { get; set; }
        public bool DefaultImageQuality_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.ImportTvChannelImagesUsingHash")]
        public bool ImportTvChannelImagesUsingHash { get; set; }
        public bool ImportTvChannelImagesUsingHash_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.DefaultPictureZoomEnabled")]
        public bool DefaultPictureZoomEnabled { get; set; }
        public bool DefaultPictureZoomEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Media.AllowSVGUploads")]
        public bool AllowSVGUploads { get; set; }
        public bool AllowSVGUploads_OverrideForStore { get; set; }

        #endregion
    }
}