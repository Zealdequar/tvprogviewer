using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVProgViewer.TVProgApp.Controllers
{
    internal class BaseController
    {
        internal static TvProgServiceReference.MainServiceClient TvProgService
        {
            get
            {
                return new TvProgServiceReference.MainServiceClient();
            }
        }
    }
}
