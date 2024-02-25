using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TvProgViewer.Core;
using TvProgViewer.Core.Infrastructure;

namespace TvProgViewer.WebUI.Areas.Admin.Helpers
{
    /// <summary>
    /// TinyMCE helper
    /// </summary>
    public partial class TinyMceHelper : ITinyMceHelper
    {
        private readonly ITvProgFileProvider _nopFileProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWorkContext _workContext;

        public TinyMceHelper(ITvProgFileProvider nopFileProvider, IWebHostEnvironment webHostEnvironment, IWorkContext workContext)
        {
            _nopFileProvider = nopFileProvider;
            _webHostEnvironment = webHostEnvironment;
            _workContext = workContext;
        }

    /// <summary>
    /// Get tinyMCE language name for current language 
    /// </summary>
    /// <returns>
    /// Задача представляет асинхронную операцию
    /// The task result contains the inyMCE language name
    /// </returns>
    public async Task<string> GetTinyMceLanguageAsync()
    {
        //tvProgViewer supports TinyMCE's localization for 10 languages:
        //Chinese, Spanish, Arabic, Portuguese, Russian, German, French, Italian, Dutch and English out-of-the-box.
        //Additional languages can be downloaded from the website TinyMCE(https://www.tinymce.com/download/language-packages/)
        
        var languageCulture = (await _workContext.GetWorkingLanguageAsync()).LanguageCulture;

        var langFile = $"{languageCulture}.js";
        var directoryPath = _nopFileProvider.Combine(_webHostEnvironment.WebRootPath, @"lib_npm\tinymce\langs");
        var fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");

        if (!fileExists)
        {
            languageCulture = languageCulture.Replace('-', '_');
            langFile = $"{languageCulture}.js";
            fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");
        }

        if (!fileExists)
        {
            languageCulture = languageCulture.Split('_', '-')[0];
            langFile = $"{languageCulture}.js";
            fileExists = _nopFileProvider.FileExists($"{directoryPath}\\{langFile}");
        }

        return fileExists ? languageCulture : string.Empty;
    }
    }
}