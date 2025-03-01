﻿using System;
using TvProgViewer.Core;

namespace TvProgViewer.Plugin.Tax.Avalara.Domain
{
    /// <summary>
    /// Represents a tax transaction log record
    /// </summary>
    public class TaxTransactionLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the response status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the requested URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the request message
        /// </summary>
        public string RequestMessage { get; set; }

        /// <summary>
        /// Gets or sets the response message
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of creation
        /// </summary>
        public DateTime CreatedDateUtc { get; set; }
    }
}