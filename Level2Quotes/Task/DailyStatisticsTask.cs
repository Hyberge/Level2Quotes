using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    class DailyStatisticsTask: ITask
    {
        AtomLock mLock = new AtomLock();

        int mPerThreadCount = 200;
        float mStatisticThreshold = 1000000.0f;

        List<int> mSymbols = null;

        Dictionary<int, DataMining.TradingComparison> mOutData = new Dictionary<int, DataMining.TradingComparison>();
 
        public DailyStatisticsTask(ITask Next): base(Next)
        {

        }

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
                        foreach (var ele in SubSymbols)
                        {
                            DataMining.DailyStatistics Processer = new DataMining.DailyStatistics();
                            DataMining.DataHub.SubmitProcesser(ele, Processer);

                            mLock.Lock();
                            mOutData[ele] = Processer.GetDealData(mStatisticThreshold);
                            mLock.Unlock();
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

                    Thread.Sleep(100);
                }
            }

            return ret;
        }

        public override void StopProcessing()
        {

        }

        public void AddSymbolsList(List<int> Symbols)
        {
            mSymbols = Symbols;
        }

        public void SetPerThreadCount(int Count)
        {
            mPerThreadCount = Count;
        }

        public void SetStatisticThreshold(int Threshold)
        {
            mStatisticThreshold = Threshold;
        }
    }
}
