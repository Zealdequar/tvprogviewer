﻿using System;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Services.Common;
using TvProgViewer.WebUI.Areas.Admin.Models.Common;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the address model factory implementation
    /// </summary>
    public partial class AddressModelFactory : IAddressModelFactory
    {
        #region Fields

        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;

        #endregion

        #region Ctor

        public AddressModelFactory(IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory)
        {
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareAddressModelAsync(AddressModel model, Address address = null)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available countries
            await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);

            //prepare available states
            await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId);

            //prepare custom address attributes
            await _addressAttributeModelFactory.PrepareCustomAddressAttributesAsync(model.CustomAddressAttributes, address);

            if (address == null)
                return;

            model.FormattedCustomAddressAttributes = await _addressAttributeFormatter.FormatAttributesAsync(address.CustomAttributes);
        }

        #endregion
    }
}