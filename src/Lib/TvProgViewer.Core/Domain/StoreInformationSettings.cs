using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Core.Domain
{
    /// <summary>
    /// Store information settings
    /// </summary>
    public partial class StoreInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether "powered by tvProgViewer" text should be displayed.
        /// Please find more info at https://tvprogviewer.ru/tvprogviewer-copyright-removal-key
        /// </summary>
        public bool HidePoweredByNopCommerce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether store is closed
        /// </summary>
        public bool StoreClosed { get; set; }

        /// <summary>
        /// Gets or sets a picture identifier of the logo. If 0, then the default one will be used
        /// </summary>
        public int LogoPictureId { get; set; }

        /// <summary>
        /// Gets or sets a default store theme
        /// </summary>
        public string DefaultStoreTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to select a theme
        /// </summary>
        public bool AllowUserToSelectTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we should display warnings about the new EU cookie law
        /// </summary>
        public bool DisplayEuCookieLawWarning { get; set; }

        /// <summary>
        /// Gets or sets a value of Facebook page URL of the site
        /// </summary>
        public string FacebookLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Twitter page URL of the site
        /// </summary>
        public string TwitterLink { get; set; }

        /// <summary>
        /// Gets or sets a value of YouTube channel URL of the site
        /// </summary>
        public string YoutubeLink { get; set; }

        /// <summary>
        /// Gets or sets a value of Instagram account URL of the site
        /// </summary>
        public string InstagramLink { get; set; }
    }
}
