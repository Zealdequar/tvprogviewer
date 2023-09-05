﻿using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record VendorNavigationModel : BaseTvProgModel
    {
        public VendorNavigationModel()
        {
            Vendors = new List<VendorBriefInfoModel>();
        }

        public IList<VendorBriefInfoModel> Vendors { get; set; }

        public int TotalVendors { get; set; }
    }

    public partial record VendorBriefInfoModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
    }
}