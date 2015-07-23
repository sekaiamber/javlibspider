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
        private void bt_catch_video_Click(object sender, RoutedEventArgs e)
        {
            string vers = Guid.NewGuid().ToString("N");
            // string vers = "32d80c66060747c0a4dcbb4b27d916f9";
            int thcount = 8;
            for (int i = 0; i < thcount; i++)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate(object s)
                {
                    int thindex = (int)s;
                    CatchVideo(vers, thcount, thindex);
                }));
                this.bt_catch_actress.IsEnabled = false;
                th.Start(i);
            }
        }

        void CatchVideo(string version, int thcount, int thindex)
        {
            while (true)
            {
                var act = Actress.GetActressNotVersion(version, true, thcount, thindex);
                try
                {
                    if (act == null)
                    {
                        break;
                    }
                    int page = 1;
                    while (page < 100)
                    {
                        string url = string.Format(Config.Actress_video_list_URL, act.JL_Id, page);
                        HttpHandler hdler = new HttpHandler();
                        string response = hdler.WebRequest(HttpMethd.GET, url, null);
                        Video[] vs = Video.GetVideoFromHtml(response);
                        if (vs.Length > 0)
                        {
                            foreach (var v in vs)
                            {
                                v.Save(version, act.Id);
                            }
                        }
                        else
                        {
                            break;
                        }
                        page++;
                    }
                    Info("id:" + act.Id + "收录完成", version, act.Id, "video");
                }
                catch (System.Exception ex)
                {
                    LogHandler.Log(LogLevel.Error, version, "id:" + act.Id + "收录失败:" + ex.Message, act.Id, "video");
                }
                
            }
            Info("抓取完成", version, -1, "video");
        }
    }
}
