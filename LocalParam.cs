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

        public LocalParam()
        {
            this.DataSource = string.Empty;
            this.Database = string.Empty;
            this.Login = string.Empty;
            this.Password = string.Empty;
            this.Timeout = 0;
            this.QueryPath = Environment.CurrentDirectory;
        }
    }
}
