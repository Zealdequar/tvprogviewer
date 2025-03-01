﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Services.Cms;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public
{
    public class TestWidgetPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList { get; } = false;
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>{ "test widget zone" });
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(TestWidgetPlugin);
        }
    }
}
