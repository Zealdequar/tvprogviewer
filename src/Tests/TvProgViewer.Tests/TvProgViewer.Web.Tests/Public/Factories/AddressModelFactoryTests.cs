using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Directory;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.WebUI.Models.Common;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Factories
{
    [TestFixture]
    public class AddressModelFactoryTests: BaseTvProgTest
    {
        private IAddressModelFactory _addressModelFactory;
        private IGenericAttributeService _genericAttributeService;
        private Address _address;
        private AddressSettings _addressSettings;
        private ICountryService _countryService;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            _addressModelFactory = GetService<IAddressModelFactory>();
            _genericAttributeService = GetService<IGenericAttributeService>();

            _address = await GetService<IAddressService>().GetAddressByIdAsync(1);
            _addressSettings = GetService<AddressSettings>();
            _countryService = GetService<ICountryService>();
        }

        [Test]
        public async Task PrepareAddressModelShouldPopulatingPropertiesFromEntity()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, _address, false, _addressSettings);

            model.Id.Should().Be(_address.Id);
            model.FirstName.Should().Be(_address.FirstName);
            model.LastName.Should().Be(_address.LastName);
            model.Email.Should().Be(_address.Email);
            model.Company.Should().Be(_address.Company);
            model.CountryId.Should().Be(_address.CountryId);
            model.StateProvinceId.Should().Be(_address.StateProvinceId);
            model.County.Should().Be(_address.County);
            model.City.Should().Be(_address.City);
            model.Address1.Should().Be(_address.Address1);
            model.Address2.Should().Be(_address.Address2);
            model.ZipPostalCode.Should().Be(_address.ZipPostalCode);
            model.PhoneNumber.Should().Be(_address.PhoneNumber);
            model.FaxNumber.Should().Be(_address.FaxNumber);
        }

        [Test]
        public async Task PrepareAddressModelShouldNotPopulatingPropertiesFromEntityIfExcludePropertiesFlagEnabled()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, _address, true, _addressSettings);

            model.Id.Should().Be(0);
            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.Email.Should().BeNull();
            model.Company.Should().BeNull();
            model.CountryId.Should().BeNull();
            model.StateProvinceId.Should().BeNull();
            model.County.Should().BeNull();
            model.City.Should().BeNull();
            model.Address1.Should().BeNull();
            model.Address2.Should().BeNull();
            model.ZipPostalCode.Should().BeNull();
            model.PhoneNumber.Should().BeNull();
            model.FaxNumber.Should().BeNull();
        }

        [Test]
        public async Task PrepareAddressModelShouldNotPopulatingPropertiesFromEntityIfEntityIsNull()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings);

            model.Id.Should().Be(0);
            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.Email.Should().BeNull();
            model.Company.Should().BeNull();
            model.CountryId.Should().BeNull();
            model.StateProvinceId.Should().BeNull();
            model.County.Should().BeNull();
            model.City.Should().BeNull();
            model.Address1.Should().BeNull();
            model.Address2.Should().BeNull();
            model.ZipPostalCode.Should().BeNull();
            model.PhoneNumber.Should().BeNull();
            model.FaxNumber.Should().BeNull();
        }

        [Test]
        public void PrepareAddressModelShouldRaiseExceptionIfPrePopulateWithUserFieldsFlagEnabledButUserNotPassed()
        {
            var model = new AddressModel();

            Assert.Throws<AggregateException>(() =>
                _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings,
                    prePopulateWithUserFields: true).Wait());
        }

        [Test]
        public async Task PrepareAddressModelShouldFillAvailableCountriesAndAvailableStates()
        {
            var model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings,
                async () => await _countryService.GetAllCountriesAsync());
            model.AvailableCountries.Any().Should().BeTrue();
            model.AvailableCountries.Count.Should().Be(250);
            model.AvailableStates.Any().Should().BeTrue();
            model.AvailableStates.Count.Should().Be(1);

            model = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(model, _address, false, _addressSettings,
                async () => await _countryService.GetAllCountriesAsync());
            model.AvailableCountries.Any().Should().BeTrue();
            model.AvailableCountries.Count.Should().Be(250);
            model.AvailableStates.Any().Should().BeTrue();
            model.AvailableStates.Count.Should().Be(63);
        }

        [Test]
        public async Task PrepareAddressModelShouldFillUsersInfoIfPrePopulateWithUserFieldsFlagEnabledAndUserPassed()
        {
            var model = new AddressModel();
            var user = await GetService<IWorkContext>().GetCurrentUserAsync();
            await _addressModelFactory.PrepareAddressModelAsync(model, null, false, _addressSettings,
                prePopulateWithUserFields: true, user: user);

            model.Email.Should().Be(user.Email);
            model.FirstName.Should().Be(user.FirstName);
            model.LastName.Should().Be(user.LastName);

            model.Company.Should().Be(user.Company);
            model.Address1.Should().Be(user.StreetAddress);
            model.Address2.Should().Be(user.StreetAddress2);
            model.ZipPostalCode.Should().Be(user.ZipPostalCode);
            model.City.Should().Be(user.City);
            model.County.Should().Be(user.County);
            model.PhoneNumber.Should().Be(user.SmartPhone);
            model.FaxNumber.Should().Be(user.Fax);
        }
    }
}
