
using System.Threading.Tasks;

namespace TvProgViewer.Web.Framework.Themes
{
    /// <summary>
    /// Represents a theme context
    /// </summary>
    public partial interface IThemeContext
    {
        /// <summary>
        /// Get current theme system name
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<string> GetWorkingThemeNameAsync();

        /// <summary>
        /// Set current theme system name
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetWorkingThemeNameAsync(string workingThemeName);
    }
}
