using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;

namespace GeoReVi
{
    public class EFDBConnectionApplicationRoleInterception : IDbConnectionInterceptor
    {
        private string _sqlServerApplicationRoleName;
        private string _password;
        private string _dbname;
        private byte[] _cookie;

        public EFDBConnectionApplicationRoleInterception() { }

        public EFDBConnectionApplicationRoleInterception(string sqlAppRoleName, string password, string dbname)
        {
            _sqlServerApplicationRoleName = sqlAppRoleName;
            _password = password;
            _dbname = dbname;
        }

        public void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (connection.State != ConnectionState.Open) return;
            if (!connection.Database.Equals(_dbname)) return;
            ActivateApplicationRole(connection, this._sqlServerApplicationRoleName, _password);
        }

        public void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (connection.State != ConnectionState.Open) return;
            if (!connection.Database.Equals(_dbname)) return;
            DeActivateApplicationRole(connection, _cookie);
        }

        public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            if (connection.State != ConnectionState.Open) return;
            if (!connection.Database.Equals(_dbname)) return;
            DeActivateApplicationRole(connection, _cookie);
        }

        public virtual void ActivateApplicationRole(DbConnection dbConn, string appRoleName, string password)
        {
            if (dbConn == null)
                throw new ArgumentNullException("DbConnection");
            if (ConnectionState.Open != dbConn.State)
                throw new InvalidOperationException("DBConnection must be opened before activating application role");
            if (string.IsNullOrWhiteSpace(appRoleName))
                throw new ArgumentNullException("appRoleName");
            if (password == null)
                throw new ArgumentNullException("password");

            using (DbCommand cmd = dbConn.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_setapprole";
                cmd.Parameters.Add(new SqlParameter("@rolename", appRoleName));
                cmd.Parameters.Add(new SqlParameter("@password", password));
                cmd.Parameters.Add(new SqlParameter("@fCreateCookie", SqlDbType.Bit) { Value = true });
                SqlParameter cookie = new SqlParameter("@cookie", System.Data.SqlDbType.Binary, 50) { Direction = System.Data.ParameterDirection.InputOutput };

                cmd.Parameters.Add(cookie);
                cmd.ExecuteNonQuery();

                if (cookie.Value == null)
                {
                    throw new InvalidOperationException("Failed to set application role.");
                }
                _cookie = (byte[])cookie.Value;
            }
        }

        public virtual void DeActivateApplicationRole(DbConnection dbConn, byte[] cookie)
        {
            using (DbCommand cmd = dbConn.CreateCommand())
            {
                cmd.CommandText = "sp_unsetapprole";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@cookie", SqlDbType.VarBinary, 50) { Value = cookie });
                cmd.ExecuteNonQuery();
            }
        }

        #region Not used interceptions

        public void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext) { }

        public void BeginningTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext) { }

        public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void ConnectionStringGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void ConnectionStringGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void ConnectionStringSet(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext) { }

        public void ConnectionStringSetting(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext) { }

        public void ConnectionTimeoutGetting(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext) { }

        public void ConnectionTimeoutGot(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext) { }

        public void DataSourceGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void DataSourceGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void DatabaseGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void DatabaseGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void EnlistedTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext) { }

        public void EnlistingTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext) { }

        public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext) { }

        public void ServerVersionGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void ServerVersionGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext) { }

        public void StateGetting(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext) { }

        public void StateGot(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext) { }

        #endregion
    }
}
