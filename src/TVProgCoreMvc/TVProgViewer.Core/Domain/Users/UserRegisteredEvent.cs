namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// User registered event
    /// </summary>
    public class UserRegisteredEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="User">User</param>
        public UserRegisteredEvent(User User)
        {
            User = User;
        }

        /// <summary>
        /// User
        /// </summary>
        public User User
        {
            get;
        }
    }
}