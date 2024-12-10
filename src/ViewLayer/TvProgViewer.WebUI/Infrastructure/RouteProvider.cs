using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Services.Installation;
using TvProgViewer.Web.Framework.Mvc.Routing;

namespace TvProgViewer.WebUI.Infrastructure
{
    /// <summary>
    /// Represents provider that provided basic routes
    /// </summary>
    public partial class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //get language pattern
            //it's not needed to use language pattern in AJAX requests and for actions returning the result directly (e.g. file to download),
            //use it only for URLs of pages that the user can go to
            var lang = GetLanguageRoutePattern();

            //areas
            endpointRouteBuilder.MapControllerRoute(name: "areaRoute",
                pattern: $"{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}");

            //home page
            endpointRouteBuilder.MapControllerRoute(name: "Homepage",
                pattern: $"{lang}",
                defaults: new { controller = "Home", action = "Index" });

            //login
            endpointRouteBuilder.MapControllerRoute(name: "Login",
                pattern: $"{lang}/login/",
                defaults: new { controller = "User", action = "Login" });

            //teleguideinfo
            endpointRouteBuilder.MapControllerRoute(name: "TeleGuideInfoUpdate",
                pattern: $"{lang}/teleguideinfo-update/",
                defaults: new { controller = "TvGuideInfo", action = "Update" });

            // multi-factor verification digit code page
            endpointRouteBuilder.MapControllerRoute(name: "MultiFactorVerification",
                pattern: $"{lang}/multi-factor-verification/",
                defaults: new { controller = "User", action = "MultiFactorVerification" });

            //register
            endpointRouteBuilder.MapControllerRoute(name: "Register",
                pattern: $"{lang}/register/",
                defaults: new { controller = "User", action = "Register" });

            //logout
            endpointRouteBuilder.MapControllerRoute(name: "Logout",
                pattern: $"{lang}/logout/",
                defaults: new { controller = "User", action = "Logout" });

            //shopping cart
            endpointRouteBuilder.MapControllerRoute(name: "ShoppingCart",
                pattern: $"{lang}/cart/",
                defaults: new { controller = "ShoppingCart", action = "Cart" });

