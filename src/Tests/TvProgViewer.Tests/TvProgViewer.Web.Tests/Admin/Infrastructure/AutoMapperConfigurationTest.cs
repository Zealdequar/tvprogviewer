using AutoMapper;
using TvProgViewer.Core.Infrastructure.Mapper;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Admin.Infrastructure
{
    [TestFixture]
    public class AutoMapperConfigurationTest
    {
        [Test]
        public void ConfigurationIsValid()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(AdminMapperConfiguration));
            });
            
            AutoMapperConfiguration.Init(config);
            AutoMapperConfiguration.MapperConfiguration.AssertConfigurationIsValid();
        }
    }
}