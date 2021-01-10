namespace TVProgViewer.Core.Caching
{
    /// <summary>
    /// Represents default values related to User services
    /// </summary>
    public static partial class TvProgUserServiceDefaults
    {
        /// <summary>
        /// Gets a password salt key size
        /// </summary>
        public static int PasswordSaltKeySize => 5;
        
        /// <summary>
        /// Gets a max username length
        /// </summary>
        public static int UserUsernameLength => 100;

        /// <summary>
        /// Gets a default hash format for User password
        /// </summary>
        public static string DefaultHashedPasswordFormat => "SHA512";
    }
}