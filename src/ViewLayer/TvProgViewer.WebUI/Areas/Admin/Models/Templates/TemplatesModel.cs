using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a templates model
    /// </summary>
    public partial record TemplatesModel : BaseTvProgModel
    {
        #region Ctor

        public TemplatesModel()
        {
            TemplatesCategory = new CategoryTemplateSearchModel();
            TemplatesManufacturer = new ManufacturerTemplateSearchModel();
            TemplatesTvChannel = new TvChannelTemplateSearchModel();
            TemplatesTopic = new TopicTemplateSearchModel();

            AddCategoryTemplate = new CategoryTemplateModel();
            AddManufacturerTemplate = new ManufacturerTemplateModel();
            AddTvChannelTemplate = new TvChannelTemplateModel();
            AddTopicTemplate = new TopicTemplateModel();
        }

        #endregion

        #region Properties

        public CategoryTemplateSearchModel TemplatesCategory { get; set; }

        public ManufacturerTemplateSearchModel TemplatesManufacturer { get; set; }

        public TvChannelTemplateSearchModel TemplatesTvChannel { get; set; }

        public TopicTemplateSearchModel TemplatesTopic { get; set; }

        public CategoryTemplateModel AddCategoryTemplate { get; set; }

        public ManufacturerTemplateModel AddManufacturerTemplate { get; set; }

        public TvChannelTemplateModel AddTvChannelTemplate { get; set; }

        public TopicTemplateModel AddTopicTemplate { get; set; }

        #endregion
    }
}
