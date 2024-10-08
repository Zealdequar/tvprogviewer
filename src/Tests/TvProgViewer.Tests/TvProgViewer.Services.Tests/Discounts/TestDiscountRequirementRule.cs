﻿using System;
using System.Threading.Tasks;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Discounts
{
    public partial class TestDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current user, discount, etc)</param>
        /// <returns>Result</returns>
        public Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
        {
            return Task.FromResult(new DiscountRequirementValidationResult
            {
                IsValid = true
            });
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            throw new NotImplementedException();
        }
    }
}
