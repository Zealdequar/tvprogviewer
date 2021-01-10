using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Payments
{
    /// <summary>
    /// Represents a payment method list model
    /// </summary>
    public partial record PaymentMethodListModel : BasePagedListModel<PaymentMethodModel>
    {
    }
}