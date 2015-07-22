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
            string[][] fixtures = new string[][] 
            {
                new string[] {"z", "p", "v", "l", "b", "d", "g", "c", "j", "r", "e", "w", "u", "f" },
                new string[] {"o", "h"},
                new string[] {"i", "t"},
                new string[] {"y", "n"},
                new string[] {"a"},
                new string[] {"m"},
                new string[] {"s"},
                new string[] {"k"}
            };
            for (int i = 0; i < fixtures.Length; i++)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate(object s)
                {
                    var fixture = s as string[];
                    CatchVideo(vers, fixture);
                }));
                this.bt_catch_actress.IsEnabled = false;
                th.Start(fixtures[i]);
            }
        }

        void CatchVideo(string version, string[] fixture)
        {
            while (true)
            {
                var act = Actress.GetActressNotVersion(version, true, fixture);
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
