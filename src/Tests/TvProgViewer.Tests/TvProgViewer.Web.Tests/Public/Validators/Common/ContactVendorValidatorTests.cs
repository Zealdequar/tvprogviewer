﻿using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Validators.Common;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.Common
{
    [TestFixture]
    public class ContactVendorValidatorTests : BaseTvProgTest
    {
        private ContactVendorValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<ContactVendorValidator>();
        }
        
        [Test]
        public void ShouldHaveErrorWhenEmailIsNullOrEmpty()
        {
            var model = new ContactVendorModel
            {
                Email = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
            model.Email = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenEmailIsWrongFormat()
        {
            var model = new ContactVendorModel
            {
                Email = "adminexample.com"
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEmailIsCorrectFormat()
        {
            var model = new ContactVendorModel
            {
                Email = "admin@example.com"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        public void ShouldHaveErrorWhenFullnameIsNullOrEmpty()
        {
            var model = new ContactVendorModel
            {
                FullName = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
            model.FullName = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Test]
        public void ShouldNotHaveErrorWhenFullnameIsSpecified()
        {
            var model = new ContactVendorModel
            {
                FullName = "John Smith"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.FullName);
        }

        [Test]
        public void ShouldHaveErrorWhenEnquiryIsNullOrEmpty()
        {
            var model = new ContactVendorModel
            {
                Enquiry = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
            model.Enquiry = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Enquiry);
        }

        [Test]
        public void ShouldNotHaveErrorWhenEnquiryIsSpecified()
        {
            var model = new ContactVendorModel
            {
                Enquiry = "please call me back"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Enquiry);
        }
    }
}
