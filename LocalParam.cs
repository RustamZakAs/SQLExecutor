using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLExecutor
{
    [Serializable]
    public class LocalParam
    {
        public string DataSource { get; set; }
        public string Database { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
        public string QueryPath { get; set; }

        public EmailSettings EmailSettings { get; set; }

        public bool SendEmailReportIfError { get; set; }
        public bool SendEmailReportIfWarning { get; set; }
        public bool SendEmailReportIfInfo { get; set; }

        public LocalParam()
        {
            this.DataSource = string.Empty;
            this.Database = string.Empty;
            this.Login = string.Empty;
            this.Password = string.Empty;
            this.Timeout = 0;
            this.QueryPath = "Query.txt";

            this.EmailSettings = new EmailSettings();

            this.SendEmailReportIfInfo = false;
            this.SendEmailReportIfWarning = false;
            this.SendEmailReportIfError = true;
        }

        public LocalParam DefaultData()
        {
            this.DataSource = string.Empty;
            this.Database = string.Empty;
            this.Login = string.Empty;
            this.Password = string.Empty;
            this.Timeout = 0;
            this.QueryPath = "Query.txt";

            this.SendEmailReportIfInfo = false;
            this.SendEmailReportIfWarning = false;
            this.SendEmailReportIfError = true;

            return this;
        }
    }
}
