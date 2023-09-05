namespace TvProgViewer.Plugin.Shipping.UPS.Domain
{
    /// <summary>
    /// Represents pickup type
    /// </summary>
    /// <remarks>
    /// Updated at January 7, 2019
    /// </remarks>
    public enum PickupType
    {
        /// <summary>
        /// Daily pickup
        /// </summary>
        [UPSCode("01")]
        DailyPickup,

        /// <summary>
        /// User counter
        /// </summary>
        [UPSCode("03")]
        UserCounter,

        /// <summary>
        /// One time pickup
        /// </summary>
        [UPSCode("06")]
        OneTimePickup,

        /// <summary>
        /// On call air
        /// </summary>
        [UPSCode("07")]
        OnCallAir,

        /// <summary>
        /// Letter center
        /// </summary>
        [UPSCode("19")]
        LetterCenter,

        /// <summary>
        /// Air service center
        /// </summary>
        [UPSCode("20")]
        AirServiceCenter
    }
}