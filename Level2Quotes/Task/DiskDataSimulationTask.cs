using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    class DiskDataSimulationTask: ITask
    {
        int mPerThreadCount = 500;

        DateTime mNeedDay = DateTime.Today;

        List<int> mSymbols = null;

        public DiskDataSimulationTask(ITask Next): base(Next)
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
                        DataCapture.StockDiskDataSimulation Processer = DataCapture.StockQuotesManager.Instance().CreateStockDiskDataSimulation(SubSymbols);
                        Processer.SetSimulationType(Level2DataType.Transaction & Level2DataType.Quotation);

                        Processer.SimulationCapture(mNeedDay);

                        GC.Collect();
                        GC.WaitForPendingFinalizers();
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

        public void SetNeededDay(DateTime Day)
        {
            mNeedDay = Day;
        }

        public void SetPerThreadCount(int Count)
        {
            mPerThreadCount = Count;
        }
    }
}
