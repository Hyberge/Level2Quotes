using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Level2Quotes
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ParametersRun autoRun = new ParametersRun();
            autoRun.ParametersParse(e.Args);

            base.OnStartup(e);
        }
    }
}
