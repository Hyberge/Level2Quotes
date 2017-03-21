using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    class WhollyDataSubscriptionTask: ITask
    {
        int mPerThreadCount = 10;

        List<int> mSymbols = null;

        public WhollyDataSubscriptionTask(ITask Next): base(Next)
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
                        DataCapture.SinaStockSubscription Capture = DataCapture.StockQuotesManager.Instance().CreateSinaStockCapture(SubSymbols);
                        Capture.SetTerminationCondition(DataCapture.TerminationCondition.TC_MarketClosed);
                        Capture.SetSubscriptionType(Level2DataType.Orders & Level2DataType.Quotation & Level2DataType.Transaction);
                        Capture.SetDataDelegation(DataMining.DataHub.PushOrdersDataInHub,
                                                  DataMining.DataHub.PushQuotationDataInHub,
                                                  (int Symbol, List<TransactionData> Transaction) => { DataMining.DataHub.PushTransactionDataInHub(Symbol, Transaction, false); });
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
