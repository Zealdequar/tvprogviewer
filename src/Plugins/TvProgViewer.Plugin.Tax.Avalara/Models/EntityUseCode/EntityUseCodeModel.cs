using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.EntityUseCode
{
    /// <summary>
    /// Represents an entity use code model
    /// </summary>
    public record EntityUseCodeModel : BaseTvProgEntityModel
    {
        #region Ctor

        public EntityUseCodeModel()
        {
            EntityUseCodes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public string PrecedingElementId { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.EntityUseCode")]
        public string AvalaraEntityUseCode { get; set; }
        public List<SelectListItem> EntityUseCodes { get; set; }

        #endregion
    }
}