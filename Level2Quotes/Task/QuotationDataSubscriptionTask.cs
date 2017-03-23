using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    enum SubscribeIntervalMode
    {
        SIM_OneMinInterval,
        SIM_FiveMinInterval,
        SIM_TenMinInterval,
        SIM_FiftenMinInterval,
        SIM_HalfHourInterval,
    }
    class QuotationDataSubscriptionTask: ITask
    {
        int mPerThreadCount = 10;

        SubscribeIntervalMode mMode = SubscribeIntervalMode.SIM_FiveMinInterval;

        AtomLock mLock = new AtomLock();

        DateTime mLastCheck;
        
        List<int> mSymbols = null;

        Dictionary<int, QuotationData> mDatas = null;
        Dictionary<int, bool> mFaultTolerance = null;

        public QuotationDataSubscriptionTask(ITask Next): base(Next)
        { }

        public void AddSymbolsList(List<int> Symbols)
        {
            mSymbols = Symbols;

            mDatas = new Dictionary<int,QuotationData>();
            mFaultTolerance = new Dictionary<int, bool>();
            foreach(var ele in mSymbols)
            {
                mDatas.Add(ele, new QuotationData());
                mFaultTolerance.Add(ele, false);
            }
        }

        public void SetPerThreadCount(int Count)
        {
            mPerThreadCount = Count;
        }

        public void SetSubscribeIntervalMode(SubscribeIntervalMode Mode)
        {
            mMode = Mode;
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
                        DataCapture.SinaStockSubscription Capture = DataCapture.StockQuotesManager.Instance().CreateSinaStockCapture(SubSymbols);
                        Capture.SetTerminationCondition(DataCapture.TerminationCondition.TC_MarketClosed);
                        Capture.SetSubscriptionType(Level2DataType.Quotation);
                        Capture.SetDataDelegation(null, this.QuotationDataProcessor, null);
                        Capture.ConnectToSina();
                    });
                    NewThread.Start();

                    SubTask.Add(NewThread);
                }

                ret = true;

                while (true)
                {
                    if (DateTime.Now.Subtract(mLastCheck).Minutes >= GetInterval())
                    {
                        mLastCheck = DateTime.Now;

                        RecordDataToDisk();
                    }

                    bool AllCompleted = true;

                    foreach (var ele in SubTask)
                    {
                        AllCompleted &= (ele.ThreadState == ThreadState.Stopped);

                        ret &= (ele.ThreadState != ThreadState.Aborted);
                    }

                    if (AllCompleted)
                        break;

                    Thread.Sleep(10000);
                }
            }

            return ret;
        }

        private int GetInterval()
        {
            int Interval = 300;

            switch (mMode)
            {
                case SubscribeIntervalMode.SIM_OneMinInterval:
                    Interval = 1;
                    break;
                case SubscribeIntervalMode.SIM_FiveMinInterval:
                    Interval = 5;
                    break;
                case SubscribeIntervalMode.SIM_TenMinInterval:
                    Interval = 10;
                    break;
                case SubscribeIntervalMode.SIM_FiftenMinInterval:
                    Interval = 15;
                    break;
                case SubscribeIntervalMode.SIM_HalfHourInterval:
                    Interval = 30;
                    break;
                default:
                    break;
            }

            return Interval;
        }

        private void QuotationDataProcessor(int Symbol, QuotationData Quotation)
        {
            mLock.Lock();
            mLock.Unlock();

            mDatas[Symbol].Copy(Quotation);
            mFaultTolerance[Symbol] = true;
        }

        private void RecordDataToDisk()
        {
            mLock.Lock();

            foreach(var ele in mDatas)
            {
                if (mFaultTolerance[ele.Key] == true)
                {
                	FileWriter.WriteQuotationDataToFile(Util.SymbolIntToString(ele.Key), ele.Value, false);
                    mFaultTolerance[ele.Key] = false;
                }
            }

            mLock.Unlock();
        }
    }
}
