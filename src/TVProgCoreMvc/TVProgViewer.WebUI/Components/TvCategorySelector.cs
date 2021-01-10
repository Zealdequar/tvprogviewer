using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVProgViewer.Web.Framework.Components;
using TVProgViewer.WebUI.Factories;

namespace TVProgViewer.WebUI.Components
{
    public class TvCategorySelectorViewComponent: TvProgViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public TvCategorySelectorViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareTvCategorySelectorModel();
            return View(model);
        }
    }
}
