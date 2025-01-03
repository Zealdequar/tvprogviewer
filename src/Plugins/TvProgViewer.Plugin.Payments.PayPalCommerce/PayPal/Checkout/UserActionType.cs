﻿namespace PayPalCheckoutSdk.Orders
{
    /// <summary>
    /// Represents the type of user action for user checkout flow
    /// </summary>
    public enum UserActionType
    {
        /// <summary>
        /// After you redirect the user to the PayPal payment page, a Continue button appears.
        /// </summary>
        Continue,

        /// <summary>
        /// After you redirect the user to the PayPal payment page, a Pay Now button appears.
        /// </summary>
        Pay_now
    }
}