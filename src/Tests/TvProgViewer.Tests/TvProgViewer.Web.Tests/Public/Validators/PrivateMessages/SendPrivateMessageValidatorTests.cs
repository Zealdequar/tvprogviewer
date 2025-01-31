﻿using FluentValidation.TestHelper;
using TvProgViewer.WebUI.Models.PrivateMessages;
using TvProgViewer.WebUI.Validators.PrivateMessages;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.WebUI.Tests.Public.Validators.PrivateMessages
{
    [TestFixture]
    public class SendPrivateMessageValidatorTests : BaseTvProgTest
    {
        private SendPrivateMessageValidator _validator;
        
        [OneTimeSetUp]
        public void Setup()
        {
            _validator = GetService<SendPrivateMessageValidator>();
        }

        [Test]
        public void ShouldHaveErrorWhenSubjectIsNullOrEmpty()
        {
            var model = new SendPrivateMessageModel
            {
                Subject = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
            model.Subject = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Subject);
        }

        [Test]
        public void ShouldNotHaveErrorWhenSubjectIsSpecified()
        {
            var model = new SendPrivateMessageModel
            {
                Subject = "some comment"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Subject);
        }

        [Test]
        public void ShouldHaveErrorWhenMessageIsNullOrEmpty()
        {
            var model = new SendPrivateMessageModel
            {
                Message = null
            };
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
            model.Message = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Message);
        }

        [Test]
        public void ShouldNotHaveErrorWhenMessageIsSpecified()
        {
            var model = new SendPrivateMessageModel
            {
                Message = "some comment"
            };
            _validator.TestValidate(model).ShouldNotHaveValidationErrorFor(x => x.Message);
        }
    }
}
