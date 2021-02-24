﻿using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Catalog
{
    public partial record ManufacturerNavigationModel : BaseTvProgModel
    {
        public ManufacturerNavigationModel()
        {
            Manufacturers = new List<ManufacturerBriefInfoModel>();
        }

        public IList<ManufacturerBriefInfoModel> Manufacturers { get; set; }

        public int TotalManufacturers { get; set; }
    }

    public partial record ManufacturerBriefInfoModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }
        
        public bool IsActive { get; set; }
    }
}