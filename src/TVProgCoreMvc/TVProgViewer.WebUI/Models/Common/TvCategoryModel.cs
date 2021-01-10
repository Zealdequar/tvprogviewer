using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public record TvCategoryModel: BaseTvProgEntityModel
    {
        public string Name { get; set; }
    }
}
