﻿using System.Runtime.Serialization;

namespace TvProgViewer.Core.Configuration
{
    /// <summary>
    /// Represents distributed cache types enumeration
    /// </summary>
    public enum DistributedCacheType
    {
        [EnumMember(Value = "memory")]
        Memory,
        [EnumMember(Value = "sqlserver")]
        SqlServer,
        [EnumMember(Value = "redis")]
        Redis,
        [EnumMember(Value = "redissynchronizedmemory")]
        RedisSynchronizedMemory
    }
}