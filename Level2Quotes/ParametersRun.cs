using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Level2Quotes.Task;
using System.Threading;

namespace Level2Quotes
{
    class ParametersRun
    {

        public void ParametersParse(String[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Contains("DownLoadData") && args.Length > i + 2)
                {
                    AutoDownLoad(args[++i], args[++i]);
                }
            }
        }

        private void AutoDownLoad(String Uid, String PassWD)
        {
            if (DataCapture.StockQuotesManager.Instance().Login(Uid, PassWD, ""))
            {
                EMailSendTask Task2 = new EMailSendTask(null);
                Task2.SetEmailContents("数据爬取报告", "主人:\r\n    任务结束!!!!");

                SinaHistoryDataCaptureTask Task1 = new SinaHistoryDataCaptureTask(Task2);
                Task1.AddSymbolsList(DataCapture.StockQuotesManager.Instance().GetAllSymbol());

                EMailSendTask Task0 = new EMailSendTask(Task1);
                Task0.SetEmailContents("数据爬取报告", "主人:\r\n    任务启动!!!!");

                new Thread(o =>
                {
                    Task0.DoTask();

                    Environment.Exit(0);
                }).Start();
            }
        }
    }
}
