using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Forums;

namespace TVProgViewer.Data.Mapping.Forums
{
    /// <summary>
    /// Represents a private message mapping configuration
    /// </summary>
    public partial class PrivateMessageMap : TvProgEntityTypeConfiguration<PrivateMessage>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<PrivateMessage> builder)
        {
            builder.HasTableName(TvProgMappingDefaults.PrivateMessageTable);

            builder.Property(message => message.Subject).HasLength(450).IsNullable(false);
            builder.Property(message => message.Text).IsNullable(false);

            builder.Property(message => message.StoreId);
            builder.Property(message => message.FromUserId);
            builder.Property(message => message.ToUserId);
            builder.Property(message => message.IsRead);
            builder.Property(message => message.IsDeletedByAuthor);
            builder.Property(message => message.IsDeletedByRecipient);
            builder.Property(message => message.CreatedOnUtc);
        }

        #endregion
    }
}