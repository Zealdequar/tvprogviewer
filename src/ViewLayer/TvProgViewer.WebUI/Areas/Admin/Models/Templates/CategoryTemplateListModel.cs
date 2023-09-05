﻿using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Templates
{
    /// <summary>
    /// Represents a category template list model
    /// </summary>
    public partial record CategoryTemplateListModel : BasePagedListModel<CategoryTemplateModel>
    {
    }
}