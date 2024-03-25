using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
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
        private static Stopwatch stopwatch { get; set; } = new Stopwatch();
        static void Main(string[] args)
        {
            try
            {
                stopwatch.Start();

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

                DbConnection dbConnection;
                if (LocalParams.DataType.ToUpper() == "MYSQL")
                {
                    dbConnection = DBSQLServerUtils.GetMySqlConnection(LocalParams.DataSource, LocalParams.Port, LocalParams.Database, LocalParams.Login, LocalParams.Password, LocalParams.Timeout);
                }
                else
                {
                    dbConnection = DBSQLServerUtils.GetSqlConnection(LocalParams.DataSource, LocalParams.Port, LocalParams.Database, LocalParams.Login, LocalParams.Password, LocalParams.Timeout);
                }
                SqlConnection sqlConnection = DBSQLServerUtils.GetSqlConnection(LocalParams.DataSource, LocalParams.Port, LocalParams.Database, LocalParams.Login, LocalParams.Password, LocalParams.Timeout);
                string sql = System.IO.File.ReadAllText(LocalParams.QueryPath);

                using (DbConnection connection = new SqlConnection(sqlConnection.ConnectionString))
                {
                    connection.Open();
                    using (DbTransaction transaction = connection.BeginTransaction())
                    {
                        var result = connection.Execute(sql, null, transaction, 0);
                        _ = LogRegistrator.WriteToLogFileAsync("Result: '" + result + "'");
                        transaction.Commit();
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
            finally
            {
                stopwatch.Stop();
                _ = LogRegistrator.WriteToLogFileAsync($"Job time is { stopwatch.Elapsed.ToString() }");
                if ((LocalParams.SendEmailReportIfError && LogRegistrator.LogContexts.Count(x => x.status == Status.Error) > 0)
                 || (LocalParams.SendEmailReportIfWarning && LogRegistrator.LogContexts.Count(x => x.status == Status.Warning) > 0)
                 || (LocalParams.SendEmailReportIfInfo && LogRegistrator.LogContexts.Count(x => x.status == Status.Info) > 0))
                {
                    try
                    {
                        new ReportEmailSender(LocalParams.EmailSettings).Send(LogRegistrator.LogContexts);
                        _ = LogRegistrator.WriteToLogFileAsync("Mail sended");
                    }
                    catch (Exception)
                    {
                        _ = LogRegistrator.WriteToLogFileAsync("Mail send error", Status.Error);
                    }
                }
            }
        }
    }
}