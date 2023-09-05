using System;
using System.Collections.Generic;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.ExportImport.Help;

namespace TvProgViewer.Services.ExportImport
{
    public partial class ImportOrderMetadata
    {
        public int EndRow { get; internal set; }

        public PropertyManager<Order, Language> Manager { get; internal set; }

        public IList<PropertyByName<Order, Language>> Properties { get; set; }

        public int CountOrdersInFile { get; set; }

        public PropertyManager<OrderItem, Language> OrderItemManager { get; internal set; }

        public List<Guid> AllOrderGuids { get; set; }

        public List<Guid> AllUserGuids { get; set; }
    }
}
