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
using sekaiav.Utils;

namespace sekaiav
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void Info(string msg, string version, int id, string action)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                this.tb_msg.Text += msg + "\r\n";
                LogHandler.Log(LogLevel.Info, version, msg, id, action);
            }));
        }
    }
}
