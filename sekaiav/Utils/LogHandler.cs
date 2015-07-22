using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sekaiav.Models;

namespace sekaiav.Utils
{
    public enum LogLevel
    {
        Info = 1,
        Error = 2
    }
    public class LogHandler
    {
        public static void Log(LogLevel lv, string version, string msg, int id, string action)
        {
            using (SekaiAVDataDataContext db = new SekaiAVDataDataContext())
            {
                t_log log = new t_log()
                {
                    f_datetime = DateTime.Now,
                    f_level = (int)lv,
                    f_msg = msg,
                    f_version = version,
                    f_id = id,
                    f_action = action
                };
                db.t_log.InsertOnSubmit(log);
                db.SubmitChanges();
            }
        }
    }
}
