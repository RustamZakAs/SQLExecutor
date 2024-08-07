using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;

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
                if (!System.IO.File.Exists(LocalParams.QueryPath))
                {
                    using (var fs = new FileStream(LocalParams.QueryPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (TextWriter tw = new StreamWriter(fs))
                        {
                            tw.Write("");
                            tw.Flush();
                        }
                    }
                }
                string sql = System.IO.File.ReadAllText(LocalParams.QueryPath);

                dbConnection.Open();
                using (DbTransaction transaction = dbConnection.BeginTransaction())
                {
                    var result = dbConnection.Execute(sql, null, transaction, 0);
                    _ = LogRegistrator.WriteToLogFileAsync("Result: '" + result + "'");
                    transaction.Commit();
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
                    int SendMailErrorCount = 0;
                    ReplaySendLog:
                    try
                    {
                        new ReportEmailSender(LocalParams.EmailSettings).Send(LogRegistrator.LogContexts);
                        _ = LogRegistrator.WriteToLogFileAsync("Mail sended");
                    }
                    catch (Exception)
                    {
                        _ = LogRegistrator.WriteToLogFileAsync($"Mail send error {++SendMailErrorCount}", Status.Error);
                        Thread.Sleep(5000);
                        if (SendMailErrorCount <= 5)
                            goto ReplaySendLog;
                    }
                }
            }
        }
    }
}