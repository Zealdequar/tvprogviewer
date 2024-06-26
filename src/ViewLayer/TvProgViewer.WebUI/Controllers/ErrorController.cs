﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TvProgViewer.WebUI.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class ErrorController : Controller
    {
        public virtual IActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return File("ErrorPage.htm", "text/html");
        }
    }
}