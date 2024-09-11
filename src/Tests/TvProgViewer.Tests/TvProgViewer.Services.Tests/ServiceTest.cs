using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Moq;
using TvProgViewer.Core;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data.Configuration;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Tests.TvProgViewer.Services.Tests.Directory;
using TvProgViewer.Tests.TvProgViewer.Services.Tests.Discounts;
using TvProgViewer.Tests.TvProgViewer.Services.Tests.Payments;
using TvProgViewer.Tests.TvProgViewer.Services.Tests.Shipping;
using TvProgViewer.Tests.TvProgViewer.Services.Tests.Tax;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests
{
    [TestFixture]
    public abstract class ServiceTest : BaseTvProgTest
    {
        protected ServiceTest()
        {
            //init plugins
            InitPlugins();
        }

        private static void InitPlugins()
        {
            var webHostEnvironment = new Mock<IWebHostEnvironment>();
            webHostEnvironment.Setup(x => x.ContentRootPath).Returns(System.Reflection.Assembly.GetExecutingAssembly().Location);
            webHostEnvironment.Setup(x => x.WebRootPath).Returns(System.IO.Directory.GetCurrentDirectory());
            CommonHelper.DefaultFileProvider = new TvProgFileProvider(webHostEnvironment.Object);
            
            Environment.SetEnvironmentVariable("ConnectionStrings", Singleton<DataConfig>.Instance.ConnectionString);

            Singleton<IPluginsInfo>.Instance = new PluginsInfo(CommonHelper.DefaultFileProvider)
            {
                PluginDescriptors = new List<(PluginDescriptor, bool)>
                {
                    (new PluginDescriptor
                    {
                        PluginType = typeof(FixedRateTestTaxProvider),
                        SystemName = "FixedTaxRateTest",
                        FriendlyName = "Fixed tax test rate provider",
                        Installed = true,
                        ReferencedAssembly = typeof(FixedRateTestTaxProvider).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(FixedRateTestShippingRateComputationMethod),
                        SystemName = "FixedRateTestShippingRateComputationMethod",
                        FriendlyName = "Fixed rate test shipping computation method",
                        Installed = true,
                        ReferencedAssembly = typeof(FixedRateTestShippingRateComputationMethod).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestPaymentMethod),
                        SystemName = "Payments.TestMethod",
                        FriendlyName = "Test payment method",
                        Installed = true,
                        ReferencedAssembly = typeof(TestPaymentMethod).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestDiscountRequirementRule),
                        SystemName = "TestDiscountRequirementRule",
                        FriendlyName = "Test discount requirement rule",
                        Installed = true,
                        ReferencedAssembly = typeof(TestDiscountRequirementRule).Assembly
                    }, true),
                    (new PluginDescriptor
                    {
                        PluginType = typeof(TestExchangeRateProvider),
                        SystemName = "CurrencyExchange.TestProvider",
                        FriendlyName = "Test exchange rate provider",
                        Installed = true,
                        ReferencedAssembly = typeof(TestExchangeRateProvider).Assembly
                    }, true)
                }
            };
        }
    }
}
