using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a User mapping configuration
    /// </summary>
    public partial class UserMap : TvProgEntityTypeConfiguration<User>
    {
        #region Methods

        /// <summary>
        /// Конфигурирует сущность
        /// </summary>
        /// <param name="builder">Строитель, который будет использоваться для настройки сущности</param>
        public override void Configure(EntityMappingBuilder<User> builder)
        {
            builder.HasTableName(nameof(User));

            builder.Property(user => user.UserName).HasLength(1000);
            builder.Property(user => user.Email).HasLength(1000);
            builder.Property(user => user.EmailToRevalidate).HasLength(1000);
            builder.Property(user => user.SystemName).HasLength(400);

            builder.Property(user => user.BillingAddressId).HasColumnName("BillingAddress_Id");
            builder.Property(user => user.ShippingAddressId).HasColumnName("ShippingAddress_Id");

            builder.Property(user => user.LastName).HasLength(150);
            builder.Property(user => user.FirstName).HasLength(150);
            builder.Property(user => user.MiddleName).HasLength(150);
            builder.Property(user => user.BirthDate);
            builder.Property(user => user.Gender);
            builder.Property(user => user.MobilePhone);
            builder.Property(user => user.PersonalDataAggree);
            builder.Property(user => user.UserGuid);
            builder.Property(user => user.AdminComment);
            builder.Property(user => user.IsTaxExempt);
            builder.Property(user => user.AffiliateId);
            builder.Property(user => user.VendorId);
            builder.Property(user => user.HasShoppingCartItems);
            builder.Property(user => user.RequireReLogin);
            builder.Property(user => user.FailedLoginAttempts);
            builder.Property(user => user.CannotLoginUntilDateUtc);
            builder.Property(user => user.Active);
            builder.Property(user => user.Deleted);
            builder.Property(user => user.IsSystemAccount);
            builder.Property(user => user.LastIpAddress);
            builder.Property(user => user.CreatedOnUtc);
            builder.Property(user => user.LastLoginDateUtc);
            builder.Property(user => user.LastActivityDateUtc);
            builder.Property(user => user.RegisteredInStoreId);
            builder.Property(user => user.GmtZone).HasLength(10);
            builder.Property(user => user.Catalog).HasLength(36);
        }
        #endregion
    }
}