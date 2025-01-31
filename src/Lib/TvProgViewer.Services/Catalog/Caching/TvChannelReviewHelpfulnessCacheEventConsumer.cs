﻿using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a tvChannel review helpfulness cache event consumer
    /// </summary>
    public partial class TvChannelReviewHelpfulnessCacheEventConsumer : CacheEventConsumer<TvChannelReviewHelpfulness>
    {
    }
}
