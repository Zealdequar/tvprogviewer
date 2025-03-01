﻿using System;
using System.Threading.Tasks;
using TvProgViewer.Services.Events;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Events
{
    public class DateTimeConsumer : IConsumer<DateTime>
    {
        public Task HandleEventAsync(DateTime eventMessage)
        {
            DateTime = eventMessage;

            return Task.CompletedTask;
        }

        // For testing
        public static DateTime DateTime { get; set; }
    }
}
