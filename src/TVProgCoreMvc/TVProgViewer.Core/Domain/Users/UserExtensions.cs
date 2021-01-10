using System;

namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// User extensions
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Gets a value indicating whether User a search engine
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        public static bool IsSearchEngineAccount(this User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (!User.IsSystemAccount || string.IsNullOrEmpty(User.SystemName))
                return false;

            var result = User.SystemName.Equals(TvProgUserDefaults.SearchEngineUserName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the User is a built-in record for background tasks
        /// </summary>
        /// <param name="User">User</param>
        /// <returns>Result</returns>
        public static bool IsBackgroundTaskAccount(this User User)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (!User.IsSystemAccount || string.IsNullOrEmpty(User.SystemName))
                return false;

            var result = User.SystemName.Equals(TvProgUserDefaults.BackgroundTaskUserName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }
    }
}