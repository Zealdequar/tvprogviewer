using System.Collections.Generic;
using ClosedXML.Excel;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Services.ExportImport.Help;

namespace TvProgViewer.Services.ExportImport
{
    public partial class ImportTvChannelMetadata
    {
        public int EndRow { get; internal set; }

        public PropertyManager<TvChannel, Language> Manager { get; internal set; }

        public IList<PropertyByName<TvChannel, Language>> Properties { get; set; }

        public int CountTvChannelsInFile => TvChannelsInFile.Count;

        public PropertyManager<ExportTvChannelAttribute, Language> TvChannelAttributeManager { get; internal set; }

        public PropertyManager<ExportSpecificationAttribute, Language> SpecificationAttributeManager { get; internal set; }

        public IXLWorksheet DefaultWorksheet { get; set; }

        public List<IXLWorksheet> LocalizedWorksheets { get; set; }

        public int SkuCellNum { get; internal set; }

        public List<string> AllSku { get; set; }

        public List<int> TvChannelsInFile { get; set; }
    }
}
