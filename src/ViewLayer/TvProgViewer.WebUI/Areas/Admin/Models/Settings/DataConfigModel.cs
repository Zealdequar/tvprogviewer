using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    public partial record DataConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Data.ConnectionString")]
        public string ConnectionString { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Data.DataProvider")]
        public int DataProvider { get; set; }
        public SelectList DataProviderTypeValues { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Data.SQLCommandTimeout")]
        [UIHint("Int32Nullable")]
        public int? SQLCommandTimeout { get; set; }

        #endregion
    }
}
