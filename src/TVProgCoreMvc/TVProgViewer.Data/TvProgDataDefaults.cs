namespace TVProgViewer.Data
{
    /// <summary>
    /// Represents default values related to TVProgViewer data
    /// </summary>
    public static partial class TvProgDataDefaults
    {
        /// <summary>
        /// Gets a path to the file that contains script to create SQL Server stored procedures
        /// </summary>
        public static string SqlServerStoredProceduresFilePath => "~/App_Data/Install/SqlServer.StoredProcedures.sql";
    }
}
