using System;
using System.Collections.Generic;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Data.Mapping
{
    /// <summary>
    /// Base instance of backward compatibility of table naming
    /// </summary>
    public partial class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new()
        {
            { typeof(TvChannelAttributeMapping), "TvChannel_TvChannelAttribute_Mapping" },
            { typeof(TvChannelTvChannelTagMapping), "TvChannel_TvChannelTag_Mapping" },
            { typeof(TvChannelReviewReviewTypeMapping), "TvChannelReview_ReviewType_Mapping" },
            { typeof(UserAddressMapping), "UserAddresses" },
            { typeof(UserUserRoleMapping), "User_UserRole_Mapping" },
            { typeof(DiscountCategoryMapping), "Discount_AppliedToCategories" },
            { typeof(DiscountManufacturerMapping), "Discount_AppliedToManufacturers" },
            { typeof(DiscountTvChannelMapping), "Discount_AppliedToTvChannels" },
            { typeof(PermissionRecordUserRoleMapping), "PermissionRecord_Role_Mapping" },
            { typeof(ShippingMethodCountryMapping), "ShippingMethodRestrictions" },
            { typeof(TvChannelCategory), "TvChannel_Category_Mapping" },
            { typeof(TvChannelManufacturer), "TvChannel_Manufacturer_Mapping" },
            { typeof(TvChannelPicture), "TvChannel_Picture_Mapping" },
            { typeof(TvChannelSpecificationAttribute), "TvChannel_SpecificationAttribute_Mapping" },
            { typeof(ForumGroup), "Forums_Group" },
            { typeof(Forum), "Forums_Forum" },
            { typeof(ForumPost), "Forums_Post" },
            { typeof(ForumPostVote), "Forums_PostVote" },
            { typeof(ForumSubscription), "Forums_Subscription" },
            { typeof(ForumTopic), "Forums_Topic" },
            { typeof(PrivateMessage), "Forums_PrivateMessage" },
            { typeof(NewsItem), "News" }
        };

        public Dictionary<(Type, string), string> ColumnName => new()
        {
            { (typeof(User), "BillingAddressId"), "BillingAddress_Id" },
            { (typeof(User), "ShippingAddressId"), "ShippingAddress_Id" },
            { (typeof(UserUserRoleMapping), "UserId"), "User_Id" },
            { (typeof(UserUserRoleMapping), "UserRoleId"), "UserRole_Id" },
            { (typeof(PermissionRecordUserRoleMapping), "PermissionRecordId"), "PermissionRecord_Id" },
            { (typeof(PermissionRecordUserRoleMapping), "UserRoleId"), "UserRole_Id" },
            { (typeof(TvChannelTvChannelTagMapping), "TvChannelId"), "TvChannel_Id" },
            { (typeof(TvChannelTvChannelTagMapping), "TvChannelTagId"), "TvChannelTag_Id" },
            { (typeof(DiscountCategoryMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountCategoryMapping), "EntityId"), "Category_Id" },
            { (typeof(DiscountManufacturerMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountManufacturerMapping), "EntityId"), "Manufacturer_Id" },
            { (typeof(DiscountTvChannelMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountTvChannelMapping), "EntityId"), "TvChannel_Id" },
            { (typeof(UserAddressMapping), "AddressId"), "Address_Id" },
            { (typeof(UserAddressMapping), "UserId"), "User_Id" },
            { (typeof(ShippingMethodCountryMapping), "ShippingMethodId"), "ShippingMethod_Id" },
            { (typeof(ShippingMethodCountryMapping), "CountryId"), "Country_Id" },
        };
    }
}