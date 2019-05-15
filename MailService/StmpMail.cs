using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailService
{
    public class StmpMail
    {
        public async static void SendMail(string message)
        {
            string host, mailserver, Pass, mailto;
            string timeLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            try
            {

                host = ConfigurationManager.AppSettings["Host"].ToString();
                mailto = ConfigurationManager.AppSettings["SendToEmail"].ToString();
                mailserver = ConfigurationManager.AppSettings["EmailServer"].ToString();
                Pass = ConfigurationManager.AppSettings["Password"].ToString();
                
                var smtpClient = new SmtpClient
                {
                    Host = host, // set your SMTP server name here
                    Port = 587, // Port
                    EnableSsl = true,
                    Credentials = new NetworkCredential(mailserver, Pass)
                };

                using (var mess = new MailMessage(mailserver, mailto)
                {
                    Subject = "Thông báo từ dịch vụ Mail service",
                    Body = message
                })
                {
                    try
                    {
                        await smtpClient.SendMailAsync(mess);
                        using (EventLog log = new EventLog("MessageService"))
                        {
                            log.Source = "MessageService";
                            log.WriteEntry(timeLog+" : send mail success", EventLogEntryType.Error, 101, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (EventLog log = new EventLog("MessageService"))
                        {
                            log.Source = "MessageService";
                            log.WriteEntry("send mail with error:" + ex.Message, EventLogEntryType.Error, 101, 1);
                        }
                    }

                }
            }
            catch (ConfigurationErrorsException ex)
            {
                using (EventLog log = new EventLog("MessageService"))
                {
                    log.Source = "MessageService";
                    log.WriteEntry("get configuration error: " + ex.Message, EventLogEntryType.Information, 101, 1);
                }
            }
        }
    }
}
