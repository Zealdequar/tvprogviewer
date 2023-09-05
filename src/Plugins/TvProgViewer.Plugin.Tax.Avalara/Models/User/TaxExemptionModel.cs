using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.User
{
    /// <summary>
    /// Represents a tax exemption model
    /// </summary>
    public record TaxExemptionModel : BaseTvProgModel
    {
        #region Ctor

        public TaxExemptionModel()
        {
            Certificates = new List<ExemptionCertificateModel>();
            AvailableExposureZones = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public string Token { get; set; }

        public string Link { get; set; }

        public int UserId { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.ExemptionCertificates.Add.ExposureZone")]
        public int ExposureZone { get; set; }

        public IList<ExemptionCertificateModel> Certificates { get; set; }

        public IList<SelectListItem> AvailableExposureZones { get; set; }

        #endregion
    }
}