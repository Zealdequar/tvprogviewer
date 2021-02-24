using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TVProgViewer.Services.Installation;
using TVProgViewer.Web.Framework.Mvc.Routing;

namespace TVProgViewer.WebUI.Infrastructure
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
            var pattern = GetRouterPattern(endpointRouteBuilder);

            //areas
            endpointRouteBuilder.MapControllerRoute(name: "areaRoute", 
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //home page
            endpointRouteBuilder.MapControllerRoute("Homepage", pattern,
				new { controller = "Home", action = "Index" });

            //login
            endpointRouteBuilder.MapControllerRoute("Login", $"{pattern}login/",
                new { controller = "User", action = "Login" });

            // multi-factor verification digit code page
            endpointRouteBuilder.MapControllerRoute("MultiFactorVerification", "multi-factor-verification",
                            new { controller = "User", action = "MultiFactorVerification" });

            //register
            endpointRouteBuilder.MapControllerRoute("Register", $"{pattern}register/",
                new { controller = "User", action = "Register" });

            //logout
            endpointRouteBuilder.MapControllerRoute("Logout", $"{pattern}logout/",
                new { controller = "User", action = "Logout" });

            //shopping cart
            endpointRouteBuilder.MapControllerRoute("ShoppingCart", $"{pattern}cart/",
				new { controller = "ShoppingCart", action = "Cart" });            

            //estimate shipping
            endpointRouteBuilder.MapControllerRoute("EstimateShipping", $"{pattern}cart/estimateshipping",
				new {controller = "ShoppingCart", action = "GetEstimateShipping"});

            //wishlist
            endpointRouteBuilder.MapControllerRoute("Wishlist", pattern + "wishlist/{userGuid?}",
				new { controller = "ShoppingCart", action = "Wishlist"});

            //user account links
            endpointRouteBuilder.MapControllerRoute("UserInfo", $"{pattern}user/info",
                new { controller = "User", action = "Info" });

            endpointRouteBuilder.MapControllerRoute("UserAddresses", $"{pattern}user/addresses",
                new { controller = "User", action = "Addresses" });

            endpointRouteBuilder.MapControllerRoute("UserOrders", $"{pattern}order/history",
                new { controller = "Order", action = "UserOrders" });

            //contact us
            endpointRouteBuilder.MapControllerRoute("ContactUs", $"{pattern}contactus",
				new { controller = "Common", action = "ContactUs" });

            //sitemap
            endpointRouteBuilder.MapControllerRoute("Sitemap", $"{pattern}sitemap",
				new { controller = "Common", action = "Sitemap" });

            //product search
            endpointRouteBuilder.MapControllerRoute("ProductSearch", $"{pattern}search/",
				new { controller = "Catalog", action = "Search" });                     

            endpointRouteBuilder.MapControllerRoute("ProductSearchAutoComplete", $"{pattern}catalog/searchtermautocomplete",
				new { controller = "Catalog", action = "SearchTermAutoComplete" });

            //change currency (AJAX link)
            endpointRouteBuilder.MapControllerRoute("ChangeCurrency", pattern + "changecurrency/{usercurrency:min(0)}",
				new { controller = "Common", action = "SetCurrency" });

            // Изменение ТВ-провайдера (AJAX ссылка):
            endpointRouteBuilder.MapControllerRoute("ChangeProvider", pattern + "changeprovider/{userprovider:min(0)}",
                new { controller = "Common", action = "SetProvider" });

            // Изменение типа ТВ-программы (AJAX ссылка):
            endpointRouteBuilder.MapControllerRoute("ChangeTypeProg", pattern + "changetypeprog/{usertypeprog:min(0)}",
                new { controller = "Common", action = "SetTypeProg" });

            // Изменение категории ТВ-программы (AJAX ссылка):
            endpointRouteBuilder.MapControllerRoute("ChangeCategory", pattern + "changecategory/{usercategory:minlength(0)}",
                new { controller = "Common", action = "SetCategory" });

            //change language (AJAX link)
            endpointRouteBuilder.MapControllerRoute("ChangeLanguage", pattern + "changelanguage/{langid:min(0)}",
				new { controller = "Common", action = "SetLanguage" });

            //change tax (AJAX link)
            endpointRouteBuilder.MapControllerRoute("ChangeTaxType", pattern + "changetaxtype/{usertaxtype:min(0)}",
				new { controller = "Common", action = "SetTaxType" });

            //recently viewed products
            endpointRouteBuilder.MapControllerRoute("RecentlyViewedProducts", $"{pattern}recentlyviewedproducts/",
				new { controller = "Product", action = "RecentlyViewedProducts" });

            //new products
            endpointRouteBuilder.MapControllerRoute("NewProducts", $"{pattern}newproducts/",
				new { controller = "Product", action = "NewProducts" });

            //blog
            endpointRouteBuilder.MapControllerRoute("Blog", $"{pattern}blog",
				new { controller = "Blog", action = "List" });

            //news
            endpointRouteBuilder.MapControllerRoute("NewsArchive", $"{pattern}news",
				new { controller = "News", action = "List" });

            //forum
            endpointRouteBuilder.MapControllerRoute("Boards", $"{pattern}boards",
				new { controller = "Boards", action = "Index" });

            //compare products
            endpointRouteBuilder.MapControllerRoute("CompareProducts", $"{pattern}compareproducts/",
				new { controller = "Product", action = "CompareProducts" });

            //product tags
            endpointRouteBuilder.MapControllerRoute("ProductTagsAll", $"{pattern}producttag/all/",
				new { controller = "Catalog", action = "ProductTagsAll" });

            //manufacturers
            endpointRouteBuilder.MapControllerRoute("ManufacturerList", $"{pattern}manufacturer/all/",
				new { controller = "Catalog", action = "ManufacturerAll" });

            //vendors
            endpointRouteBuilder.MapControllerRoute("VendorList", $"{pattern}vendor/all/",
				new { controller = "Catalog", action = "VendorAll" });

            //add product to cart (without any attributes and options). used on catalog pages.
            endpointRouteBuilder.MapControllerRoute("AddProductToCart-Catalog", 
                pattern + "addproducttocart/catalog/{productId:min(0)}/{shoppingCartTypeId:min(0)}/{quantity:min(0)}",
				new { controller = "ShoppingCart", action = "AddProductToCart_Catalog" });

            //add product to cart (with attributes and options). used on the product details pages.
            endpointRouteBuilder.MapControllerRoute("AddProductToCart-Details", 
                pattern + "addproducttocart/details/{productId:min(0)}/{shoppingCartTypeId:min(0)}",
				new { controller = "ShoppingCart", action = "AddProductToCart_Details" });

            //comparing products
            endpointRouteBuilder.MapControllerRoute("AddProductToCompare", "compareproducts/add/{productId:min(0)}",
				new { controller = "Product", action = "AddProductToCompareList" });

            //product email a friend
            endpointRouteBuilder.MapControllerRoute("ProductEmailAFriend", 
                pattern + "productemailafriend/{productId:min(0)}",
				new { controller = "Product", action = "ProductEmailAFriend" });

            //reviews
            endpointRouteBuilder.MapControllerRoute("ProductReviews2", pattern + "productreviews/{productId2}",
                new { controller = "Product", action = "ProductReviews2" });

            endpointRouteBuilder.MapControllerRoute("ProductReviews", pattern + "productreviews/{productId}",
				new { controller = "Product", action = "ProductReviews" });

            endpointRouteBuilder.MapControllerRoute("UserProductReviews", $"{pattern}user/productreviews",
				new { controller = "Product", action = "UserProductReviews" });

            endpointRouteBuilder.MapControllerRoute("UserProductReviewsPaged",
                pattern + "user/productreviews/page/{pageNumber:min(0)}",
                new { controller = "Product", action = "UserProductReviews" });

            //back in stock notifications
            endpointRouteBuilder.MapControllerRoute("BackInStockSubscribePopup", 
                pattern + "backinstocksubscribe/{productId:min(0)}",
				new { controller = "BackInStockSubscription", action = "SubscribePopup" });

            endpointRouteBuilder.MapControllerRoute("BackInStockSubscribeSend", 
                pattern + "backinstocksubscribesend/{productId:min(0)}",
				new { controller = "BackInStockSubscription", action = "SubscribePopupPOST" });

            //downloads
            endpointRouteBuilder.MapControllerRoute("GetSampleDownload", 
                pattern + "download/sample/{productid:min(0)}",
				new { controller = "Download", action = "Sample" });

            //checkout pages
            endpointRouteBuilder.MapControllerRoute("Checkout", $"{pattern}checkout/",
				new { controller = "Checkout", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("CheckoutOnePage", $"{pattern}onepagecheckout/",
				new { controller = "Checkout", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute("CheckoutShippingAddress", $"{pattern}checkout/shippingaddress",
				new { controller = "Checkout", action = "ShippingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutSelectShippingAddress", $"{pattern}checkout/selectshippingaddress",
				new { controller = "Checkout", action = "SelectShippingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutBillingAddress", $"{pattern}checkout/billingaddress",
				new { controller = "Checkout", action = "BillingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutSelectBillingAddress", $"{pattern}checkout/selectbillingaddress",
				new { controller = "Checkout", action = "SelectBillingAddress" });

            endpointRouteBuilder.MapControllerRoute("CheckoutShippingMethod", $"{pattern}checkout/shippingmethod",
				new { controller = "Checkout", action = "ShippingMethod" });

            endpointRouteBuilder.MapControllerRoute("CheckoutPaymentMethod", $"{pattern}checkout/paymentmethod",
				new { controller = "Checkout", action = "PaymentMethod" });

            endpointRouteBuilder.MapControllerRoute("CheckoutPaymentInfo", $"{pattern}checkout/paymentinfo",
				new { controller = "Checkout", action = "PaymentInfo" });

            endpointRouteBuilder.MapControllerRoute("CheckoutConfirm", $"{pattern}checkout/confirm",
				new { controller = "Checkout", action = "Confirm" });

            endpointRouteBuilder.MapControllerRoute("CheckoutCompleted", 
                pattern + "checkout/completed/{orderId:int}",
                new { controller = "Checkout", action = "Completed" });

            //subscribe newsletters
            endpointRouteBuilder.MapControllerRoute("SubscribeNewsletter", $"{pattern}subscribenewsletter",
				new { controller = "Newsletter", action = "SubscribeNewsletter" });

            //email wishlist
            endpointRouteBuilder.MapControllerRoute("EmailWishlist", $"{pattern}emailwishlist",
				new { controller = "ShoppingCart", action = "EmailWishlist" });

            //login page for checkout as guest
            endpointRouteBuilder.MapControllerRoute("LoginCheckoutAsGuest", $"{pattern}login/checkoutasguest",
                new { controller = "User", action = "Login", checkoutAsGuest = true });

            //register result page
            endpointRouteBuilder.MapControllerRoute("RegisterResult", 
                pattern + "registerresult/{resultId:min(0)}",
                new { controller = "User", action = "RegisterResult" });

            //check username availability
            endpointRouteBuilder.MapControllerRoute("CheckUsernameAvailability", $"{pattern}user/checkusernameavailability",
                new { controller = "User", action = "CheckUsernameAvailability" });

            //passwordrecovery
            endpointRouteBuilder.MapControllerRoute("PasswordRecovery", $"{pattern}passwordrecovery",
                new { controller = "User", action = "PasswordRecovery" });

            //password recovery confirmation
            endpointRouteBuilder.MapControllerRoute("PasswordRecoveryConfirm", $"{pattern}passwordrecovery/confirm",
                new { controller = "User", action = "PasswordRecoveryConfirm" });

            //topics
            endpointRouteBuilder.MapControllerRoute("TopicPopup", 
                pattern + "t-popup/{SystemName}",
				new { controller = "Topic", action = "TopicDetailsPopup" });
            
            //blog
            endpointRouteBuilder.MapControllerRoute("BlogByTag", 
                pattern + "blog/tag/{tag}",
				new { controller = "Blog", action = "BlogByTag" });

            endpointRouteBuilder.MapControllerRoute("BlogByMonth", 
                pattern + "blog/month/{month}",
				new { controller = "Blog", action = "BlogByMonth" });

            //blog RSS
            endpointRouteBuilder.MapControllerRoute("BlogRSS", 
                pattern + "blog/rss/{languageId:min(0)}",
				new { controller = "Blog", action = "ListRss" });

            //news RSS
            endpointRouteBuilder.MapControllerRoute("NewsRSS", 
                pattern + "news/rss/{languageId:min(0)}",
				new { controller = "News", action = "ListRss" });

            //set review helpfulness (AJAX link)
            endpointRouteBuilder.MapControllerRoute("SetProductReviewHelpfulness", $"{pattern}setproductreviewhelpfulness",
				new { controller = "Product", action = "SetProductReviewHelpfulness" });

            //user account links
            endpointRouteBuilder.MapControllerRoute("UserReturnRequests", $"{pattern}returnrequest/history",
				new { controller = "ReturnRequest", action = "UserReturnRequests" });

            endpointRouteBuilder.MapControllerRoute("UserDownloadableProducts", $"{pattern}user/downloadableproducts",
                new { controller = "User", action = "DownloadableProducts" });

            endpointRouteBuilder.MapControllerRoute("UserBackInStockSubscriptions",
                pattern + "backinstocksubscriptions/manage/{pageNumber:int?}",
                new { controller = "BackInStockSubscription", action = "UserSubscriptions" });

            endpointRouteBuilder.MapControllerRoute("UserRewardPoints", $"{pattern}rewardpoints/history",
                new { controller = "Order", action = "UserRewardPoints" });

            endpointRouteBuilder.MapControllerRoute("UserRewardPointsPaged",
                pattern + "rewardpoints/history/page/{pageNumber:min(0)}",
                new { controller = "Order", action = "UserRewardPoints" });

            endpointRouteBuilder.MapControllerRoute("UserChangePassword", $"{pattern}user/changepassword",
                new { controller = "User", action = "ChangePassword" });

            endpointRouteBuilder.MapControllerRoute("UserAvatar", $"{pattern}user/avatar",
                new { controller = "User", action = "Avatar" });

            endpointRouteBuilder.MapControllerRoute("AccountActivation", $"{pattern}user/activation",
                new { controller = "User", action = "AccountActivation" });

            endpointRouteBuilder.MapControllerRoute("EmailRevalidation", $"{pattern}user/revalidateemail",
                new { controller = "User", action = "EmailRevalidation" });

            endpointRouteBuilder.MapControllerRoute("UserForumSubscriptions",
                pattern + "boards/forumsubscriptions/{pageNumber:int?}",
                new { controller = "Boards", action = "UserForumSubscriptions" });

            endpointRouteBuilder.MapControllerRoute("UserAddressEdit",
                pattern + "user/addressedit/{addressId:min(0)}",
                new { controller = "User", action = "AddressEdit" });

            endpointRouteBuilder.MapControllerRoute("UserAddressAdd", $"{pattern}user/addressadd",
                new { controller = "User", action = "AddressAdd" });

            endpointRouteBuilder.MapControllerRoute("UserMultiFactorAuthenticationProviderConfig", $"{pattern}user/providerconfig",
                new { controller = "User", action = "ConfigureMultiFactorAuthenticationProvider" });

            //user profile page
            endpointRouteBuilder.MapControllerRoute("UserProfile",
                pattern + "profile/{id:min(0)}",
				new { controller = "Profile", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("UserProfilePaged",
                pattern + "profile/{id:min(0)}/page/{pageNumber:min(0)}",
				new { controller = "Profile", action = "Index" });

            //orders
            endpointRouteBuilder.MapControllerRoute("OrderDetails", 
                pattern + "orderdetails/{orderId:min(0)}",
				new { controller = "Order", action = "Details" });

            endpointRouteBuilder.MapControllerRoute("ShipmentDetails", 
                pattern + "orderdetails/shipment/{shipmentId}",
				new { controller = "Order", action = "ShipmentDetails" });

            endpointRouteBuilder.MapControllerRoute("ReturnRequest", 
                pattern + "returnrequest/{orderId:min(0)}",
				new { controller = "ReturnRequest", action = "ReturnRequest" });

            endpointRouteBuilder.MapControllerRoute("ReOrder", 
                pattern + "reorder/{orderId:min(0)}",
				new { controller = "Order", action = "ReOrder" });

            endpointRouteBuilder.MapControllerRoute("GetOrderPdfInvoice", 
                pattern + "orderdetails/pdf/{orderId}",
				new { controller = "Order", action = "GetPdfInvoice" });

            endpointRouteBuilder.MapControllerRoute("PrintOrderDetails", 
                pattern + "orderdetails/print/{orderId}",
				new { controller = "Order", action = "PrintOrderDetails" });

            //order downloads
            endpointRouteBuilder.MapControllerRoute("GetDownload", 
                pattern + "download/getdownload/{orderItemId:guid}/{agree?}",
				new { controller = "Download", action = "GetDownload" });

            endpointRouteBuilder.MapControllerRoute("GetLicense", 
                pattern + "download/getlicense/{orderItemId:guid}/",
				new { controller = "Download", action = "GetLicense" });

            endpointRouteBuilder.MapControllerRoute("DownloadUserAgreement", 
                pattern + "user/useragreement/{orderItemId:guid}",
                new { controller = "User", action = "UserAgreement" });

            endpointRouteBuilder.MapControllerRoute("GetOrderNoteFile", 
                pattern + "download/ordernotefile/{ordernoteid:min(0)}",
				new { controller = "Download", action = "GetOrderNoteFile" });

            //contact vendor
            endpointRouteBuilder.MapControllerRoute("ContactVendor", 
                pattern + "contactvendor/{vendorId}",
				new { controller = "Common", action = "ContactVendor" });

            //apply for vendor account
            endpointRouteBuilder.MapControllerRoute("ApplyVendorAccount", $"{pattern}vendor/apply",
				new { controller = "Vendor", action = "ApplyVendor" });

            //vendor info
            endpointRouteBuilder.MapControllerRoute("UserVendorInfo", $"{pattern}user/vendorinfo",
				new { controller = "Vendor", action = "Info" });

            //user GDPR
            endpointRouteBuilder.MapControllerRoute("GdprTools", $"{pattern}user/gdpr",
                new { controller = "User", action = "GdprTools" });

            //user check gift card balance 
            endpointRouteBuilder.MapControllerRoute("CheckGiftCardBalance", $"{pattern}user/checkgiftcardbalance",
                new { controller = "User", action = "CheckGiftCardBalance" });

            //user multi-factor authentication settings 
            endpointRouteBuilder.MapControllerRoute("MultiFactorAuthenticationSettings", $"{pattern}user/multifactorauthentication",
                new { controller = "User", action = "MultiFactorAuthentication" });

            //poll vote AJAX link
            endpointRouteBuilder.MapControllerRoute("PollVote", "poll/vote",
				new { controller = "Poll", action = "Vote" });

            //comparing products
            endpointRouteBuilder.MapControllerRoute("RemoveProductFromCompareList", 
                pattern + "compareproducts/remove/{productId}",
				new { controller = "Product", action = "RemoveProductFromCompareList" });

            endpointRouteBuilder.MapControllerRoute("ClearCompareList", $"{pattern}clearcomparelist/",
				new { controller = "Product", action = "ClearCompareList" });

            //new RSS
            endpointRouteBuilder.MapControllerRoute("NewProductsRSS", $"{pattern}newproducts/rss",
				new { controller = "Product", action = "NewProductsRss" });
            
            //get state list by country ID  (AJAX link)
            endpointRouteBuilder.MapControllerRoute("GetStatesByCountryId", $"{pattern}country/getstatesbycountryid/",
				new { controller = "Country", action = "GetStatesByCountryId" });

            //EU Cookie law accept button handler (AJAX link)
            endpointRouteBuilder.MapControllerRoute("EuCookieLawAccept", $"{pattern}eucookielawaccept",
				new { controller = "Common", action = "EuCookieLawAccept" });

            //authenticate topic AJAX link
            endpointRouteBuilder.MapControllerRoute("TopicAuthenticate", $"{pattern}topic/authenticate",
                new { controller = "Topic", action = "Authenticate" });

            //prepare top menu (AJAX link)
            endpointRouteBuilder.MapControllerRoute("GetCatalogRoot", $"{pattern}catalog/getcatalogroot",
                new { controller = "Catalog", action = "GetCatalogRoot" });

            endpointRouteBuilder.MapControllerRoute("GetCatalogSubCategories", $"{pattern}catalog/getcatalogsubcategories",
                new { controller = "Catalog", action = "GetCatalogSubCategories" });

            //product attributes with "upload file" type
            endpointRouteBuilder.MapControllerRoute("UploadFileProductAttribute", 
                pattern + "uploadfileproductattribute/{attributeId:min(0)}",
				new { controller = "ShoppingCart", action = "UploadFileProductAttribute" });

            //checkout attributes with "upload file" type
            endpointRouteBuilder.MapControllerRoute("UploadFileCheckoutAttribute", 
                pattern + "uploadfilecheckoutattribute/{attributeId:min(0)}",
				new { controller = "ShoppingCart", action = "UploadFileCheckoutAttribute" });

            //return request with "upload file" support
            endpointRouteBuilder.MapControllerRoute("UploadFileReturnRequest", $"{pattern}uploadfilereturnrequest",
				new { controller = "ReturnRequest", action = "UploadFileReturnRequest" });

            //forums
            endpointRouteBuilder.MapControllerRoute("ActiveDiscussions", $"{pattern}boards/activediscussions",
				new { controller = "Boards", action = "ActiveDiscussions" });

            endpointRouteBuilder.MapControllerRoute("ActiveDiscussionsPaged", 
                pattern + "boards/activediscussions/page/{pageNumber:int}",
                new { controller = "Boards", action = "ActiveDiscussions" });

            endpointRouteBuilder.MapControllerRoute("ActiveDiscussionsRSS", $"{pattern}boards/activediscussionsrss",
				new { controller = "Boards", action = "ActiveDiscussionsRSS" });

            endpointRouteBuilder.MapControllerRoute("PostEdit", 
                pattern + "boards/postedit/{id:min(0)}",
				new { controller = "Boards", action = "PostEdit" });

            endpointRouteBuilder.MapControllerRoute("PostDelete", 
                pattern + "boards/postdelete/{id:min(0)}",
				new { controller = "Boards", action = "PostDelete" });

            endpointRouteBuilder.MapControllerRoute("PostCreate", 
                pattern + "boards/postcreate/{id:min(0)}",
				new { controller = "Boards", action = "PostCreate" });

            endpointRouteBuilder.MapControllerRoute("PostCreateQuote", 
                pattern + "boards/postcreate/{id:min(0)}/{quote:min(0)}",
				new { controller = "Boards", action = "PostCreate" });

            endpointRouteBuilder.MapControllerRoute("TopicEdit", 
                pattern + "boards/topicedit/{id:min(0)}",
				new { controller = "Boards", action = "TopicEdit" });

            endpointRouteBuilder.MapControllerRoute("TopicDelete", 
                pattern + "boards/topicdelete/{id:min(0)}",
				new { controller = "Boards", action = "TopicDelete" });

            endpointRouteBuilder.MapControllerRoute("TopicCreate", 
                pattern + "boards/topiccreate/{id:min(0)}",
				new { controller = "Boards", action = "TopicCreate" });

            endpointRouteBuilder.MapControllerRoute("TopicMove", 
                pattern + "boards/topicmove/{id:min(0)}",
				new { controller = "Boards", action = "TopicMove" });

            endpointRouteBuilder.MapControllerRoute("TopicWatch", 
                pattern + "boards/topicwatch/{id:min(0)}",
				new { controller = "Boards", action = "TopicWatch" });

            endpointRouteBuilder.MapControllerRoute("TopicSlug", 
                pattern + "boards/topic/{id:min(0)}/{slug?}",
				new { controller = "Boards", action = "Topic" });

            endpointRouteBuilder.MapControllerRoute("TopicSlugPaged", 
                pattern + "boards/topic/{id:min(0)}/{slug?}/page/{pageNumber:int}",
                new { controller = "Boards", action = "Topic" });

            endpointRouteBuilder.MapControllerRoute("ForumWatch", 
                pattern + "boards/forumwatch/{id:min(0)}",
				new { controller = "Boards", action = "ForumWatch" });

            endpointRouteBuilder.MapControllerRoute("ForumRSS", 
                pattern + "boards/forumrss/{id:min(0)}",
				new { controller = "Boards", action = "ForumRSS" });

            endpointRouteBuilder.MapControllerRoute("ForumSlug", 
                pattern + "boards/forum/{id:min(0)}/{slug?}",
				new { controller = "Boards", action = "Forum" });

            endpointRouteBuilder.MapControllerRoute("ForumSlugPaged", 
                pattern + "boards/forum/{id:min(0)}/{slug?}/page/{pageNumber:int}",
                new { controller = "Boards", action = "Forum" });

            endpointRouteBuilder.MapControllerRoute("ForumGroupSlug", 
                pattern + "boards/forumgroup/{id:min(0)}/{slug?}",
				new { controller = "Boards", action = "ForumGroup"});

            endpointRouteBuilder.MapControllerRoute("Search", $"{pattern}boards/search",
				new { controller = "Boards", action = "Search" });

            //private messages
            endpointRouteBuilder.MapControllerRoute("PrivateMessages", 
                pattern + "privatemessages/{tab?}",
				new { controller = "PrivateMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("PrivateMessagesPaged", 
                pattern + "privatemessages/{tab?}/page/{pageNumber:min(0)}",
				new { controller = "PrivateMessages", action = "Index" });

            endpointRouteBuilder.MapControllerRoute("PrivateMessagesInbox", $"{pattern}inboxupdate",
				new { controller = "PrivateMessages", action = "InboxUpdate" });

            endpointRouteBuilder.MapControllerRoute("PrivateMessagesSent", $"{pattern}sentupdate",
				new { controller = "PrivateMessages", action = "SentUpdate" });

            endpointRouteBuilder.MapControllerRoute("SendPM", 
                pattern + "sendpm/{toUserId:min(0)}",
				new { controller = "PrivateMessages", action = "SendPM" });

            endpointRouteBuilder.MapControllerRoute("SendPMReply", 
                pattern + "sendpm/{toUserId:min(0)}/{replyToMessageId:min(0)}",
				new { controller = "PrivateMessages", action = "SendPM" });

            endpointRouteBuilder.MapControllerRoute("ViewPM", 
                pattern + "viewpm/{privateMessageId:min(0)}",
				new { controller = "PrivateMessages", action = "ViewPM" });

            endpointRouteBuilder.MapControllerRoute("DeletePM", 
                pattern + "deletepm/{privateMessageId:min(0)}",
				new { controller = "PrivateMessages", action = "DeletePM" });

            //activate newsletters
            endpointRouteBuilder.MapControllerRoute("NewsletterActivation", 
                pattern + "newsletter/subscriptionactivation/{token:guid}/{active}",
				new { controller = "Newsletter", action = "SubscriptionActivation" });

            //robots.txt
            endpointRouteBuilder.MapControllerRoute("robots.txt", $"{pattern}robots.txt",
				new { controller = "Common", action = "RobotsTextFile" });

            //sitemap (XML)
            endpointRouteBuilder.MapControllerRoute("sitemap.xml", $"{pattern}sitemap.xml",
				new { controller = "Common", action = "SitemapXml" });

            endpointRouteBuilder.MapControllerRoute("sitemap-indexed.xml", 
                pattern + "sitemap-{Id:min(0)}.xml",
				new { controller = "Common", action = "SitemapXml" });

            //store closed
            endpointRouteBuilder.MapControllerRoute("StoreClosed", $"{pattern}storeclosed",
				new { controller = "Common", action = "StoreClosed" });

            //install
            endpointRouteBuilder.MapControllerRoute("Installation", $"{pattern}{TvProgInstallationDefaults.InstallPath}",
                new { controller = "Install", action = "Index" });

            //error page
            endpointRouteBuilder.MapControllerRoute("Error", "error",
                new { controller = "Common", action = "Error" });

            //page not found
            endpointRouteBuilder.MapControllerRoute("PageNotFound", $"{pattern}page-not-found", 
                new { controller = "Common", action = "PageNotFound" });
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
