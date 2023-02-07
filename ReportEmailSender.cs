using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLExecutor
{
    public class ReportEmailSender
    {
        //https://www.acodersjourney.com/how-to-send-email-using-csharp-and-outlook/
        private readonly EmailSettings _emailSettings;
        private readonly List<string> _recipients;

        public ReportEmailSender(EmailSettings emailSettings = null)
        {
            _emailSettings = emailSettings;
            _recipients = emailSettings?.MailRecipients;
        }

        public void Send(List<LogContext> contexts)
        {
            if (contexts == null) contexts = new List<LogContext>();

            string subject = $@"Job report {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}";
            string htmlMessage = @"
<!DOCTYPE html>
<html>
    <head>
        <style>
            table {
                font-family: arial, sans-serif;
                border-collapse: collapse;
                width: 100%;
            }

            td, th {
                border: 2px solid #dddddd;
                text-align: left;
                padding: 2px;
            }
            .Error {
                color: red;
            }
        </style>
    </head>
    <body>
        <table>
            <tr>
                <th>Sətir sayı</th>
                <th>Məzmun</th>
            </tr>
            {{table_values}}
        </table>
    </body>
</html>";

            string values = @"";
            foreach (var item in contexts.Select((value, i) => new { value, i }))
            {
                string color = item.i % 2 == 0 ? "style='background-color: #dddddd;'" : "";
                values += $@"<tr {color}>
                                <td class='{item.value.status}'>{item.i + 1}</td>
                                <td>{item.value.context}</td>
                            </tr>";
            }
            htmlMessage = htmlMessage.Replace("{{table_values}}", values);
            new ReportEmailSender(_emailSettings).SendEmailAsync(_recipients.ToArray(), subject, htmlMessage, null);
        }

        public void SendEmailAsync(string[] recipients, string subject, string htmlMessage, string[] attachments)
        {
            if (_emailSettings == null) throw new Exception("EmailSettings is null");
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                message.From = new System.Net.Mail.MailAddress(_emailSettings.MailSender, _emailSettings.MailSenderName);
                message.Sender = new System.Net.Mail.MailAddress(_emailSettings.MailSender, _emailSettings.MailSenderName);
                if (recipients != null && recipients.Length > 0)
                    foreach (string item in recipients)
                    {
                        message.To.Add(new System.Net.Mail.MailAddress(item));

                    }
                if (attachments != null && attachments.Length > 0)
                    foreach (string item in attachments)
                    {
                        message.Attachments.Add(new System.Net.Mail.Attachment(item));
                    }
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html
                message.Body = htmlMessage;
                //smtp.Port = 587;
                smtp.Port = _emailSettings.MailPort;
                //smtp.Host = "epoct.ds-az.com"; //for gmail host
                smtp.Host = _emailSettings.MailServer;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential("rustam.z@bizimmarket.az", "RZAk33161!@");
                smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.MailSender, _emailSettings.MailPassword);
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }

    public class EmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string MailSenderName { get; set; }
        public string MailSender { get; set; }
        public string MailPassword { get; set; }
        public List<string> MailRecipients { get; set; }
    }
}
