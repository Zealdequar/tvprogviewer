﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Services.Directory;
using TvProgViewer.WebUI.Controllers;

namespace TvProgViewer.Plugin.Widgets.What3words.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class What3wordsCountryController : BasePublicController
    {
        #region Fields

        private readonly ICountryService _countryService;

        #endregion

        #region Ctor

        public What3wordsCountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> GetAlpha2byCountryName(int countryId)
        {
            var result = (await _countryService.GetCountryByIdAsync(countryId))?.TwoLetterIsoCode;
            return Json(result);
        }

        #endregion
    }
}
