using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Moq;
using TvProgViewer.Core;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests
{
    [TestFixture]
    public abstract class WebTest : BaseTvProgTest
    {
        protected WebTest()
        {
            //init plugins
            InitPlugins();
        }

        private void InitPlugins()
        {
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new TvProgFileProvider(webHostEnvironment.Object);

            Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
            {
                PluginDescriptors = new List<(PluginDescriptor, bool)>
                {
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestWidgetPlugin),
                        SystemName = "TestWidgetPlugin",
                        FriendlyName = "Test widget plugin",
                        Installed = true,
                        ReferencedAssembly = typeof(TestWidgetPlugin).Assembly
                    }, true)
                }
            };
        }
    }
}
