namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Represents a best User report line
    /// </summary>
    public partial class BestUserReportLine
    {
        /// <summary>
        /// Gets or sets the User identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the order count
        /// </summary>
        public int OrderCount { get; set; }
    }
}
