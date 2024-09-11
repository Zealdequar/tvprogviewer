using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Catalog;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Catalog
{
    [TestFixture]
    public class TvChannelAttributeParserTests : ServiceTest
    {
        private ITvChannelAttributeParser _tvChannelAttributeParser;
        private ITvChannelAttributeFormatter _tvChannelAttributeFormatter;
        private IEnumerable<KeyValuePair<TvChannelAttributeMapping, IList<TvChannelAttributeValue>>> _tvChannelAttributeMappings;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            var tvChannelAttributeService = GetService<ITvChannelAttributeService>();

            _tvChannelAttributeParser = GetService<ITvChannelAttributeParser>();
            _tvChannelAttributeFormatter = GetService<ITvChannelAttributeFormatter>();

            var tvChannel = await GetService<ITvChannelService>()
                .GetTvChannelBySkuAsync("COMP_CUST");
            var mappings = await tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id);
            _tvChannelAttributeMappings = await mappings.SelectAwait(async p => KeyValuePair.Create(p, await tvChannelAttributeService.GetTvChannelAttributeValuesAsync(p.Id))).ToListAsync();
        }

        [Test]
        public async Task CanAddAndParseTvChannelAttributes()
        {
            var attributes = string.Empty;

            foreach (var tvChannelAttributeMapping in _tvChannelAttributeMappings)
            {
                var skip = true;

                foreach (var tvChannelAttributeValue in tvChannelAttributeMapping.Value.OrderBy(p => p.Id))
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }

                    attributes = _tvChannelAttributeParser.AddTvChannelAttribute(attributes, tvChannelAttributeMapping.Key, tvChannelAttributeValue.Id.ToString());
                }
            }

            var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributes);

            var parsedAttributeValues = attributeValues.Select(p => p.Id).ToList();

            foreach (var tvChannelAttributeMapping in _tvChannelAttributeMappings)
            {
                var skip = true;

               foreach (var tvChannelAttributeValue in tvChannelAttributeMapping.Value.OrderBy(p => p.Id))
                {
                    if (skip)
                    {
                        parsedAttributeValues.Contains(tvChannelAttributeValue.Id).Should().BeFalse();
                        skip = false;
                        continue;
                    }

                    parsedAttributeValues.Contains(tvChannelAttributeValue.Id).Should().BeTrue();
                }
            }
        }

        [Test]
        public async Task CanAddAndRemoveTvChannelAttributes()
        {
            var attributes = string.Empty;

            var delete = false;

            foreach (var tvChannelAttributeMapping in _tvChannelAttributeMappings)
            {
                foreach (var tvChannelAttributeValue in tvChannelAttributeMapping.Value.OrderBy(p => p.Id)) 
                    attributes = _tvChannelAttributeParser.AddTvChannelAttribute(attributes, tvChannelAttributeMapping.Key, tvChannelAttributeValue.Id.ToString());

                if (delete)
                    attributes = _tvChannelAttributeParser.RemoveTvChannelAttribute(attributes, tvChannelAttributeMapping.Key);

                delete = !delete;
            }

            var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributes);

            var parsedAttributeValues = attributeValues.Select(p => p.Id).ToList();

            delete = false;

            foreach (var tvChannelAttributeMapping in _tvChannelAttributeMappings)
            {
                foreach (var tvChannelAttributeValue in tvChannelAttributeMapping.Value.OrderBy(p => p.Id)) 
                    parsedAttributeValues.Contains(tvChannelAttributeValue.Id).Should().Be(!delete);

                delete = !delete;
            }
        }

        [Test]
        public async Task CanRenderAttributesWithoutPrices()
        {
            var attributes = string.Empty;

            foreach (var tvChannelAttributeMapping in _tvChannelAttributeMappings)
                foreach (var tvChannelAttributeValue in tvChannelAttributeMapping.Value.OrderBy(p => p.Id))
                    attributes = _tvChannelAttributeParser.AddTvChannelAttribute(attributes, tvChannelAttributeMapping.Key, tvChannelAttributeValue.Id.ToString());

            attributes = _tvChannelAttributeParser.AddGiftCardAttribute(attributes,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");
            
            var tvChannel = new TvChannel { IsGiftCard = true, GiftCardType = GiftCardType.Virtual };
            var user = new User();
            var store = new Store();

            var formattedAttributes = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel,
                attributes, user, store, "<br />", false);

            formattedAttributes.Should().Be(
                "Processor: 2.2 GHz Intel Pentium Dual-Core E2200<br />Processor: 2.5 GHz Intel Pentium Dual-Core E2200 [+$15.00]<br />RAM: 2 GB<br />RAM: 4GB [+$20.00]<br />RAM: 8GB [+$60.00]<br />HDD: 320 GB<br />HDD: 400 GB [+$100.00]<br />OS: Vista Home [+$50.00]<br />OS: Vista Premium [+$60.00]<br />Software: Microsoft Office [+$50.00]<br />Software: Acrobat Reader [+$10.00]<br />Software: Total Commander [+$5.00]<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");

            formattedAttributes = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel,
                attributes, user, store, "<br />", false, false);

            formattedAttributes.Should().Be(
                "Processor: 2.2 GHz Intel Pentium Dual-Core E2200<br />Processor: 2.5 GHz Intel Pentium Dual-Core E2200<br />RAM: 2 GB<br />RAM: 4GB<br />RAM: 8GB<br />HDD: 320 GB<br />HDD: 400 GB<br />OS: Vista Home<br />OS: Vista Premium<br />Software: Microsoft Office<br />Software: Acrobat Reader<br />Software: Total Commander<br />From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
        }

        [Test]
        public void CanAddAndParseGiftCardAttributes()
        {
            var attributes = string.Empty;
            attributes = _tvChannelAttributeParser.AddGiftCardAttribute(attributes,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            _tvChannelAttributeParser.GetGiftCardAttribute(attributes,
                out var recipientName,
                out var recipientEmail,
                out var senderName,
                out var senderEmail,
                out var giftCardMessage);
            recipientName.Should().Be("recipientName 1");
            recipientEmail.Should().Be("recipientEmail@gmail.com");
            senderName.Should().Be("senderName 1");
            senderEmail.Should().Be("senderEmail@gmail.com");
            giftCardMessage.Should().Be("custom message");
        }

        [Test]
        public async Task CanRenderVirtualGiftCart()
        {
            var attributes = _tvChannelAttributeParser.AddGiftCardAttribute(string.Empty,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var tvChannel = new TvChannel
            {
                IsGiftCard = true,
                GiftCardType = GiftCardType.Virtual
            };
            var user = new User();
            var store = new Store();
            
            var formattedAttributes = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel,
                attributes, user, store, "<br />", false, false);
            formattedAttributes.Should().Be("From: senderName 1 <senderEmail@gmail.com><br />For: recipientName 1 <recipientEmail@gmail.com>");
        }

        [Test]
        public async Task CanRenderPhysicalGiftCart()
        {
            var attributes = _tvChannelAttributeParser.AddGiftCardAttribute(string.Empty,
                "recipientName 1", "recipientEmail@gmail.com",
                "senderName 1", "senderEmail@gmail.com", "custom message");

            var tvChannel = new TvChannel
            {
                IsGiftCard = true,
                GiftCardType = GiftCardType.Physical
            };
            var user = new User();
            var store = new Store();
            
            var formattedAttributes = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel,
                attributes, user, store, "<br />", false, false);
            formattedAttributes.Should().Be("From: senderName 1<br />For: recipientName 1");
        }
    }
}