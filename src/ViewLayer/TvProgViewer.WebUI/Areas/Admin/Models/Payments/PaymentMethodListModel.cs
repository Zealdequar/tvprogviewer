using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Payments
{
    /// <summary>
    /// Represents a payment method list model
    /// </summary>
    public partial record PaymentMethodListModel : BasePagedListModel<PaymentMethodModel>
    {
    }
}