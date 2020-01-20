using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVProgViewer.WebUI.Concrete
{
    public class BaseEFRepository
    {
        /// <summary>
        /// Провайдер службы TvProgService
        /// </summary>
        internal static TVProgService.MainServiceClient TvProgService
        {
            get
            {
                return new TVProgService.MainServiceClient();
            }
        }
    }
}