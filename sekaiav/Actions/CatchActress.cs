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
        private void bt_catch_actress_Click(object sender, RoutedEventArgs e)
        {
            Thread th = new Thread(new ThreadStart(delegate
            {
                CatchActress();
            }));
            this.bt_catch_actress.IsEnabled = false;
            th.Start();
        }

        void CatchActress()
        {
            string version = Guid.NewGuid().ToString("N");
            string[] prefixs = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            int page = 1;
            foreach (var prefix in prefixs)
            {
                while (page < 100)
                {
                    string url = string.Format(Config.Actress_URL, prefix, page);
                    HttpHandler hdler = new HttpHandler();
                    string response = hdler.WebRequest(HttpMethd.GET, url, null);
                    Actress[] acts = Actress.GetActressFromHtml(response);
                    if (acts.Length > 0)
                    {
                        foreach (var act in acts)
                        {
                            act.Prefix = prefix;
                            act.Save(version);
                        }
                        Info(prefix + "开头女优第" + page + "页收录" + acts.Length + "个", version, -1, "actress");
                    }
                    else
                    {
                        break;
                    }
                    page++;
                }
                page = 1;
            }
        }
    }
}
