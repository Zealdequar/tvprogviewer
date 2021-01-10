using System;
using System.Collections.Generic;

namespace TVProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TVProgViewer.Core.Domain
    /// </summary>
    public partial class WebResources
    {
        public WebResources()
        {
            UpdateProgLog = new HashSet<UpdateProgLog>();
        }

        public int WebResourceId { get; set; }
        public int Tpid { get; set; }
        public string FileName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceUrl { get; set; }

        public virtual TypeProg Tp { get; set; }
        public virtual ICollection<UpdateProgLog> UpdateProgLog { get; set; }
    }
}
