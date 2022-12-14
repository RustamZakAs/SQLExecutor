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
        public static SqlConnection GetDBConnection(string datasource, string database, string username, string password, int timeout)
        {
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Connection Timeout=" + timeout + ";Persist Security Info=True;Pooling=False;User ID=" 
                        + username + ";Password=" + password;

            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
