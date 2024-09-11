using TvProgViewer.Core.Domain.Catalog;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class TvChannelAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this tvChannel attribute should have values
        /// </summary>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            if (tvChannelAttributeMapping == null)
                return false;

            if (tvChannelAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support values
            return true;
        }

        /// <summary>
        /// A value indicating whether this tvChannel attribute can be used as condition for some other attribute
        /// </summary>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool CanBeUsedAsCondition(this TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            if (tvChannelAttributeMapping == null)
                return false;

            if (tvChannelAttributeMapping.AttributeControlType == AttributeControlType.ReadonlyCheckboxes || 
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute control types support it
            return true;
        }

        /// <summary>
        /// A value indicating whether this tvChannel attribute should can have some validation rules
        /// </summary>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ValidationRulesAllowed(this TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            if (tvChannelAttributeMapping == null)
                return false;

            if (tvChannelAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                tvChannelAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return true;

            //other attribute control types does not have validation
            return false;
        }

        /// <summary>
        /// A value indicating whether this tvChannel attribute is non-combinable
        /// </summary>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>Result</returns>
        public static bool IsNonCombinable(this TvChannelAttributeMapping tvChannelAttributeMapping)
        {
            //When you have a tvChannel with several attributes it may well be that some are combinable,
            //whose combination may form a new SKU with its own inventory,
            //and some non-combinable are more used to add accessories

            if (tvChannelAttributeMapping == null)
                return false;

            //we can add a new property to "TvChannelAttributeMapping" entity indicating whether it's combinable/non-combinable
            //but we assume that attributes
            //which cannot have values (any value can be entered by a user)
            //are non-combinable
            var result = !ShouldHaveValues(tvChannelAttributeMapping);
            return result;
        }
    }
}
