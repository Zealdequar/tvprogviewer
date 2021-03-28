using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TVProgViewer.Core;
using TVProgViewer.TVProgUpdaterV2.Extensions;
using TVProgViewer.Core.Domain.Common;
using System.Threading.Tasks;

namespace TVProgViewer.TVProgUpdaterV2.TagHelpers.Admin
{
    /// <summary>
    /// TVProgViewer-nested-setting tag helper
    /// </summary>
    [HtmlTargetElement("tvprog-nested-setting", Attributes = FOR_ATTRIBUTE_NAME)]
    public class TvProgNestedSettingTagHelper : TagHelper
    {
        #region Constants

        private const string FOR_ATTRIBUTE_NAME = "asp-for";

        #endregion

        #region Properties

        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion

        #region Ctor

        public TvProgNestedSettingTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously executes the tag helper with the given context and output
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var parentSettingName = For.Name;

            var random = CommonHelper.GenerateRandomInteger();
            var nestedSettingId = $"nestedSetting{random}";
            var parentSettingId = $"parentSetting{random}";

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "nested-setting");

            if (context.AllAttributes.ContainsName("id"))
                nestedSettingId = context.AllAttributes["id"].Value.ToString();
            output.Attributes.Add("id", nestedSettingId);

            //use javascript
            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml(
                "$(document).ready(function () {" +
                    $"initNestedSetting('{parentSettingName}', '{parentSettingId}', '{nestedSettingId}');" +
                "});");
            var scriptTag = await script.RenderHtmlContentAsync();
            output.PreContent.SetHtmlContent(scriptTag);
        }

        #endregion
    }
}