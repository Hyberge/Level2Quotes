using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using mshtml;
using System.Threading;
using Level2Quotes.Task;
using System.IO;

namespace Level2Quotes
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Pin = new WebBrowser();
            Pin.Navigate(new Uri("http://login.sina.com.cn/cgi/pin.php"));
        }

        private void Pin_LoadCompleted(object sender, NavigationEventArgs e)
        {
            mshtml.HTMLDocument Dom = (mshtml.HTMLDocument)Pin.Document;

            Dom.documentElement.style.overflow = "hidden";    //隐藏浏览器的滚动条 
            Dom.body.setAttribute("scroll", "no");            //禁用浏览器的滚动条
        }

        private void Flush_Click(object sender, RoutedEventArgs e)
        {
            Pin.Navigate(new Uri("http://login.sina.com.cn/cgi/pin.php"));
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            String Uid = UserID.Text;
            String Pwd = PassWD.Password;
            String VerifyCode = Verify.Text;

            if (Uid == String.Empty)
            {
                MessageBox.Show("请输入账户 !!!");
                return;
            }
            if (Pwd == String.Empty)
            {
                MessageBox.Show("请输入密码 !!!");
                return;
            }

            DataCapture.StockCaptureManager.Instance().Login(Uid, Pwd, VerifyCode);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            SinaDataCaptureTask Task = new SinaDataCaptureTask(null);
            Task.AddSymbolsList(DataCapture.StockCaptureManager.Instance().GetAllIntSymbol());
            Task.DoTask();
            //DirectoryInfo Dir = new DirectoryInfo("../../HTD/");
            //String FileName = Verify.Text;
            //
            //foreach (var item in Dir.GetDirectories())
            //{
            //    if (File.Exists(item.FullName + "/2017-03-16"))
            //    {
            //        File.Move(item.FullName + "/2017-03-16", item.FullName + "/2017-03-15");
            //    }
            //}

            //List<int> Symbols = DataCapture.StockCaptureManager.Instance().GetAllIntSymbol();
            //DailyStatisticsTask Task0 = new DailyStatisticsTask(null);
            //Task0.AddSymbolsList(Symbols);
            //
            //DiskDataCaptureTask Task1 = new DiskDataCaptureTask(Task0);
            //Task1.AddSymbolsList(Symbols);
            //Task1.SetNeededDay(DateTime.Today.AddDays(-1));
            //
            //new Thread(o =>
            //{
            //    Task1.DoTask();
            //}).Start();
        }

        private void DownLoad_Click(object sender, RoutedEventArgs e)
        {
            if (DataCapture.StockCaptureManager.Instance().IsLogin())
            {
                String Path = "../../HTD/";
                List<String> MissingSymbol = new List<String>();
                List<String> Symbols = DataCapture.StockCaptureManager.Instance().GetAllSymbol();
                foreach(var ele in Symbols)
                {
                    if (!Directory.Exists(Path + ele))
                    {
                        MissingSymbol.Add(ele);
                    }
                }
                SinaHistoryDataCaptureTask Task = new SinaHistoryDataCaptureTask(null);
                Task.AddSymbolsList(MissingSymbol);

                new Thread(o =>
                {
                    Task.DoTask();
                }).Start();
            } 
            else
            {
                MessageBox.Show("请先登陆 !!!");
            }
        }
    }
}
