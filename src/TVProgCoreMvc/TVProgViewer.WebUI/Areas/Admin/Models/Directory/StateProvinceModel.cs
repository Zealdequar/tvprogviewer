using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a state and province model
    /// </summary>
    public partial record StateProvinceModel : BaseTvProgEntityModel, ILocalizedModel<StateProvinceLocalizedModel>
    {
        #region Ctor

        public StateProvinceModel()
        {
            Locales = new List<StateProvinceLocalizedModel>();
        }

        #endregion

        #region Properties

        public int CountryId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Countries.States.Fields.Abbreviation")]
        public string Abbreviation { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
        public bool Published { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<StateProvinceLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record StateProvinceLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        public string Name { get; set; }
    }
}