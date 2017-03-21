using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    class TimeIntervalStatisticsTask : ITask
    {
        AtomLock mLock = new AtomLock();

        int mPerThreadCount = 200;
        float mStatisticThreshold = 1000000.0f;

        List<int> mSymbols = null;

        public List<DataMining.StatisticsData> mResult = new List<DataMining.StatisticsData>();

        public TimeIntervalStatisticsTask(ITask Next): base(Next)
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
                        foreach (var ele in SubSymbols)
                        {
                            DataMining.TimeIntervalStatistics Processer = new DataMining.TimeIntervalStatistics(93000000, 100000000, 500000.0f);
                            DataMining.DataHub.SubmitProcesser(ele, Processer);

                            DataMining.StatisticsData Data = new DataMining.StatisticsData();
                            Data.PriceDeviation = Processer.GetPriceDeviation();
                            Data.BigOrdersNumberProportion = Processer.GetBigOrdersNumberProportion();
                            Data.BigOrdersCountProportion = Processer.GetBigOrdersCountProportion();

                            mLock.Lock();
                            mResult.Add(Data);
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
