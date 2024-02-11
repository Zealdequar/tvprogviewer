using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class TvChannelAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this tvchannel attribute should have values
        /// </summary>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            if (tvchannelAttributeMapping == null)
                return false;

            if (tvchannelAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support values
            return true;
        }

        /// <summary>
        /// A value indicating whether this tvchannel attribute can be used as condition for some other attribute
        /// </summary>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool CanBeUsedAsCondition(this TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            if (tvchannelAttributeMapping == null)
                return false;

            if (tvchannelAttributeMapping.AttributeControlType == AttributeControlType.ReadonlyCheckboxes || 
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support it
            return true;
        }

        /// <summary>
        /// A value indicating whether this tvchannel attribute should can have some validation rules
        /// </summary>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ValidationRulesAllowed(this TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            if (tvchannelAttributeMapping == null)
                return false;

            if (tvchannelAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                tvchannelAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return true;

            //other attribute control types does not have validation
            return false;
        }

        /// <summary>
        /// A value indicating whether this tvchannel attribute is non-combinable
        /// </summary>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool IsNonCombinable(this TvChannelAttributeMapping tvchannelAttributeMapping)
        {
            //When you have a tvchannel with several attributes it may well be that some are combinable,
            //whose combination may form a new SKU with its own inventory,
            //and some non-combinable are more used to add accessories

            if (tvchannelAttributeMapping == null)
                return false;

            //we can add a new property to "TvChannelAttributeMapping" entity indicating whether it's combinable/non-combinable
            //but we assume that attributes
            //which cannot have values (any value can be entered by a user)
            //are non-combinable
            var result = !ShouldHaveValues(tvchannelAttributeMapping);
            return result;
        }
    }
}
