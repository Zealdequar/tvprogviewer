using System.Collections.Generic;
using System.Data;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace TVProgViewer.Data
{
    /// <summary>
    /// Implements database connection abstraction.
    /// </summary>
    internal class TvProgDataConnection : DataConnection
    {
        #region Ctor

        public TvProgDataConnection()
        {
            AddMappingSchema(AdditionalSchema);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type and
        /// returns results as collection of values of specified type
        /// </summary>
        /// <typeparam name="TEntity">Result record type</typeparam>
        /// <param name="storeProcedureName">Procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Returns collection of query result records</returns>
        public IList<TEntity> ExecuteStoredProcedure<TEntity>(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(this, storeProcedureName, dataParameters);

            command.DataConnection.CommandTimeout = 999990;

            var rez = command.QueryProc<TEntity>().ToList();

            var outputParameters = dataParameters.Where(p => p.Direction == ParameterDirection.Output)
                .ToDictionary(p => p.Name, p => p);
            
            foreach (var outputParametersName in outputParameters.Keys.ToList())
            {
                outputParameters[outputParametersName].Value = (Command.Parameters[outputParametersName] as IDbDataParameter)?.Value;
            }

            return rez;
        }

        #endregion

        #region Properties

        public static MappingSchema AdditionalSchema { get; } = new MappingSchema();

        #endregion
    }
}
