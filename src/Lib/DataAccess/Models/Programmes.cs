using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    // Перенесено на TvProgViewer.Data.Domain
    public partial class Programmes
    {
        public Programmes()
        {
            UsersPrograms = new HashSet<UsersPrograms>();
        }

        public long ProgrammesId { get; set; }
        public int Tid { get; set; }
        public int Cid { get; set; }
        public int? InternalChanId { get; set; }
        public DateTimeOffset TsStart { get; set; }
        public DateTimeOffset TsStop { get; set; }
        public DateTime TsStartMo { get; set; }
        public DateTime TsStopMo { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public string Category { get; set; }

        public virtual Channels C { get; set; }
        public virtual TypeProg T { get; set; }
        public virtual ICollection<UsersPrograms> UsersPrograms { get; set; }
    }
}
