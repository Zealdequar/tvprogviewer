using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Mapping.Users
{
    /// <summary>
    /// Represents a User password mapping configuration
    /// </summary>
    public partial class UserPasswordMap : TvProgEntityTypeConfiguration<UserPassword>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<UserPassword> builder)
        {
            builder.HasTableName(nameof(UserPassword));

            builder.Property(password => password.UserId);
            builder.Property(password => password.Password);
            builder.Property(password => password.PasswordFormatId);
            builder.Property(password => password.PasswordSalt);
            builder.Property(password => password.CreatedOnUtc);

            builder.Ignore(password => password.PasswordFormat);
        }

        #endregion
    }
}