using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Core.Domain.TvProgMain;

namespace TVProgViewer.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current User
        /// </summary>
        User CurrentUser { get; set; }

        /// <summary>
        /// Gets the original User (in case the current one is impersonated)
        /// </summary>
        User OriginalUserIfImpersonated { get; }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        Vendor CurrentVendor { get; }

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// Для удаления
        /// </summary>
        Currency WorkingCurrency { get; set; }

        /// <summary>
        /// Получение или установка пользовательского ТВ-провайдера
        /// </summary>
        TvProgProviders WorkingProvider { get; set; }

        /// <summary>
        /// Получение или установка пользовательского типа ТВ-программы
        /// </summary>
        TypeProg WorkingTypeProg { get; set; }

        /// <summary>
        /// Получение или установка пользовательской категории ТВ-программы
        /// </summary>
        string WorkingCategory { get; set; }

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        TaxDisplayType TaxDisplayType { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether we're in admin area
        /// </summary>
        bool IsAdmin { get; set; }
    }
}
