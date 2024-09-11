using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.WebUI.Controllers
{
    public partial class DownloadController : BasePublicController
    {
        private readonly UserSettings _userSettings;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IWorkContext _workContext;

        public DownloadController(UserSettings userSettings,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IOrderService orderService,
            ITvChannelService tvChannelService,
            IWorkContext workContext)
        {
            _userSettings = userSettings;
            _downloadService = downloadService;
            _localizationService = localizationService;
            _orderService = orderService;
            _tvChannelService = tvChannelService;
            _workContext = workContext;
        }
        
        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> Sample(int tvChannelId)
        {
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelId);
            if (tvChannel == null)
                return InvokeHttp404();

            if (!tvChannel.HasSampleDownload)
                return Content("TvChannel doesn't have a sample download.");

            var download = await _downloadService.GetDownloadByIdAsync(tvChannel.SampleDownloadId);
            if (download == null)
                return Content("Sample download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");
            
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : tvChannel.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension }; 
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetDownload(Guid orderItemId, bool agree = false)
        {
            var orderItem = await _orderService.GetOrderItemByGuidAsync(orderItemId);
            if (orderItem == null)
                return InvokeHttp404();

            var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);
            
            if (!await _orderService.IsDownloadAllowedAsync(orderItem))
                return Content("Downloads are not allowed");

            if (_userSettings.DownloadableTvChannelsValidateUser)
            {
                var user = await _workContext.GetCurrentUserAsync();
                if (user == null)
                    return Challenge();

                if (order.UserId != user.Id)
                    return Content("This is not your order");
            }

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(orderItem.TvChannelId);

            var download = await _downloadService.GetDownloadByIdAsync(tvChannel.DownloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (tvChannel.HasUserAgreement && !agree)
                return RedirectToRoute("DownloadUserAgreement", new { orderItemId = orderItemId });


            if (!tvChannel.UnlimitedDownloads && orderItem.DownloadCount >= tvChannel.MaxNumberOfDownloads)
                return Content(string.Format(await _localizationService.GetResourceAsync("DownloadableTvChannels.ReachedMaximumNumber"), tvChannel.MaxNumberOfDownloads));
           
            if (download.UseDownloadUrl)
            {
                //increase download
                orderItem.DownloadCount++;
                await _orderService.UpdateOrderItemAsync(orderItem);

                //return result
                //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
                //In this case, it is not relevant. Url may not be local.
                return new RedirectResult(download.DownloadUrl);
            }
            
            //binary download
            if (download.DownloadBinary == null)
                    return Content("Download data is not available any more.");

            //increase download
            orderItem.DownloadCount++;
            await _orderService.UpdateOrderItemAsync(orderItem);

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : tvChannel.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };  
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetLicense(Guid orderItemId)
        {
            var orderItem = await _orderService.GetOrderItemByGuidAsync(orderItemId);
            if (orderItem == null)
                return InvokeHttp404();

            var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);

            if (!await _orderService.IsLicenseDownloadAllowedAsync(orderItem))
                return Content("Downloads are not allowed");

            if (_userSettings.DownloadableTvChannelsValidateUser)
            {
                var user = await _workContext.GetCurrentUserAsync();
                if (user == null || order.UserId != user.Id)
                    return Challenge();
            }

            var download = await _downloadService.GetDownloadByIdAsync(orderItem.LicenseDownloadId ?? 0);
            if (download == null)
                return Content("Download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : orderItem.TvChannelId.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        public virtual async Task<IActionResult> GetFileUpload(Guid downloadId)
        {
            var download = await _downloadService.GetDownloadByGuidAsync(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(ignore: true)]
        public virtual async Task<IActionResult> GetOrderNoteFile(int orderNoteId)
        {
            var orderNote = await _orderService.GetOrderNoteByIdAsync(orderNoteId);
            if (orderNote == null)
                return InvokeHttp404();

            var order = await _orderService.GetOrderByIdAsync(orderNote.OrderId);
            var user = await _workContext.GetCurrentUserAsync();
            if (user == null || order.UserId != user.Id)
                return Challenge();

            var download = await _downloadService.GetDownloadByIdAsync(orderNote.DownloadId);
            if (download == null)
                return Content("Download is not available any more.");
            
            //A warning (SCS0027 - Open Redirect) from the "Security Code Scan" analyzer may appear at this point. 
            //In this case, it is not relevant. Url may not be local.
            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : orderNote.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }
    }
}