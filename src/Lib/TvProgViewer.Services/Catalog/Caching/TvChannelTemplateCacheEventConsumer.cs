﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel template cache event consumer
    /// </summary>
    public partial class TvChannelTemplateCacheEventConsumer : CacheEventConsumer<TvChannelTemplate>
    {
    }
}
