using System.Threading.Tasks;
using TVProgViewer.WebUI.Areas.Admin.Models.Payments;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the payment method model factory
    /// </summary>
    public partial interface IPaymentModelFactory
    {
        /// <summary>
        /// Prepare payment methods model
        /// </summary>
        /// <param name="methodsModel">Payment methods model</param>        
        /// <returns>Payment methods model</returns>
        Task<PaymentMethodsModel> PreparePaymentMethodsModelAsync(PaymentMethodsModel methodsModel);

        /// <summary>
        /// Prepare paged payment method list model
        /// </summary>
        /// <param name="searchModel">Payment method search model</param>
        /// <returns>Payment method list model</returns>
        Task<PaymentMethodListModel> PreparePaymentMethodListModelAsync(PaymentMethodSearchModel searchModel);
    }
}