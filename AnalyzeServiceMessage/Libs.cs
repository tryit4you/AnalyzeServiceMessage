using System;
using System.Configuration;

namespace AnalyzeServiceMessage
{
    public class Libs
    {
        public static bool CheckSchedule()
        {
            int hourSchedule = SetParams.DefaultSchedule;
            try
            {
                hourSchedule = int.Parse(ConfigurationManager.AppSettings["TimeSchedule"]);
            }
            catch (Exception) { }
            
            DateTime timenow = DateTime.Now;
            int time24h = timenow.TimeOfDay.Hours;
            if (time24h == hourSchedule)
                return true;
            else
                return false;
        }
    }
    public class SetParams
    {
        public static int DefaultSchedule { get; private set; } = 23;
        public static double IntervalEnlapsed { get;private set; } = 300000;

    }
}
