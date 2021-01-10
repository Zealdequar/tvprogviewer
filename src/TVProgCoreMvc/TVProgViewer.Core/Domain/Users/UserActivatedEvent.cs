using System;
using System.Collections.Generic;
using System.Text;

namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// User activated event
    /// </summary>
    public class UserActivatedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="User">User</param>
        public UserActivatedEvent(User User)
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
