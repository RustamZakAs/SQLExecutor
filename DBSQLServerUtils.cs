using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLExecutor
{
    public class DBSQLServerUtils
    {
        public static SqlConnection GetSqlConnection(string datasource, int port, string database, string username, string password, int timeout)
        {
            string connString = $@"Application Name={"SQL Executor"};
                                   Data Source={datasource},{port};
                                   Initial Catalog={database};
                                   Connection Timeout={timeout};
                                   Persist Security Info=True;
                                   Pooling=True;
                                   User ID={username};
                                   Password={password}";

            return new SqlConnection(connString);
        }

        public static MySqlConnection GetMySqlConnection(string datasource, int port, string database, string username, string password, int timeout)
        {
            string connString = $@"Server={datasource};
                                   Port={port};
                                   Database={database};
                                   Pooling=True;
                                   Convert zero datetime=True;
                                   Uid={username};
                                   Pwd={password};
                                   Connection Timeout={timeout};";

            return new MySqlConnection(connString);
        }
    }
}