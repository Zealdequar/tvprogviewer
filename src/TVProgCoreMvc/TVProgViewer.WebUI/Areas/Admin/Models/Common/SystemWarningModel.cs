using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    public partial record SystemWarningModel : BaseTvProgModel
    {
        public SystemWarningLevel Level { get; set; }

        public string Text { get; set; }

        public bool DontEncode { get; set; }
    }

    public enum SystemWarningLevel
    {
        Pass,
        Recommendation,
        CopyrightRemovalKey,
        Warning,
        Fail
    }
}