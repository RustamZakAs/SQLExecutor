using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLExecutor
{
    public class LogRegistrator
    {
        private static string _logFilePath { get; set; } = "Log.txt";
        public static List<LogContext> LogContexts { get; set; } = new List<LogContext>();

        public static async Task WriteToLogFileAsync(string text, Status status = Status.Info)
        {
            LogContext context = new LogContext() { status = status, context = DateTime.Now.ToString() + " " + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + $@" {status.ToString()} -> " + text + "\n" };
            LogContexts.Add(context);

            byte[] encodedText = System.Text.Encoding.Unicode.GetBytes(context.context);

            using (FileStream sourceStream = new FileStream(_logFilePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }
    }

    public class LogContext
    {
        public Status status { get; set; }
        public string context { get; set; }

        public override string ToString()
        {
            return status.ToString() + ": " + context;
        }
    }

    public enum Status
    {
        Info,
        Warning,
        Error,
    }
}
