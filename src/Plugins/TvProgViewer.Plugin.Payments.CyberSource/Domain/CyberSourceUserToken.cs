using TvProgViewer.Core;

namespace TvProgViewer.Plugin.Payments.CyberSource.Domain
{
    /// <summary>
    /// Represents a CyberSource user token
    /// </summary>
    public class CyberSourceUserToken : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the token subscription identifier
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the last four digit of card
        /// </summary>
        public string LastFourDigitOfCard { get; set; }

        /// <summary>
        /// Gets or sets the first six digit of card
        /// </summary>
        public string FirstSixDigitOfCard { get; set; }

        /// <summary>
        /// Gets or sets the card expiration year
        /// </summary>
        public string CardExpirationYear { get; set; }

        /// <summary>
        /// Gets or sets the card expiration month
        /// </summary>
        public string CardExpirationMonth { get; set; }

        /// <summary>
        /// Gets or sets the three digit card type
        /// </summary>
        public string ThreeDigitCardType { get; set; }

        /// <summary>
        /// Gets or sets the instrument identifier
        /// </summary>
        public string InstrumentIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the instrument identifier is new
        /// </summary>
        public bool IsInstrumentIdentifierNew { get; set; }

        /// <summary>
        /// Gets or sets the instrument identifier status
        /// </summary>
        public string InstrumentIdentifierStatus { get; set; }

        /// <summary>
        /// Gets or sets the CyberSource user identifier
        /// </summary>
        public string CyberSourceUserId { get; set; }

        /// <summary>
        /// Gets or sets the CyberSource transaction identifier
        /// </summary>
        public string TransactionId { get; set; }
    }
}