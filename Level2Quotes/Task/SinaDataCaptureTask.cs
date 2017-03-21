using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    class SinaDataCaptureTask: ITask
    {
        int mPerThreadCount = 10;

        List<int> mSymbols = null;

        public SinaDataCaptureTask(ITask Next): base(Next)
        { }

        public override bool TransactionProcessing()
        {
            bool ret = false;

            if (mSymbols != null)
            {
                List<Thread> SubTask = new List<Thread>();

                int Index = 0;
                while (Index < mSymbols.Count)
                {
                    List<int> SubSymbols = mSymbols.GetRange(Index, Math.Min(mPerThreadCount, mSymbols.Count - Index));
                    Index += mPerThreadCount;

                    Thread NewThread = new Thread(o =>
                    {
                        DataCapture.SinaStockCapture Capture = DataCapture.StockCaptureManager.Instance().CreateSinaStockCapture(SubSymbols);
                        Capture.ConnectToSina();
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

                    Thread.Sleep(100);
                }
            }

            return ret;
        }

        public void AddSymbolsList(List<int> Symbols)
        {
            mSymbols = Symbols;
        }

        public void SetPerThreadCount(int Count)
        {
            mPerThreadCount = Count;
        }
    }
}
