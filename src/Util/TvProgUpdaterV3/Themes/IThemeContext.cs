﻿
using System.Threading.Tasks;

namespace TvProgViewer.TvProgUpdaterV3.Themes
{
    /// <summary>
    /// Represents a theme context
    /// </summary>
    public partial interface IThemeContext
    {
        /// <summary>
        /// Get current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<string> GetWorkingThemeNameAsync();

        /// <summary>
        /// Set current theme system name
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetWorkingThemeNameAsync(string workingThemeName);
    }
}
