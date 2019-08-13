using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TVProgViewer.WebUI.Controllers
{
    public class StaticContentController : Controller
    {
        [HandleError]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View("PageNotFound");
        }

        public ActionResult PageNotFound()
        {
            return NotFound();
        }
    }
}