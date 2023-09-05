using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TvProgViewer.Web.Framework.TagHelpers.Admin
{
    /// <summary>
    /// "tvprog-cards" tag helper
    /// </summary>
    [HtmlTargetElement("tvprog-cards", Attributes = ID_ATTRIBUTE_NAME)]
    public partial class TvProgCardsTagHelper : TagHelper
    {
        #region Constants

        private const string ID_ATTRIBUTE_NAME = "id";

        #endregion

        #region Properties

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion
    }
}