﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request action model
    /// </summary>
    public partial record ReturnRequestActionModel : BaseTvProgEntityModel, ILocalizedModel<ReturnRequestActionLocalizedModel>
    {
        #region Ctor

        public ReturnRequestActionModel()
        {
            Locales = new List<ReturnRequestActionLocalizedModel>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.Name")]
        public string Name { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ReturnRequestActionLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial record ReturnRequestActionLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.Name")]
        public string Name { get; set; }
    }
}