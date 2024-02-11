using System.Collections.Generic;
using System.Linq;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record TopMenuModel : BaseTvProgModel
    {
        public TopMenuModel()
        {
            Categories = new List<CategorySimpleModel>();
            Topics = new List<TopicModel>();
        }

        public IList<CategorySimpleModel> Categories { get; set; }
        public IList<TopicModel> Topics { get; set; }

        public bool BlogEnabled { get; set; }
        public bool NewTvChannelsEnabled { get; set; }
        public bool ForumEnabled { get; set; }

        public bool DisplayHomepageMenuItem { get; set; }
        public bool DisplayNewTvChannelsMenuItem { get; set; }
        public bool DisplayTvChannelSearchMenuItem { get; set; }
        public bool DisplayUserInfoMenuItem { get; set; }
        public bool DisplayBlogMenuItem { get; set; }
        public bool DisplayForumsMenuItem { get; set; }
        public bool DisplayContactUsMenuItem { get; set; }

        public bool UseAjaxMenu { get; set; }

        public bool HasOnlyCategories => Categories.Any()
                       && !Topics.Any()
                       && !DisplayHomepageMenuItem
                       && !(DisplayNewTvChannelsMenuItem && NewTvChannelsEnabled)
                       && !DisplayTvChannelSearchMenuItem
                       && !DisplayUserInfoMenuItem
                       && !(DisplayBlogMenuItem && BlogEnabled)
                       && !(DisplayForumsMenuItem && ForumEnabled)
                       && !DisplayContactUsMenuItem;

        #region Вложенные классы

        public partial record TopicModel : BaseTvProgEntityModel
        {
            public string Name { get; set; }
            public string SeName { get; set; }
        }

        public partial record CategoryLineModel : BaseTvProgModel
        {
            public int Level { get; set; }
            public bool ResponsiveMobileMenu { get; set; }
            public CategorySimpleModel Category { get; set; }
        }

        #endregion
    }
}