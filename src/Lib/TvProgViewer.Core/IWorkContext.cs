using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Domain.TvProgMain;

namespace TvProgViewer.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets the current user
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<User> GetCurrentUserAsync();

        /// <summary>
        /// Sets the current user
        /// </summary>
        /// <param name="user">Current user</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetCurrentUserAsync(User user = null);

        /// <summary>
        /// Gets the original user (in case the current one is impersonated)
        /// </summary>
        User OriginalUserIfImpersonated { get; }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<Vendor> GetCurrentVendorAsync();

        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<Language> GetWorkingLanguageAsync();

        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetWorkingLanguageAsync(Language language);

        
        /// <summary>
        /// Gets or sets current user working currency
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<Currency> GetWorkingCurrencyAsync();

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        Task SetWorkingCurrencyAsync(Currency currency);

        /// <summary>
        /// Получение пользовательского ТВ-провайдера
        /// </summary>
        Task<TvProgProviders> GetWorkingProviderAsync();

        /// <summary>
        /// Установка пользовательского ТВ-провайдера
        /// </summary>
        /// <param name="tvProgProviders">ТВ-провайдер</param>
        /// <returns></returns>
        Task SetWorkingProviderAsync(TvProgProviders tvProgProviders);

        /// <summary>
        /// Получение пользовательского типа ТВ-программы
        /// </summary>
        Task<TypeProg> GetWorkingTypeProgAsync();

        /// <summary>
        /// Sets current user working currency
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>Задача представляет асинхронную операцию</returns>

        /// <summary>
        /// Установка пользователького типа ТВ-программы 
        /// </summary>
        /// <param name="typeProg">Тип ТВ-программы</param>
        Task SetWorkingTypeProgAsync(TypeProg typeProg);

        /// <summary>
        /// Получение пользовательской категории ТВ-программы
        /// </summary>
        Task<string> GetWorkingCategoryAsync();

        /// <summary>
        /// Установка пользовательской категории ТВ-программы 
        /// </summary>
        /// <param name="category">Категория</param>
        Task SetWorkingCategoryAsync(string category);

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<TaxDisplayType> GetTaxDisplayTypeAsync();

        /// <summary>
        /// Sets current tax display type
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType);        
    }
}
