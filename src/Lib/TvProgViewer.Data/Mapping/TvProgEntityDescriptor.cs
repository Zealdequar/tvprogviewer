using System.Collections.Generic;

namespace TvProgViewer.Data.Mapping
{
    public partial class TvProgEntityDescriptor
    {
        public TvProgEntityDescriptor()
        {
            Fields = new List<TvProgEntityFieldDescriptor>();
        }

        public string EntityName { get; set; }
        public string SchemaName { get; set; }
        public ICollection<TvProgEntityFieldDescriptor> Fields { get; set; }
    }
}