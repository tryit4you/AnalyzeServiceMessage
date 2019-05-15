using MailService;
using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;

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
            watchTimer.Enabled = true;
            try
            {

                watchTimer.Interval = double.Parse(ConfigurationManager.AppSettings["IntervalEnlapsed"].ToString());
            }
            catch (Exception)
            {
               watchTimer.Interval = SetParams.IntervalEnlapsed;
            }
        }

        protected override void OnStop()
        {
            watchTimer.Enabled = false;
        }

        private void WatchTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //đặt lịch khởi động lại cho AnalyzeService nếu dịch vụ dừng
            bool checkScheduleStatus = Libs.CheckSchedule();
            if (checkScheduleStatus)
                Schedule();
            CheckAnalyzeDCURuning();
        }
        private void Schedule()
        {
            ServiceController sc = new ServiceController("AnlyzeService");
            if (sc.Status == ServiceControllerStatus.Stopped)
                sc.Start();
        }
        private void CheckAnalyzeDCURuning()
        {

            string timeLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            ServiceController sc = new ServiceController("AnlyzeService");
            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    WriteLog(timeLog + " : " + "dịch vụ hiện đang chạy!");
                    break;
                case ServiceControllerStatus.Stopped:
                    WriteLog(timeLog + " : " + "dịch vụ đã dừng !");
                    sc.Start();
                    StmpMail.SendMail( "Dịch vụ đã khởi động thành công!");
                    break;
                case ServiceControllerStatus.Paused:
                    WriteLog(timeLog + " : " + "dịch vụ tạm dừng!");
                    sc.Start();
                    StmpMail.SendMail( "Dịch vụ đã khởi động thành công!");
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

            using (EventLog log = new EventLog("MessageService"))
            {
                if (DateTime.Now.Hour == 0)
                    log.Clear();
                log.Source = "MessageService";
                log.WriteEntry(message, EventLogEntryType.Information, 101, 1);
            };
        }
    }
}
