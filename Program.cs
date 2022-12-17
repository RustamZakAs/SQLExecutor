using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using Newtonsoft.Json;

namespace SQLExecutor
{
    class Program
    {
        private static LocalParam LocalParams { get; set; } = new LocalParam();
        private static string ParamsPath { get; set; } = "Params.json";
        static void Main(string[] args)
        {
            try
            {
                _ = LogRegistrator.WriteToLogFileAsync("Started");
                if (!System.IO.File.Exists(ParamsPath))
                {
                    string json = JsonConvert.SerializeObject(LocalParams, Formatting.Indented);
                    System.IO.File.WriteAllText(ParamsPath, json);
                    return;
                }
                else
                {
                    string json = System.IO.File.ReadAllText(ParamsPath);
                    LocalParams = JsonConvert.DeserializeObject<LocalParam>(json);
                }

                SqlConnection sqlConnection = DBSQLServerUtils.GetDBConnection(LocalParams.DataSource, LocalParams.Database, LocalParams.Login, LocalParams.Password, LocalParams.Timeout);
                string sql = System.IO.File.ReadAllText(LocalParams.QueryPath);

                using (SqlConnection connection = new SqlConnection(sqlConnection.ConnectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        var result = connection.Execute(sql, null, transaction, 0);
                        _ = LogRegistrator.WriteToLogFileAsync("Result: '" + result + "'");
                    }
                }
                _ = LogRegistrator.WriteToLogFileAsync("Ended");
            }
            catch (Exception ex)
            {
                _ = LogRegistrator.WriteToLogFileAsync("Ended" + " | " + ex?.Message + " | " + ex?.InnerException?.Message, Status.Error);
                return;
                throw;
            }
            
        }
    }
}