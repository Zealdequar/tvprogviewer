using System;
using System.Globalization;
using FluentMigrator;

namespace TVProgViewer.Data.Migrations
{
    /// <summary>
    /// Attribute for a migration
    /// </summary>
    public partial class TvProgMigrationAttribute : MigrationAttribute
    {
        private static long GetVersion(string dateTime)
        {
            return DateTime.ParseExact(dateTime, TvProgMigrationDefaults.DateFormats, CultureInfo.InvariantCulture).Ticks;
        }

        private static long GetVersion(string dateTime, UpdateMigrationType migrationType)
        {
            return GetVersion(dateTime) + (int)migrationType;
        }
        
        private static string GetDescription(string nopVersion, UpdateMigrationType migrationType)
        {
            return string.Format(TvProgMigrationDefaults.UpdateMigrationDescription, nopVersion, migrationType.ToString());
        }

        /// <summary>
        /// Initializes a new instance of the TvProgMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        public TvProgMigrationAttribute(string dateTime) :
            base(GetVersion(dateTime), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TvProgMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="description">The migration description</param>
        public TvProgMigrationAttribute(string dateTime, string description) :
            base(GetVersion(dateTime), description)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TvProgMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="nopVersion">nopCommerce full version</param>
        /// <param name="migrationType">The migration type</param>
        public TvProgMigrationAttribute(string dateTime, string nopVersion, UpdateMigrationType migrationType) :
            base(GetVersion(dateTime, migrationType), GetDescription(nopVersion, migrationType))
        {
        }
    }
}
