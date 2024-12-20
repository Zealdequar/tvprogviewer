﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record TvChannelsByTagModel : BaseTvProgEntityModel
    {
        public TvChannelsByTagModel()
        {
            CatalogTvChannelsModel = new CatalogTvChannelsModel();
        }

        public string TagName { get; set; }
        public string TagSeName { get; set; }

        public CatalogTvChannelsModel CatalogTvChannelsModel { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
    }
}