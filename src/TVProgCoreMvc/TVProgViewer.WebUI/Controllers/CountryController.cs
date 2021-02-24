using Microsoft.AspNetCore.Mvc;
using TVProgViewer.WebUI.Factories;
using TVProgViewer.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Controllers
{
    public partial class CountryController : BasePublicController
	{
        #region Fields

        private readonly ICountryModelFactory _countryModelFactory;

        #endregion

        #region Ctor

        public CountryController(ICountryModelFactory countryModelFactory)
        {
            _countryModelFactory = countryModelFactory;
        }

        #endregion

        #region States / provinces

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> GetStatesByCountryId(string countryId, bool addSelectStateItem)
        {
            var model = await _countryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);

            return Json(model);
        }

        #endregion
    }
}