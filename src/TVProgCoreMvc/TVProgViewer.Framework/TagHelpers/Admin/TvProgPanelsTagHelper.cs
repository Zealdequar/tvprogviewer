using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TVProgViewer.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// TVProgViewer-panel tag helper
    /// </summary>
    [HtmlTargetElement("tvprog-panels", Attributes = ID_ATTRIBUTE_NAME)]
    public class TvProgPanelsTagHelper : TagHelper
    {
        private const string ID_ATTRIBUTE_NAME = "id";

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }
}