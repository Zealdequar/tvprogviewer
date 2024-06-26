﻿using System.Collections.Generic;
using ClosedXML.Excel;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Services.ExportImport.Help;

namespace TvProgViewer.Services.ExportImport
{
    public class WorkbookMetadata<T>
    {
        public List<PropertyByName<T, Language>> DefaultProperties { get; set; }

        public List<PropertyByName<T, Language>> LocalizedProperties { get; set; }

        public IXLWorksheet DefaultWorksheet { get; set; }

        public List<IXLWorksheet> LocalizedWorksheets { get; set; }
    }
}
