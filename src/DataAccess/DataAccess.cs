using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace TVProgViewer.DataAccess
{
    /// <summary>
    /// ADO.NET вспомогательные методы
    /// </summary>
    public class DataAccess 
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Выполнить хранимку
        /// </summary>
        /// <param name="conn">Соединение</param>
        /// <param name="cmdName">Название хранимой процедуры</param>
        /// <param name="pars">Параметры</param>
        public void ExecCommand(DbConnection conn, string cmdName, List<DbParameter> pars)
        {
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 99000;
                if (pars != null)
                    cmd.Parameters.AddRange((from p in pars where p != null select p).ToArray<DbParameter>());
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    string errMsg = ex.Message;
                    Logger.Error(ex, errMsg);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Выполнить хранимку с одним параметром
        /// </summary>
        /// <param name="conn">Соединение</param>
        /// <param name="cmdName">Название команды</param>
        /// <param name="par">Параметр</param>
        public void ExecCommand(DbConnection conn, string cmdName, DbParameter par)
        {
            ExecCommand(conn, cmdName, new List<DbParameter>() { par });
        }

        /// <summary>
        /// Выполнить хранимку и получить список
        /// </summary>
        /// <param name="conn">Соединение</param>
        /// <param name="cmdName">Название хранимой процедуры</param>
        /// <param name="pars">Параметры</param>
        /// <param name="func">Лямбда-функция для маппинга полей</param>
        public List<T> ExecReaderCommand<T>(DbConnection conn, string cmdName, List<DbParameter> pars,
            Func<DbDataReader, T> func)
        {
            List<T> listResult = new List<T>();
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandTimeout = 99000;
                cmd.CommandText = cmdName;
                cmd.CommandType = CommandType.StoredProcedure;
                if (pars != null)
                    cmd.Parameters.AddRange((from p in pars where p != null select p).ToArray<DbParameter>());
                try
                {
                    conn.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                listResult.Add(func(reader));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    conn.Close();
                }
            }
            return listResult;
        }

        /// <summary>
        /// Выполнить хранимку с одним параметром и получить список
        /// </summary>
        /// <param name="conn">Соединение</param>
        /// <param name="cmdName">Название хранимой процедуры</param>
        /// <param name="par">Параметр</param>
        /// <param name="func">Лямбда-функция для маппинга полей</param>
        public List<T> ExecReaderCommand<T>(
            DbConnection conn, 
            string cmdName, 
            DbParameter par,
            Func<DbDataReader, T> func)
        {
            return ExecReaderCommand<T>(conn, cmdName, new List<DbParameter>() { par }, func);
        }

        /// <summary>
        /// Выполнить хранимку и получить один объект
        /// </summary>
        /// <param name="conn">Соединение</param>
        /// <param name="cmdName">Название хранимой процедуры</param>
        /// <param name="pars">Параметры</param>
        /// <param name="func">Лямда-функция для маппинга полей</param>
        public T ExecReaderCommandReturnOne<T>(
            DbConnection conn,
            string cmdName,
            List<DbParameter> pars,
            Func<DbDataReader, T> func)
        {
            return ExecReaderCommand<T>(conn, cmdName, pars, func).FirstOrDefault();
        }

        /// <summary>
        /// Выполнить хранимку с единственным параметром и получить один объект
        /// </summary>
        /// <param name="conn">Соединение</param>
        /// <param name="cmdName">Название хранимой процедуры</param>
        /// <param name="par">Параметр</param>
        /// <param name="func">Лямда-функция для маппинга полей</param>
        public T ExecReaderCommandReturnOne<T>(
            DbConnection conn,
            string cmdName,
            DbParameter par,
            Func<DbDataReader, T> func)
        {
            return ExecReaderCommand<T>(conn, cmdName, new List<DbParameter>() { par }, func).FirstOrDefault();
        }

        /// <summary>
        /// Выполнить хранимку с возвращением скаляра
        /// </summary>
        /// <param name="conn">Соединение</param>
        /// <param name="cmdName">Название хранимой процедуры</param>
        /// <param name="pars">Набор параметров</param>
        public object ExecScalar(DbConnection conn, string cmdName, params DbParameter[] pars)
        {
            object scalarResult = null;
            using (DbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = cmdName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(pars);
                try
                {
                    conn.Open();
                    scalarResult = cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    conn.Close();
                }
            }
            return scalarResult;
        }
        
        /// <summary>
        /// Получить главное соединение
        /// </summary>
        /// <returns>Подключение</returns>
        public static DbConnection GetTvProgMainConnection()
        {
           return GetConnection("tvProgBase.Main"); 
        }

        /// <summary>
        /// Получить безопасное соединение
        /// </summary>
        /// <returns>Подключение</returns>
        public static DbConnection GetTvProgSecureConnection()
        {
           return GetConnection("tvProgBase.Secure");
        }

        /// <summary>
        /// Получение соединения
        /// </summary>
        /// <param name="name">Название подключения в файле конфигурации</param>
        /// <returns>Подключение</returns>
        private static DbConnection GetConnection(string name)
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            DbProviderFactory factory = DbProviderFactories.GetFactory(settings.ProviderName);
            DbConnection conn = factory.CreateConnection();
            conn.ConnectionString = settings.ConnectionString;
            return conn;
        }
    }
}
