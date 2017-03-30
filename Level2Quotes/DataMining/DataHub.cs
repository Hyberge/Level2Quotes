using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataMining
{
    public class SymbolData
    {
        public AtomLock mLock = new AtomLock();

        public BaseStatus mBase = new BaseStatus();
        public CrossQuotesStatus mCrossQuotes = new CrossQuotesStatus();
        public List<StatisticsStatus> mStatistics = new List<StatisticsStatus>();
        public List<TransactionData> mTransaction = new List<TransactionData>();
    }

    /*
     * DataHub仅仅是数据容器，不负责任何数据管理工作
     */
    class DataHub
    {
        static AtomLock mLock = new AtomLock();
        static Dictionary<int, SymbolData> mDatas = new Dictionary<int,SymbolData>();

        public static void InitDataHubSymbolList(List<int> SymbolList)
        {
            mLock.Lock();
            foreach(var ele in SymbolList)
            {
                mDatas[ele] = new SymbolData();
            }
            mLock.Unlock();
        }

        public static void ClearData(List<int> SymbolList)
        {
            mLock.Lock();
            foreach (var ele in SymbolList)
            {
                mDatas.Remove(ele);
            }
            mLock.Unlock();
        }

        public static void ClearAllData()
        {
            mDatas.Clear();
        }

        public static void PushOrdersDataInHub(int Symbol, OrdersData Orders)
        {
            
        }

        public static void PushQuotationDataInHub(int Symbol, QuotationData Quotation)
        {
            SymbolData CurData = mDatas[Symbol];

            CurData.mLock.Lock();

            CurData.mBase.Copy(Quotation.Base);
            CurData.mCrossQuotes.Copy(Quotation.CrossQuotes);

            StatisticsStatus Statistics = new StatisticsStatus();
            Statistics.Copy(Quotation.Statistics);
            CurData.mStatistics.Add(Statistics);

            CurData.mLock.Unlock();
        }

        public static void PushQuotationDataInHub(int Symbol, List<QuotationData> Quotation)
        {
            foreach(var ele in Quotation)
            {
                PushQuotationDataInHub(Symbol, ele);
            }
        }

        public static void PushTransactionDataInHub(int Symbol, List<TransactionData> Transaction, bool Clear)
        {
            SymbolData CurData = mDatas[Symbol];

            CurData.mLock.Lock();
            if (Clear)
            {
                CurData.mTransaction.Clear();
            }
            
            foreach (var ele in Transaction)
            {
                CurData.mTransaction.Add(ele);
            }
            
            CurData.mLock.Unlock();
        }

        public static bool SubmitProcesser(int Symbol, IDataProcesser Processer)
        {
            bool ret = false;

            if (mDatas.ContainsKey(Symbol))
            {
                SymbolData CurData = mDatas[Symbol];

                CurData.mLock.Lock();

                Processer.Process(CurData);

                CurData.mLock.Unlock();

                ret = true;
            }

            return ret;
        }
    }
}
