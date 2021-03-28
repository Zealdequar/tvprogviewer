
using System.Threading.Tasks;

namespace TVProgViewer.TVProgUpdaterV2.Themes
{
    /// <summary>
    /// Represents a theme context
    /// </summary>
    public interface IThemeContext
    {
        /// <summary>
        /// Get current theme system name
        /// </summary>
        Task<string> GetWorkingThemeNameAsync();

        /// <summary>
        /// Set current theme system name
        /// </summary>
        Task SetWorkingThemeNameAsync(string workingThemeName);
    }
}
