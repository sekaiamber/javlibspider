using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using sekaiav.Utils;
using sekaiav.Models;

namespace sekaiav
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private void bt_catch_video_info_Click(object sender, RoutedEventArgs e)
        {
            string vers = Guid.NewGuid().ToString("N");
            for (int i = 0; i < Config.ThreadCount; i++)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate(object s)
                {
                    int thindex = (int)s;
                    CatchVideoInfo(vers, Config.ThreadCount, thindex);
                }));
                this.bt_catch_actress.IsEnabled = false;
                th.IsBackground = true;
                th.Start(i);
            }
        }

        void CatchVideoInfo(string version, int thcount, int thindex)
        {
            while (true)
            {
                var act = Video.GetActressNotVersion(version, thcount, thindex);
                try
                {
                    if (act == null)
                    {
                        break;
                    }
                    string url = string.Format(Config.Actress_video_detail_URL, act.JL_Id);
                    HttpHandler hdler = new HttpHandler();
                    string response = hdler.WebRequest(HttpMethd.GET, url, null);
                    act.UpdateDetailFromHtml(response);
                    act.Save(version);
                    Info("id:" + act.Id + "收录完成", version, act.Id, "videoInfo");
                }
                catch (System.Exception ex)
                {
                    LogHandler.Log(LogLevel.Error, version, "id:" + act.Id + "收录失败:" + ex.Message, act.Id, "videoInfo");
                }

            }
            Info("抓取完成", version, -1, "videoInfo");
        }
    }
}