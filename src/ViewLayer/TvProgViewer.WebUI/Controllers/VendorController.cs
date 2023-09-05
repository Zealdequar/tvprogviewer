﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Html;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Factories;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.WebUI.Models.Vendors;

namespace TvProgViewer.WebUI.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class VendorController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly IUserService _userService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IVendorModelFactory _vendorModelFactory;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public VendorController(CaptchaSettings captchaSettings,
            IUserService userService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorModelFactory vendorModelFactory,
            IVendorService vendorService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            VendorSettings vendorSettings)
        {
            _captchaSettings = captchaSettings;
            _userService = userService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _vendorModelFactory = vendorModelFactory;
            _vendorService = vendorService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdatePictureSeoNamesAsync(Vendor vendor)
        {
            var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(vendor.Name));
        }

        protected virtual async Task<string> ParseVendorAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = await _vendorAttributeService.GetAllVendorAttributesAsync();
            foreach (var attribute in attributes)
            {
                var controlId = $"{TvProgVendorDefaults.VendorAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                )
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await _vendorAttributeService.GetVendorAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported vendor attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> ApplyVendor()
        {
            if (!_vendorSettings.AllowUsersToApplyForVendorAccount)
                return RedirectToRoute("Homepage");

            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            var model = new ApplyVendorModel();
            model = await _vendorModelFactory.PrepareApplyVendorModelAsync(model, true, false, null);
            return View(model);
        }

        [HttpPost, ActionName("ApplyVendor")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> ApplyVendorSubmit(ApplyVendorModel model, bool captchaValid, IFormFile uploadedFile, IFormCollection form)
        {
            if (!_vendorSettings.AllowUsersToApplyForVendorAccount)
                return RedirectToRoute("Homepage");

            var user = await _workContext.GetCurrentUserAsync();
            if (!await _userService.IsRegisteredAsync(user))
                return Challenge();

            if (await _userService.IsAdminAsync(user))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Vendors.ApplyAccount.IsAdmin"));

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnApplyVendorPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            var pictureId = 0;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType.ToLowerInvariant();

                    if(!contentType.StartsWith("image/") || contentType.StartsWith("image/svg"))
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Vendors.ApplyAccount.Picture.ErrorMessage"));
                    else
                    {
                        var vendorPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                        var picture = await _pictureService.InsertPictureAsync(vendorPictureBinary, contentType, null);

                        if (picture != null)
                            pictureId = picture.Id;
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Vendors.ApplyAccount.Picture.ErrorMessage"));
                }
            }

            //vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(form);
            var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
            foreach(var warning in warnings)
            {
                ModelState.AddModelError(string.Empty, warning);
            }

            if (ModelState.IsValid)
            {
                var description = _htmlFormatter.FormatText(model.Description, false, false, true, false, false, false);
                //disabled by default
                var vendor = new Vendor
                {
                    Name = model.Name,
                    Email = model.Email,
                    //some default settings
                    PageSize = 6,
                    AllowUsersToSelectPageSize = true,
                    PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions,
                    PictureId = pictureId,
                    Description = WebUtility.HtmlEncode(description) 
                };
                await _vendorService.InsertVendorAsync(vendor);
                //search engine name (the same as vendor name)
                var seName = await _urlRecordService.ValidateSeNameAsync(vendor, vendor.Name, vendor.Name, true);
                await _urlRecordService.SaveSlugAsync(vendor, seName, 0);

                //associate to the current user
                //but a store owner will have to manually add this user role to "Vendors" role
                //if he wants to grant access to admin area
                user.VendorId = vendor.Id;
                await _userService.UpdateUserAsync(user);

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                //save vendor attributes
                await _genericAttributeService.SaveAttributeAsync(vendor, TvProgVendorDefaults.VendorAttributes, vendorAttributesXml);

                //notify store owner here (email)
                await _workflowMessageService.SendNewVendorAccountApplyStoreOwnerNotificationAsync(user,
                    vendor, _localizationSettings.DefaultAdminLanguageId);

                model.DisableFormInput = true;
                model.Result = await _localizationService.GetResourceAsync("Vendors.ApplyAccount.Submitted");
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            model = await _vendorModelFactory.PrepareApplyVendorModelAsync(model, false, true, vendorAttributesXml);
            return View(model);
        }

        public virtual async Task<IActionResult> Info()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            if (await _workContext.GetCurrentVendorAsync() == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("UserInfo");

            var model = new VendorInfoModel();
            model = await _vendorModelFactory.PrepareVendorInfoModelAsync(model, false);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [FormValueRequired("save-info-button")]
        public virtual async Task<IActionResult> Info(VendorInfoModel model, IFormFile uploadedFile, IFormCollection form)
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            var vendor = await _workContext.GetCurrentVendorAsync();
            if (vendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("UserInfo");

            Picture picture = null;

            if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
            {
                try
                {
                    var contentType = uploadedFile.ContentType.ToLowerInvariant();

                    if (!contentType.StartsWith("image/") || contentType.StartsWith("image/svg"))
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.VendorInfo.Picture.ErrorMessage"));
                    else
                    {
                        var vendorPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                        picture = await _pictureService.InsertPictureAsync(vendorPictureBinary, contentType, null);
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.VendorInfo.Picture.ErrorMessage"));
                }
            }

            var prevPicture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);

            //vendor attributes
            var vendorAttributesXml = await ParseVendorAttributesAsync(form);
            var warnings = (await _vendorAttributeParser.GetAttributeWarningsAsync(vendorAttributesXml)).ToList();
            foreach (var warning in warnings)
            {
                ModelState.AddModelError(string.Empty, warning);
            }

            if (ModelState.IsValid)
            {
                var description = _htmlFormatter.FormatText(model.Description, false, false, true, false, false, false);

                vendor.Name = model.Name;
                vendor.Email = model.Email;
                vendor.Description = WebUtility.HtmlEncode(description);

                if (picture != null)
                {
                    vendor.PictureId = picture.Id;

                    if (prevPicture != null)
                        await _pictureService.DeletePictureAsync(prevPicture);
                }

                //update picture seo file name
                await UpdatePictureSeoNamesAsync(vendor);

                await _vendorService.UpdateVendorAsync(vendor);

                //save vendor attributes
                await _genericAttributeService.SaveAttributeAsync(vendor, TvProgVendorDefaults.VendorAttributes, vendorAttributesXml);

                //notifications
                if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                    await _workflowMessageService.SendVendorInformationChangeStoreOwnerNotificationAsync(vendor, _localizationSettings.DefaultAdminLanguageId);

                return RedirectToAction("Info");
            }

            //If we got this far, something failed, redisplay form
            model = await _vendorModelFactory.PrepareVendorInfoModelAsync(model, true, vendorAttributesXml);
            return View(model);
        }

        [HttpPost, ActionName("Info")]
        [FormValueRequired("remove-picture")]
        public virtual async Task<IActionResult> RemovePicture()
        {
            if (!await _userService.IsRegisteredAsync(await _workContext.GetCurrentUserAsync()))
                return Challenge();

            var vendor = await _workContext.GetCurrentVendorAsync();
            if (vendor == null || !_vendorSettings.AllowVendorsToEditInfo)
                return RedirectToRoute("UserInfo");

            var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);

            if (picture != null)
                await _pictureService.DeletePictureAsync(picture);

            vendor.PictureId = 0;
            await _vendorService.UpdateVendorAsync(vendor);

            //notifications
            if (_vendorSettings.NotifyStoreOwnerAboutVendorInformationChange)
                await _workflowMessageService.SendVendorInformationChangeStoreOwnerNotificationAsync(vendor, _localizationSettings.DefaultAdminLanguageId);

            return RedirectToAction("Info");
        }

        #endregion
    }
}