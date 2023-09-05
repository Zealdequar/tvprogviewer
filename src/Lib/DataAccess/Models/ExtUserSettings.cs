﻿using System;
using System.Collections.Generic;

namespace TvProgViewer.DataAccess.Models
{
    /// <summary>
    /// Перенесено на TvProgViewer.Core.Domain
    /// </summary>
    public partial class ExtUserSettings
    {
        public int ExtUserSettingsId { get; set; }
        public long Uid { get; set; }
        public int TvprogProviderId { get; set; }
        public short? UncheckedChannels { get; set; }

        public virtual TvprogProviders TvprogProvider { get; set; }
        public virtual SystemUsers U { get; set; }
    }
}
