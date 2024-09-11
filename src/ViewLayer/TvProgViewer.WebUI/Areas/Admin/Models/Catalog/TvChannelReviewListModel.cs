using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel review list model
    /// </summary>
    public partial record TvChannelReviewListModel : BasePagedListModel<TvChannelReviewModel>
    {
    }
}