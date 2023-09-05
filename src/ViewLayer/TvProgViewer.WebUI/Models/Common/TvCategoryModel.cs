using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public record TvCategoryModel: BaseTvProgEntityModel
    {
        public string Name { get; set; }
    }
}
