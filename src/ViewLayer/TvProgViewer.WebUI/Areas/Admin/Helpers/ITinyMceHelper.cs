using System.Threading.Tasks;

namespace TvProgViewer.WebUI.Areas.Admin.Helpers
{
    public partial interface ITinyMceHelper
    {
        Task<string> GetTinyMceLanguageAsync();
    }
}