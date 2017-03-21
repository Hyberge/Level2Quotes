using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    class SinaHistoryDataCaptureTask: ITask
    {
        int mMaxThreadCount = 20;

        List<String> mSymbols = null;

        public SinaHistoryDataCaptureTask(ITask Next): base(Next)
        {
        }

        public override bool TransactionProcessing()
        {
            bool ret = false;

            if (mSymbols != null)
            {
                List<Thread> SubTask = new List<Thread>();

                AtomLock Lock = new AtomLock();
                int Index = 0;
                for (int i = 0; i < mMaxThreadCount; ++i)
                {
                    Thread NewThread = new Thread(o =>
                    {
                        bool Running = true;
                        while (Running)
                        {
                            String Symbol = String.Empty;
                            Lock.Lock();
                            {
	                            if (Index < mSymbols.Count)
	                            {
                                    Symbol = mSymbols[Index++];
	                            }
                                else
                                {
                                    Running = false;
                                }
                                Lock.Unlock();
                            }
	                        if (Symbol != String.Empty)
	                        {
		                        DataCapture.SinaStockHistoryCapture Capture = DataCapture.StockQuotesManager.Instance().CreateSinaStockHistoryCapture();
	                            Capture.GetTodayHistoryTransactionData(Symbol);
	                        }
                        }
                    });
                    NewThread.Start();

                    SubTask.Add(NewThread);
                }

                ret = true;

                while (true)
                {
                    bool AllCompleted = true;

                    foreach (var ele in SubTask)
                    {
                        AllCompleted &= (ele.ThreadState == ThreadState.Stopped);

                        ret &= (ele.ThreadState != ThreadState.Aborted);
                    }

                    if (AllCompleted)
                        break;

                    Thread.Sleep(30000);
                }
            }

            return ret;
        }

        public void AddSymbolsList(List<String> Symbols)
        {
            mSymbols = Symbols;
        }

        public void SetThreadCount(int Count)
        {
            mMaxThreadCount = Count;
        }
    }
}
