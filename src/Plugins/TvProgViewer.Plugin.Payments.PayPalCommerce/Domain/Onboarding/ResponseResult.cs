using System.Runtime.Serialization;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Domain.Onboarding
{
    /// <summary>
    /// Represents response result enumeration
    /// </summary>
    public enum ResponseResult
    {
        /// <summary>
        /// Request failed
        /// </summary>
        [EnumMember(Value = "error")]
        Error,

        /// <summary>
        /// Request was successful
        /// </summary>
        [EnumMember(Value = "success")]
        Success
    }
}