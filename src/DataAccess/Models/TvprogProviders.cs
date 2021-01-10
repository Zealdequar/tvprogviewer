using System;
using System.Collections.Generic;

namespace TVProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TVProgViewer.Core.Domain
    /// </summary>
    public partial class TvprogProviders
    {
        public TvprogProviders()
        {
            Channels = new HashSet<Channels>();
            ExtUserSettings = new HashSet<ExtUserSettings>();
            TypeProg = new HashSet<TypeProg>();
        }

        public int TvprogProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ProviderWebSite { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string Rss { get; set; }

        public virtual ICollection<Channels> Channels { get; set; }
        public virtual ICollection<ExtUserSettings> ExtUserSettings { get; set; }
        public virtual ICollection<TypeProg> TypeProg { get; set; }
    }
}
