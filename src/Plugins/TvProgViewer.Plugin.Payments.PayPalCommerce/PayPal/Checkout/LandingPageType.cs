namespace PayPalCheckoutSdk.Orders
{
    /// <summary>
    /// Represents the type of landing page to show on the PayPal site for user checkout
    /// </summary>
    public enum LandingPageType
    {
        /// <summary>
        /// When the user clicks PayPal Checkout, the user is redirected to a page to log in to PayPal and approve the payment.
        /// </summary>
        Login,

        /// <summary>
        /// When the user clicks PayPal Checkout, the user is redirected to a page to enter credit or debit card and other relevant billing information required to complete the purchase.
        /// </summary>
        Billing,

        /// <summary>
        /// When the user clicks PayPal Checkout, the user is redirected to either a page to log in to PayPal and approve the payment or to a page to enter credit or debit card and other relevant billing information required to complete the purchase, depending on their previous interaction with PayPal.
        /// </summary>
        No_preference
    }
}