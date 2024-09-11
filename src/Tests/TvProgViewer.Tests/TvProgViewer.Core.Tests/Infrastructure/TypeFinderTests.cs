using System.Linq;
using FluentAssertions;
using TvProgViewer.Core.Infrastructure;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Core.Tests.Infrastructure
{
    [TestFixture]
    public class TypeFinderTests : BaseTvProgTest
    {
        [Test]
        public void TypeFinderBenchmarkFindings()
        {
            var finder = GetService<ITypeFinder>();
            var type = finder.FindClassesOfType<ISomeInterface>().ToList();
            type.Count.Should().Be(1);
            typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).Should().BeTrue();
        }

        public interface ISomeInterface
        {
        }

        public class SomeClass : ISomeInterface
        {
        }
    }
}
