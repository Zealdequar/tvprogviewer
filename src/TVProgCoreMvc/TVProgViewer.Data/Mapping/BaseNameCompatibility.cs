using System;
using System.Collections.Generic;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Security;
using TVProgViewer.Core.Domain.Shipping;

namespace TVProgViewer.Data.Mapping
{
    /// <summary>
    /// Base instance of backward compatibility of table naming
    /// </summary>
    public partial class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(ProductAttributeMapping), "Product_ProductAttribute_Mapping" },
            { typeof(ProductProductTagMapping), "Product_ProductTag_Mapping" },
            { typeof(ProductReviewReviewTypeMapping), "ProductReview_ReviewType_Mapping" },
            { typeof(UserAddressMapping), "UserAddresses" },
            { typeof(UserUserRoleMapping), "User_UserRole_Mapping" },
            { typeof(DiscountCategoryMapping), "Discount_AppliedToCategories" },
            { typeof(DiscountManufacturerMapping), "Discount_AppliedToManufacturers" },
            { typeof(DiscountProductMapping), "Discount_AppliedToProducts" },
            { typeof(PermissionRecordUserRoleMapping), "PermissionRecord_Role_Mapping" },
            { typeof(ShippingMethodCountryMapping), "ShippingMethodRestrictions" },
            { typeof(ProductCategory), "Product_Category_Mapping" },
            { typeof(ProductManufacturer), "Product_Manufacturer_Mapping" },
            { typeof(ProductPicture), "Product_Picture_Mapping" },
            { typeof(ProductSpecificationAttribute), "Product_SpecificationAttribute_Mapping" },
            { typeof(ForumGroup), "Forums_Group" },
            { typeof(Forum), "Forums_Forum" },
            { typeof(ForumPost), "Forums_Post" },
            { typeof(ForumPostVote), "Forums_PostVote" },
            { typeof(ForumSubscription), "Forums_Subscription" },
            { typeof(ForumTopic), "Forums_Topic" },
            { typeof(PrivateMessage), "Forums_PrivateMessage" },
            { typeof(NewsItem), "News" }
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
            { (typeof(User), "BillingAddressId"), "BillingAddress_Id" },
            { (typeof(User), "ShippingAddressId"), "ShippingAddress_Id" },
            { (typeof(UserUserRoleMapping), "UserId"), "User_Id" },
            { (typeof(UserUserRoleMapping), "UserRoleId"), "UserRole_Id" },
            { (typeof(PermissionRecordUserRoleMapping), "PermissionRecordId"), "PermissionRecord_Id" },
            { (typeof(PermissionRecordUserRoleMapping), "UserRoleId"), "UserRole_Id" },
            { (typeof(ProductProductTagMapping), "ProductId"), "Product_Id" },
            { (typeof(ProductProductTagMapping), "ProductTagId"), "ProductTag_Id" },
            { (typeof(DiscountCategoryMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountCategoryMapping), "EntityId"), "Category_Id" },
            { (typeof(DiscountManufacturerMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountManufacturerMapping), "EntityId"), "Manufacturer_Id" },
            { (typeof(DiscountProductMapping), "DiscountId"), "Discount_Id" },
            { (typeof(DiscountProductMapping), "EntityId"), "Product_Id" },
            { (typeof(UserAddressMapping), "AddressId"), "Address_Id" },
            { (typeof(UserAddressMapping), "UserId"), "User_Id" },
            { (typeof(ShippingMethodCountryMapping), "ShippingMethodId"), "ShippingMethod_Id" },
            { (typeof(ShippingMethodCountryMapping), "CountryId"), "Country_Id" },
        };
    }
}