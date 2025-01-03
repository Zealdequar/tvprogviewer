﻿using FluentAssertions;
using TvProgViewer.Core.Domain.Configuration;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Core.Tests.Domain.Configuration
{
    [TestFixture]
    public class SettingTestFixture
    {
        [Test]
        public void CanCreateSetting()
        {
            var setting = new Setting("Setting1", "Value1");
            setting.Name.Should().Be("Setting1");
            setting.Value.Should().Be("Value1");
        }
    }
}
