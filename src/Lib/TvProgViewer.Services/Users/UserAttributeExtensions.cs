using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Services.Users
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class UserAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this user attribute should have values
        /// </summary>
        /// <param name="userAttribute">User attribute</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this UserAttribute userAttribute)
        {
            if (userAttribute == null)
                return false;

            if (userAttribute.AttributeControlType == AttributeControlType.TextBox ||
                userAttribute.AttributeControlType == AttributeControlType.MultilineTextbox ||
                userAttribute.AttributeControlType == AttributeControlType.Datepicker ||
                userAttribute.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support values
            return true;
        }
    }
}
