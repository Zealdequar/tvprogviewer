using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Tax;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Core.Domain.TvProgMain;
using System.Threading.Tasks;

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
        Task<User> GetCurrentUserAsync();

        /// <summary>
        /// Установка текущего пользователя
        /// </summary>
        /// <param name="user">Текущий пользователь</param>
        /// <returns></returns>
        Task SetCurrentUserAsync(User user = null);


        /// <summary>
        /// Gets the original User (in case the current one is impersonated)
        /// </summary>
        User OriginalUserIfImpersonated { get; }

        /// <summary>
        /// Gets the current vendor (logged-in manager)
        /// </summary>
        Task<Vendor> GetCurrentVendorAsync();

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        Task<Language> GetWorkingLanguageAsync();

        /// <summary>
        /// Установка рабочего языка
        /// </summary>
        /// <param name="language">Язык</param>
        /// <returns></returns>
        Task SetWorkingLanguageAsync(Language language);

        /// <summary>
        /// Для удаления
        /// </summary>
        Task<Currency> GetWorkingCurrencyAsync();
        
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
        Task<TaxDisplayType> GetTaxDisplayTypeAsync();

        /// <summary>
        /// Sets current tax display type
        /// </summary>
        Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType);
    }
}
