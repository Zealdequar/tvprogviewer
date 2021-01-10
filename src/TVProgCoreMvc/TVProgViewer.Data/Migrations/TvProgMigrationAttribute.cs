using System;
using System.Globalization;
using FluentMigrator;

namespace TVProgViewer.Data.Migrations
{
    public partial class TvProgMigrationAttribute : MigrationAttribute
    {
        private static readonly string[] _dateFormats = { "yyyy-MM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss:fffffff", "yyyy.MM.dd HH:mm:ss:fffffff", "yyyy/MM/dd HH:mm:ss:fffffff" };
                                                            
        /// <summary>
        /// Initializes a new instance of the TvProgMigrationAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        public TvProgMigrationAttribute(string dateTime) :
            base(DateTime.ParseExact(dateTime.Trim(), _dateFormats, CultureInfo.InvariantCulture).Ticks, null)
        {

        }

        /// <inheritdoc />
        public TvProgMigrationAttribute(long version, string description) : base(version, description)
        {
        }

        /// <inheritdoc />
        public TvProgMigrationAttribute(long version, TransactionBehavior transactionBehavior = TransactionBehavior.Default, string description = null) : base(version, transactionBehavior, description)
        {
        }
    }
}
