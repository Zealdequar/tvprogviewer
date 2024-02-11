using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvchannel review and review type mapping list model
    /// </summary>
    public partial record TvChannelReviewReviewTypeMappingListModel : BasePagedListModel<TvChannelReviewReviewTypeMappingModel>
    {
    }
}
