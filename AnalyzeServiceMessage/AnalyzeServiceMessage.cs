using MailService;
using MMS.FileToDataService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeServiceMessage
{
    public partial class AnalyzeServiceMessage : ServiceBase
    {
        public AnalyzeServiceMessage()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.watchTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            this.watchTimer.Enabled = false;
        }

        private void WatchTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
          
            CheckAnalyzeDCURuning();
        }
        private void CheckAnalyzeDCURuning()
        {

            string timeLog = DateTime.Now.ToString("ddMMyyyy");
          
            ServiceController sc = new ServiceController("AnlyzeService");
     
            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
       
                    WriteLog(timeLog + " : " + "dịch vụ hiện đang chạy!");
                    break;
                case ServiceControllerStatus.Stopped:
                    WriteLog(timeLog + " : " + "dịch vụ đã dừng !");
                    sc.Start();
                    StmpMail.SendEmail("vovanhung.demo@gmail.com","Email theo dõi dịch vụ","Thông báo lỗi","Thông báo","Dịch vụ đã khởi động thành công!");
                    break;
                case ServiceControllerStatus.Paused:
                    WriteLog(timeLog + " : " + "dịch vụ tạm dừng!");
                    sc.Start();
                    StmpMail.SendEmail("vovanhung.demo@gmail.com","Email theo dõi dịch vụ","Service is paused ","Thông báo","Dịch vụ đã khởi động thành công!");
                    break;
                case ServiceControllerStatus.StopPending:
                    WriteLog(timeLog + " : " + "dịch vụ đang dừng!");
                    break;
                case ServiceControllerStatus.StartPending:
                    WriteLog(timeLog + " : " + "dịch vụ chờ bắt đầu!");
                    break;
                default:
                    WriteLog(timeLog + " : " + "Không rõ!");
                    break;
            }
        }
        private void WriteLog(string message)
        {
            using (EventLog log = new EventLog("MessageService")) {
                log.Source = "MessageService";
                log.WriteEntry(message, EventLogEntryType.Information, 101, 1);
            };
        }
    }
}
