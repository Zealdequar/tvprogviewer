using System;
using System.Collections.Generic;

namespace TVProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TVProgViewer.Core.Domain
    /// </summary>
    public partial class TypeProg
    {
        public TypeProg()
        {
            Programmes = new HashSet<Programmes>();
            WebResources = new HashSet<WebResources>();
        }

        public int TypeProgId { get; set; }
        public int TvprogProviderId { get; set; }
        public string TypeName { get; set; }
        public string FileFormat { get; set; }

        public virtual TvprogProviders TvprogProvider { get; set; }
        public virtual ICollection<Programmes> Programmes { get; set; }
        public virtual ICollection<WebResources> WebResources { get; set; }
    }
}
