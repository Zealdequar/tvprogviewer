using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TvProgViewer.Data.TvProgMain
{
    [DataContract]
    public class LocalStorageOptChans
    {
        [DataMember]
        public string pageNum { get; set; }

        [DataMember]
        public long rowId { get; set; }

        [DataMember]
        public long ChannelId { get; set; }
    }
}
