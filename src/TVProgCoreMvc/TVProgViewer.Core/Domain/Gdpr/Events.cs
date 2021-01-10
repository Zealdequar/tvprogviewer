namespace TVProgViewer.Core.Domain.Gdpr
{
    /// <summary>
    /// User permanently deleted (GDPR)
    /// </summary>
    public class UserPermanentlyDeleted
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <param name="email">Email</param>
        public UserPermanentlyDeleted(int UserId, string email)
        {
            UserId = UserId;
            Email = email;
        }

        /// <summary>
        /// User identifier
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; }
    }
}