namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// "User is logged out" event
    /// </summary>
    public class UserLoggedOutEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="User">User</param>
        public UserLoggedOutEvent(User User)
        {
            User = User;
        }

        /// <summary>
        /// Get or set the User
        /// </summary>
        public User User { get; }
    }
}