using System.Collections.Generic;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    public partial record TvChannelAttributeConditionModel : BaseTvProgModel
    {
        public TvChannelAttributeConditionModel()
        {
            TvChannelAttributes = new List<TvChannelAttributeModel>();
        }
        
        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Condition.EnableCondition")]
        public bool EnableCondition { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelAttributes.Attributes.Condition.Attributes")]
        public int SelectedTvChannelAttributeId { get; set; }
        public IList<TvChannelAttributeModel> TvChannelAttributes { get; set; }

        public int TvChannelAttributeMappingId { get; set; }

        #region Nested classes

        public partial record TvChannelAttributeModel : BaseTvProgEntityModel
        {
            public TvChannelAttributeModel()
            {
                Values = new List<TvChannelAttributeValueModel>();
            }

            public int TvChannelAttributeId { get; set; }

            public string Name { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<TvChannelAttributeValueModel> Values { get; set; }
        }

        public partial record TvChannelAttributeValueModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }

        #endregion
    }
}