            //estimate shipping (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "EstimateShipping",
                pattern: $"cart/estimateshipping",
                defaults: new { controller = "ShoppingCart", action = "GetEstimateShipping" });

            //wishlist
            endpointRouteBuilder.MapControllerRoute(name: "Wishlist",
                pattern: $"{lang}/wishlist/{{userGuid?}}",
                defaults: new { controller = "ShoppingCart", action = "Wishlist" });

            //user account links
            endpointRouteBuilder.MapControllerRoute(name: "UserInfo",
                pattern: $"{lang}/user/info",
                defaults: new { controller = "User", action = "Info" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAddresses",
                pattern: $"{lang}/user/addresses",
                defaults: new { controller = "User", action = "Addresses" });

            endpointRouteBuilder.MapControllerRoute(name: "UserOrders",
                pattern: $"{lang}/order/history",
                defaults: new { controller = "Order", action = "UserOrders" });

            //contact us
            endpointRouteBuilder.MapControllerRoute(name: "ContactUs",
                pattern: $"{lang}/contactus",
                defaults: new { controller = "Common", action = "ContactUs" });

            //tvChannel search
            endpointRouteBuilder.MapControllerRoute(name: "TvSearch",
                pattern: $"{lang}/search/",
                defaults: new { controller = "Catalog", action = "Search" });

            //autocomplete search term (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TvChannelSearchAutoComplete",
                pattern: $"catalog/searchtermautocomplete",
                defaults: new { controller = "Catalog", action = "SearchTermAutoComplete" });

            //change currency
            endpointRouteBuilder.MapControllerRoute(name: "ChangeCurrency",
                pattern: $"{lang}/changecurrency/{{usercurrency:min(0)}}",
                defaults: new { controller = "Common", action = "SetCurrency" });

            //change language
            endpointRouteBuilder.MapControllerRoute(name: "ChangeLanguage",
                pattern: $"{lang}/changelanguage/{{langid:min(0)}}",
                defaults: new { controller = "Common", action = "SetLanguage" });

            //change tax
            endpointRouteBuilder.MapControllerRoute(name: "ChangeTaxType",
                pattern: $"{lang}/changetaxtype/{{usertaxtype:min(0)}}",
                defaults: new { controller = "Common", action = "SetTaxType" });

            //recently viewed tvChannels
            endpointRouteBuilder.MapControllerRoute(name: "RecentlyViewedTvChannels",
                pattern: $"{lang}/recentlyviewedtvchannels/",
                defaults: new { controller = "TvChannel", action = "RecentlyViewedTvChannels" });

            //new tvChannels
            endpointRouteBuilder.MapControllerRoute(name: "NewTvChannels",
                pattern: $"{lang}/newtvchannels/",
                defaults: new { controller = "Catalog", action = "NewTvChannels" });

            //blog
            endpointRouteBuilder.MapControllerRoute(name: "Blog",
                pattern: $"{lang}/blog",
                defaults: new { controller = "Blog", action = "List" });

            //news
            endpointRouteBuilder.MapControllerRoute(name: "NewsArchive",
                pattern: $"{lang}/news",
                defaults: new { controller = "News", action = "List" });

            //forum
            endpointRouteBuilder.MapControllerRoute(name: "Boards",
                pattern: $"{lang}/boards",
                defaults: new { controller = "Boards", action = "Index" });

            //compare tvChannels
            endpointRouteBuilder.MapControllerRoute(name: "CompareTvChannels",
                pattern: $"{lang}/comparetvchannels/",
                defaults: new { controller = "TvChannel", action = "CompareTvChannels" });

            //tvChannel tags
            endpointRouteBuilder.MapControllerRoute(name: "TvChannelTagsAll",
                pattern: $"{lang}/tvchanneltag/all/",
                defaults: new { controller = "Catalog", action = "TvChannelTagsAll" });

            //manufacturers
            endpointRouteBuilder.MapControllerRoute(name: "ManufacturerList",
                pattern: $"{lang}/manufacturer/all/",
                defaults: new { controller = "Catalog", action = "ManufacturerAll" });

            //vendors
            endpointRouteBuilder.MapControllerRoute(name: "VendorList",
                pattern: $"{lang}/vendor/all/",
                defaults: new { controller = "Catalog", action = "VendorAll" });

            //add tvChannel to cart (without any attributes and options). used on catalog pages. (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "AddTvChannelToCart-Catalog",
                pattern: $"addtvchanneltocart/catalog/{{tvChannelId:min(0)}}/{{shoppingCartTypeId:min(0)}}/{{quantity:min(0)}}",
                defaults: new { controller = "ShoppingCart", action = "AddTvChannelToCart_Catalog" });

            //add tvChannel to cart (with attributes and options). used on the tvChannel details pages. (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "AddTvChannelToCart-Details",
                pattern: $"addtvchanneltocart/details/{{tvChannelId:min(0)}}/{{shoppingCartTypeId:min(0)}}",
                defaults: new { controller = "ShoppingCart", action = "AddTvChannelToCart_Details" });

            //comparing tvChannels (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "AddTvChannelToCompare",
                pattern: $"comparetvchannels/add/{{tvChannelId:min(0)}}",
                defaults: new { controller = "TvChannel", action = "AddTvChannelToCompareList" });

            //tvChannel email a friend
            endpointRouteBuilder.MapControllerRoute(name: "TvChannelEmailAFriend",
                pattern: $"{lang}/tvchannelemailafriend/{{tvChannelId:min(0)}}",
                defaults: new { controller = "TvChannel", action = "TvChannelEmailAFriend" });

            //reviews
            endpointRouteBuilder.MapControllerRoute(name: "TvChannelReviews",
                pattern: $"{lang}/tvchannelreviews/{{tvChannelId}}",
                defaults: new { controller = "TvChannel", action = "TvChannelReviews" });

            endpointRouteBuilder.MapControllerRoute(name: "UserTvChannelReviews",
                pattern: $"{lang}/user/tvchannelreviews",
                defaults: new { controller = "TvChannel", action = "UserTvChannelReviews" });

            endpointRouteBuilder.MapControllerRoute(name: "UserTvChannelReviewsPaged",
                pattern: $"{lang}/user/tvchannelreviews/page/{{pageNumber:min(0)}}",
                defaults: new { controller = "TvChannel", action = "UserTvChannelReviews" });

            //back in stock notifications (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "BackInStockSubscribePopup",
                pattern: $"backinstocksubscribe/{{tvChannelId:min(0)}}",
                defaults: new { controller = "BackInStockSubscription", action = "SubscribePopup" });

            endpointRouteBuilder.MapControllerRoute(name: "BackInStockSubscribeSend",
                pattern: $"backinstocksubscribesend/{{tvChannelId:min(0)}}",
                defaults: new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

            //downloads (file result)
            endpointRouteBuilder.MapControllerRoute(name: "GetSampleDownload",
                pattern: $"download/sample/{{tvchannelid:min(0)}}",
                defaults: new { controller = "Download", action = "Sample" });

            //checkout pages
            endpointRouteBuilder.MapControllerRoute(name: "Checkout",
                pattern: $"{lang}/checkout/",
                defaults: new { controller = "Checkout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutOnePage",
                pattern: $"{lang}/onepagecheckout/",
                defaults: new { controller = "Checkout", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingAddress",
                pattern: $"{lang}/checkout/shippingaddress",
                defaults: new { controller = "Checkout", action = "ShippingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectShippingAddress",
                pattern: $"{lang}/checkout/selectshippingaddress",
                defaults: new { controller = "Checkout", action = "SelectShippingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutBillingAddress",
                pattern: $"{lang}/checkout/billingaddress",
                defaults: new { controller = "Checkout", action = "BillingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutSelectBillingAddress",
                pattern: $"{lang}/checkout/selectbillingaddress",
                defaults: new { controller = "Checkout", action = "SelectBillingAddress" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutShippingMethod",
                pattern: $"{lang}/checkout/shippingmethod",
                defaults: new { controller = "Checkout", action = "ShippingMethod" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentMethod",
                pattern: $"{lang}/checkout/paymentmethod",
                defaults: new { controller = "Checkout", action = "PaymentMethod" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutPaymentInfo",
                pattern: $"{lang}/checkout/paymentinfo",
                defaults: new { controller = "Checkout", action = "PaymentInfo" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutConfirm",
                pattern: $"{lang}/checkout/confirm",
                defaults: new { controller = "Checkout", action = "Confirm" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutCompleted",
                pattern: $"{lang}/checkout/completed/{{orderId:int?}}",
                defaults: new { controller = "Checkout", action = "Completed" });

            //subscribe newsletters (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "SubscribeNewsletter",
                pattern: $"subscribenewsletter",
                defaults: new { controller = "Newsletter", action = "SubscribeNewsletter" });

            //email wishlist
            endpointRouteBuilder.MapControllerRoute(name: "EmailWishlist",
                pattern: $"{lang}/emailwishlist",
                defaults: new { controller = "ShoppingCart", action = "EmailWishlist" });

            //login page for checkout as guest
            endpointRouteBuilder.MapControllerRoute(name: "LoginCheckoutAsGuest",
                pattern: $"{lang}/login/checkoutasguest",
                defaults: new { controller = "User", action = "Login", checkoutAsGuest = true });

            //register result page
            endpointRouteBuilder.MapControllerRoute(name: "RegisterResult",
                pattern: $"{lang}/registerresult/{{resultId:min(0)}}",
                defaults: new { controller = "User", action = "RegisterResult" });

            //check username availability (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "CheckUsernameAvailability",
                pattern: $"user/checkusernameavailability",
                defaults: new { controller = "User", action = "CheckUsernameAvailability" });

            //passwordrecovery
            endpointRouteBuilder.MapControllerRoute(name: "PasswordRecovery",
                pattern: $"{lang}/passwordrecovery",
                defaults: new { controller = "User", action = "PasswordRecovery" });

            //password recovery confirmation
            endpointRouteBuilder.MapControllerRoute(name: "PasswordRecoveryConfirm",
                pattern: $"{lang}/passwordrecovery/confirm",
                defaults: new { controller = "User", action = "PasswordRecoveryConfirm" });

            //topics (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TopicPopup",
                pattern: $"t-popup/{{SystemName}}",
                defaults: new { controller = "Topic", action = "TopicDetailsPopup" });

            //blog
            endpointRouteBuilder.MapControllerRoute(name: "BlogByTag",
                pattern: $"{lang}/blog/tag/{{tag}}",
                defaults: new { controller = "Blog", action = "BlogByTag" });

            endpointRouteBuilder.MapControllerRoute(name: "BlogByMonth",
                pattern: $"{lang}/blog/month/{{month}}",
                defaults: new { controller = "Blog", action = "BlogByMonth" });

            //blog RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "BlogRSS",
                pattern: $"blog/rss/{{languageId:min(0)}}",
                defaults: new { controller = "Blog", action = "ListRss" });

            //news RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "NewsRSS",
                pattern: $"news/rss/{{languageId:min(0)}}",
                defaults: new { controller = "News", action = "ListRss" });

            //set review helpfulness (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "SetTvChannelReviewHelpfulness",
                pattern: $"settvchannelreviewhelpfulness",
                defaults: new { controller = "TvChannel", action = "SetTvChannelReviewHelpfulness" });

            //user account links
            endpointRouteBuilder.MapControllerRoute(name: "UserReturnRequests",
                pattern: $"{lang}/returnrequest/history",
                defaults: new { controller = "ReturnRequest", action = "UserReturnRequests" });

            endpointRouteBuilder.MapControllerRoute(name: "UserDownloadableTvChannels",
                pattern: $"{lang}/user/downloadabletvchannels",
                defaults: new { controller = "User", action = "DownloadableTvChannels" });

            endpointRouteBuilder.MapControllerRoute(name: "UserBackInStockSubscriptions",
                pattern: $"{lang}/backinstocksubscriptions/manage/{{pageNumber:int?}}",
                defaults: new { controller = "BackInStockSubscription", action = "UserSubscriptions" });

            endpointRouteBuilder.MapControllerRoute(name: "UserRewardPoints",
                pattern: $"{lang}/rewardpoints/history",
                defaults: new { controller = "Order", action = "UserRewardPoints" });

            endpointRouteBuilder.MapControllerRoute(name: "UserRewardPointsPaged",
                pattern: $"{lang}/rewardpoints/history/page/{{pageNumber:min(0)}}",
                defaults: new { controller = "Order", action = "UserRewardPoints" });

            endpointRouteBuilder.MapControllerRoute(name: "UserChangePassword",
                pattern: $"{lang}/user/changepassword",
                defaults: new { controller = "User", action = "ChangePassword" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAvatar",
                pattern: $"{lang}/user/avatar",
                defaults: new { controller = "User", action = "Avatar" });

            endpointRouteBuilder.MapControllerRoute(name: "AccountActivation",
                pattern: $"{lang}/user/activation",
                defaults: new { controller = "User", action = "AccountActivation" });

            endpointRouteBuilder.MapControllerRoute(name: "EmailRevalidation",
                pattern: $"{lang}/user/revalidateemail",
                defaults: new { controller = "User", action = "EmailRevalidation" });

            endpointRouteBuilder.MapControllerRoute(name: "UserForumSubscriptions",
                pattern: $"{lang}/boards/forumsubscriptions/{{pageNumber:int?}}",
                defaults: new { controller = "Boards", action = "UserForumSubscriptions" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAddressEdit",
                pattern: $"{lang}/user/addressedit/{{addressId:min(0)}}",
                defaults: new { controller = "User", action = "AddressEdit" });

            endpointRouteBuilder.MapControllerRoute(name: "UserAddressAdd",
                pattern: $"{lang}/user/addressadd",
                defaults: new { controller = "User", action = "AddressAdd" });

            endpointRouteBuilder.MapControllerRoute(name: "UserMultiFactorAuthenticationProviderConfig",
                pattern: $"{lang}/user/providerconfig",
                defaults: new { controller = "User", action = "ConfigureMultiFactorAuthenticationProvider" });

            //user profile page
            endpointRouteBuilder.MapControllerRoute(name: "UserProfile",
                pattern: $"{lang}/profile/{{id:min(0)}}",
                defaults: new { controller = "Profile", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "UserProfilePaged",
                pattern: $"{lang}/profile/{{id:min(0)}}/page/{{pageNumber:min(0)}}",
                defaults: new { controller = "Profile", action = "Index" });

            //orders
            endpointRouteBuilder.MapControllerRoute(name: "OrderDetails",
                pattern: $"{lang}/orderdetails/{{orderId:min(0)}}",
                defaults: new { controller = "Order", action = "Details" });

            endpointRouteBuilder.MapControllerRoute(name: "ShipmentDetails",
                pattern: $"{lang}/orderdetails/shipment/{{shipmentId}}",
                defaults: new { controller = "Order", action = "ShipmentDetails" });

            endpointRouteBuilder.MapControllerRoute(name: "ReturnRequest",
                pattern: $"{lang}/returnrequest/{{orderId:min(0)}}",
                defaults: new { controller = "ReturnRequest", action = "ReturnRequest" });

            endpointRouteBuilder.MapControllerRoute(name: "ReOrder",
                pattern: $"{lang}/reorder/{{orderId:min(0)}}",
                defaults: new { controller = "Order", action = "ReOrder" });

            //pdf invoice (file result)
            endpointRouteBuilder.MapControllerRoute(name: "GetOrderPdfInvoice",
                pattern: $"orderdetails/pdf/{{orderId}}",
                defaults: new { controller = "Order", action = "GetPdfInvoice" });

            endpointRouteBuilder.MapControllerRoute(name: "PrintOrderDetails",
                pattern: $"{lang}/orderdetails/print/{{orderId}}",
                defaults: new { controller = "Order", action = "PrintOrderDetails" });

            //order downloads (file result)
            endpointRouteBuilder.MapControllerRoute(name: "GetDownload",
                pattern: $"download/getdownload/{{orderItemId:guid}}/{{agree?}}",
                defaults: new { controller = "Download", action = "GetDownload" });

            endpointRouteBuilder.MapControllerRoute(name: "GetLicense",
                pattern: $"download/getlicense/{{orderItemId:guid}}/",
                defaults: new { controller = "Download", action = "GetLicense" });

            endpointRouteBuilder.MapControllerRoute(name: "DownloadUserAgreement",
                pattern: $"user/useragreement/{{orderItemId:guid}}",
                defaults: new { controller = "User", action = "UserAgreement" });

            endpointRouteBuilder.MapControllerRoute(name: "GetOrderNoteFile",
                pattern: $"download/ordernotefile/{{ordernoteid:min(0)}}",
                defaults: new { controller = "Download", action = "GetOrderNoteFile" });

            //contact vendor
            endpointRouteBuilder.MapControllerRoute(name: "ContactVendor",
                pattern: $"{lang}/contactvendor/{{vendorId}}",
                defaults: new { controller = "Common", action = "ContactVendor" });

            //apply for vendor account
            endpointRouteBuilder.MapControllerRoute(name: "ApplyVendorAccount",
                pattern: $"{lang}/vendor/apply",
                defaults: new { controller = "Vendor", action = "ApplyVendor" });

            //vendor info
            endpointRouteBuilder.MapControllerRoute(name: "UserVendorInfo",
                pattern: $"{lang}/user/vendorinfo",
                defaults: new { controller = "Vendor", action = "Info" });

            //user GDPR
            endpointRouteBuilder.MapControllerRoute(name: "GdprTools",
                pattern: $"{lang}/user/gdpr",
                defaults: new { controller = "User", action = "GdprTools" });

            //user check gift card balance 
            endpointRouteBuilder.MapControllerRoute(name: "CheckGiftCardBalance",
                pattern: $"{lang}/user/checkgiftcardbalance",
                defaults: new { controller = "User", action = "CheckGiftCardBalance" });

            //user multi-factor authentication settings 
            endpointRouteBuilder.MapControllerRoute(name: "MultiFactorAuthenticationSettings",
                pattern: $"{lang}/user/multifactorauthentication",
                defaults: new { controller = "User", action = "MultiFactorAuthentication" });

            //poll vote (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "PollVote",
                pattern: $"poll/vote",
                defaults: new { controller = "Poll", action = "Vote" });

            //comparing tvChannels
            endpointRouteBuilder.MapControllerRoute(name: "RemoveTvChannelFromCompareList",
                pattern: $"{lang}/comparetvchannels/remove/{{tvChannelId}}",
                defaults: new { controller = "TvChannel", action = "RemoveTvChannelFromCompareList" });

            endpointRouteBuilder.MapControllerRoute(name: "ClearCompareList",
                pattern: $"{lang}/clearcomparelist/",
                defaults: new { controller = "TvChannel", action = "ClearCompareList" });

            //new RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "NewTvChannelsRSS",
                pattern: $"newtvchannels/rss",
                defaults: new { controller = "Catalog", action = "NewTvChannelsRss" });

            //get state list by country ID (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "GetStatesByCountryId",
                pattern: $"country/getstatesbycountryid/",
                defaults: new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "EuCookieLawAccept",
                pattern: $"eucookielawaccept",
                defaults: new { controller = "Common", action = "EuCookieLawAccept" });

            //authenticate topic (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TopicAuthenticate",
                pattern: $"topic/authenticate",
                defaults: new { controller = "Topic", action = "Authenticate" });

            //prepare top menu (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "GetCatalogRoot",
                pattern: $"catalog/getcatalogroot",
                defaults: new { controller = "Catalog", action = "GetCatalogRoot" });

            endpointRouteBuilder.MapControllerRoute(name: "GetCatalogSubCategories",
                pattern: $"catalog/getcatalogsubcategories",
                defaults: new { controller = "Catalog", action = "GetCatalogSubCategories" });

            // Изменение ТВ-провайдера (AJAX ссылка):
            endpointRouteBuilder.MapControllerRoute("ChangeProvider", $"changeprovider/{{userprovider:min(0)}}",
                new { controller = "Common", action = "SetProvider" });

            // Изменение типа ТВ-программы (AJAX ссылка):
            endpointRouteBuilder.MapControllerRoute("ChangeTypeProg", $"changetypeprog/{{usertypeprog:min(0)}}",
                new { controller = "Common", action = "SetTypeProg" });

            // Изменение категории ТВ-программы (AJAX ссылка):
            endpointRouteBuilder.MapControllerRoute("ChangeCategory", $"changecategory/{{usercategory:minlength(0)}}",
                new { controller = "Common", action = "SetCategory" });

            //change language (AJAX link)
            endpointRouteBuilder.MapControllerRoute("ChangeLanguage", $"changelanguage/{{langid:min(0)}}",
                new { controller = "Common", action = "SetLanguage" });

            //Catalog tvChannels (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "GetCategoryTvChannels",
                pattern: $"category/tvchannels/",
                defaults: new { controller = "Catalog", action = "GetCategoryTvChannels" });

            endpointRouteBuilder.MapControllerRoute(name: "GetManufacturerTvChannels",
                pattern: $"manufacturer/tvchannels/",
                defaults: new { controller = "Catalog", action = "GetManufacturerTvChannels" });

            endpointRouteBuilder.MapControllerRoute(name: "GetTagTvChannels",
                pattern: $"tag/tvchannels",
                defaults: new { controller = "Catalog", action = "GetTagTvChannels" });

            endpointRouteBuilder.MapControllerRoute(name: "SearchTvChannels",
                pattern: $"tvChannel/search",
                defaults: new { controller = "Catalog", action = "SearchTvChannels" });

            endpointRouteBuilder.MapControllerRoute(name: "GetVendorTvChannels",
                pattern: $"vendor/tvchannels",
                defaults: new { controller = "Catalog", action = "GetVendorTvChannels" });

            endpointRouteBuilder.MapControllerRoute(name: "GetNewTvChannels",
                pattern: $"newtvchannels/tvchannels/",
                defaults: new { controller = "Catalog", action = "GetNewTvChannels" });

            //tvChannel combinations (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "GetTvChannelCombinations",
                pattern: $"tvchannel/combinations",
                defaults: new { controller = "TvChannel", action = "GetTvChannelCombinations" });

            //tvChannel attributes with "upload file" type (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "UploadFileTvChannelAttribute",
                pattern: $"uploadfiletvchannelattribute/{{attributeId:min(0)}}",
                defaults: new { controller = "ShoppingCart", action = "UploadFileTvChannelAttribute" });

            //checkout attributes with "upload file" type (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "UploadFileCheckoutAttribute",
                pattern: $"uploadfilecheckoutattribute/{{attributeId:min(0)}}",
                defaults: new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

            //return request with "upload file" support (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "UploadFileReturnRequest",
                pattern: $"uploadfilereturnrequest",
                defaults: new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

            //forums
            endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussions",
                pattern: $"{lang}/boards/activediscussions",
                defaults: new { controller = "Boards", action = "ActiveDiscussions" });

            endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussionsPaged",
                pattern: $"{lang}/boards/activediscussions/page/{{pageNumber:int}}",
                defaults: new { controller = "Boards", action = "ActiveDiscussions" });

            //forums RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "ActiveDiscussionsRSS",
                pattern: $"boards/activediscussionsrss",
                defaults: new { controller = "Boards", action = "ActiveDiscussionsRSS" });

            endpointRouteBuilder.MapControllerRoute(name: "PostEdit",
                pattern: $"{lang}/boards/postedit/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "PostEdit" });

            endpointRouteBuilder.MapControllerRoute(name: "PostDelete",
                pattern: $"{lang}/boards/postdelete/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "PostDelete" });

            endpointRouteBuilder.MapControllerRoute(name: "PostCreate",
                pattern: $"{lang}/boards/postcreate/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "PostCreate" });

            endpointRouteBuilder.MapControllerRoute(name: "PostCreateQuote",
                pattern: $"{lang}/boards/postcreate/{{id:min(0)}}/{{quote:min(0)}}",
                defaults: new { controller = "Boards", action = "PostCreate" });

            endpointRouteBuilder.MapControllerRoute(name: "TopicEdit",
                pattern: $"{lang}/boards/topicedit/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "TopicEdit" });

            endpointRouteBuilder.MapControllerRoute(name: "TopicDelete",
                pattern: $"{lang}/boards/topicdelete/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "TopicDelete" });

            endpointRouteBuilder.MapControllerRoute(name: "TopicCreate",
                pattern: $"{lang}/boards/topiccreate/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "TopicCreate" });

            endpointRouteBuilder.MapControllerRoute(name: "TopicMove",
                pattern: $"{lang}/boards/topicmove/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "TopicMove" });

            //topic watch (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "TopicWatch",
                pattern: $"boards/topicwatch/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "TopicWatch" });

            endpointRouteBuilder.MapControllerRoute(name: "TopicSlug",
                pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}",
                defaults: new { controller = "Boards", action = "Topic" });

            endpointRouteBuilder.MapControllerRoute(name: "TopicSlugPaged",
                pattern: $"{lang}/boards/topic/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
                defaults: new { controller = "Boards", action = "Topic" });

            //forum watch (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "ForumWatch",
                pattern: $"boards/forumwatch/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "ForumWatch" });

            //forums RSS (file result)
            endpointRouteBuilder.MapControllerRoute(name: "ForumRSS",
                pattern: $"boards/forumrss/{{id:min(0)}}",
                defaults: new { controller = "Boards", action = "ForumRSS" });

            endpointRouteBuilder.MapControllerRoute(name: "ForumSlug",
                pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}",
                defaults: new { controller = "Boards", action = "Forum" });

            endpointRouteBuilder.MapControllerRoute(name: "ForumSlugPaged",
                pattern: $"{lang}/boards/forum/{{id:min(0)}}/{{slug?}}/page/{{pageNumber:int}}",
                defaults: new { controller = "Boards", action = "Forum" });

            endpointRouteBuilder.MapControllerRoute(name: "ForumGroupSlug",
                pattern: $"{lang}/boards/forumgroup/{{id:min(0)}}/{{slug?}}",
                defaults: new { controller = "Boards", action = "ForumGroup" });

            endpointRouteBuilder.MapControllerRoute(name: "Search",
                pattern: $"{lang}/boards/search",
                defaults: new { controller = "Boards", action = "Search" });

            //private messages
            endpointRouteBuilder.MapControllerRoute(name: "PrivateMessages",
                pattern: $"{lang}/privatemessages/{{tab?}}",
                defaults: new { controller = "PrivateMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesPaged",
                pattern: $"{lang}/privatemessages/{{tab?}}/page/{{pageNumber:min(0)}}",
                defaults: new { controller = "PrivateMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesInbox",
                pattern: $"{lang}/inboxupdate",
                defaults: new { controller = "PrivateMessages", action = "InboxUpdate" });

            endpointRouteBuilder.MapControllerRoute(name: "PrivateMessagesSent",
                pattern: $"{lang}/sentupdate",
                defaults: new { controller = "PrivateMessages", action = "SentUpdate" });

            endpointRouteBuilder.MapControllerRoute(name: "SendPM",
                pattern: $"{lang}/sendpm/{{toUserId:min(0)}}",
                defaults: new { controller = "PrivateMessages", action = "SendPM" });

            endpointRouteBuilder.MapControllerRoute(name: "SendPMReply",
                pattern: $"{lang}/sendpm/{{toUserId:min(0)}}/{{replyToMessageId:min(0)}}",
                defaults: new { controller = "PrivateMessages", action = "SendPM" });

            endpointRouteBuilder.MapControllerRoute(name: "ViewPM",
                pattern: $"{lang}/viewpm/{{privateMessageId:min(0)}}",
                defaults: new { controller = "PrivateMessages", action = "ViewPM" });

            endpointRouteBuilder.MapControllerRoute(name: "DeletePM",
                pattern: $"{lang}/deletepm/{{privateMessageId:min(0)}}",
                defaults: new { controller = "PrivateMessages", action = "DeletePM" });

            //activate newsletters
            endpointRouteBuilder.MapControllerRoute(name: "NewsletterActivation",
                pattern: $"{lang}/newsletter/subscriptionactivation/{{token:guid}}/{{active}}",
                defaults: new { controller = "Newsletter", action = "SubscriptionActivation" });

            //robots.txt (file result)
            endpointRouteBuilder.MapControllerRoute(name: "robots.txt",
                pattern: $"robots.txt",
                defaults: new { controller = "Common", action = "RobotsTextFile" });

            //sitemap
            endpointRouteBuilder.MapControllerRoute(name: "Sitemap",
                pattern: $"{lang}/sitemap",
                defaults: new { controller = "Common", action = "Sitemap" });

            //sitemap.xml (file result)
            endpointRouteBuilder.MapControllerRoute(name: "sitemap.xml",
                pattern: $"sitemap.xml",
                defaults: new { controller = "Common", action = "SitemapXml" });

            endpointRouteBuilder.MapControllerRoute(name: "sitemap-indexed.xml",
                pattern: $"sitemap-{{Id:min(0)}}.xml",
                defaults: new { controller = "Common", action = "SitemapXml" });

            //store closed
            endpointRouteBuilder.MapControllerRoute(name: "StoreClosed",
                pattern: $"{lang}/storeclosed",
                defaults: new { controller = "Common", action = "StoreClosed" });

            //install
            endpointRouteBuilder.MapControllerRoute(name: "Installation",
                pattern: $"{TvProgInstallationDefaults.InstallPath}",
                defaults: new { controller = "Install", action = "Index" });

            //error page
            endpointRouteBuilder.MapControllerRoute(name: "Error",
                pattern: $"error",
                defaults: new { controller = "Common", action = "Error" });

            //page not found
            endpointRouteBuilder.MapControllerRoute(name: "PageNotFound",
                pattern: $"{lang}/page-not-found",
                defaults: new { controller = "Common", action = "PageNotFound" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;

        #endregion
    }
}