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

        public float LastClose;
        public float OpenPrice;
        public float[] BidPrice = new float[10];     // 买1~10价
        public int[] BidVolume = new int[10];        // 买1~10量
        public float[] AskPrice = new float[10];     // 卖1~10价
        public int[] AskVolume = new int[10];        // 卖1~10量
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
            for (int i=0; i<10; ++i)
            {
                CurData.BidPrice[i] = Quotation.BidPrice[i];
                CurData.BidVolume[i] = Quotation.BidVolume[i];
                CurData.AskPrice[i] = Quotation.AskPrice[i];
                CurData.AskVolume[i] = Quotation.AskVolume[i];
            }
            CurData.mLock.Unlock();
        }

        public static void PushTransactionDataInHub(int Symbol, List<TransactionData> Transaction, bool Clear)
        {
            SymbolData CurData = mDatas[Symbol];

            CurData.mLock.Lock();
            if (Clear)
            {
                CurData.mTransaction = Transaction;
            }
            else
            {
                foreach (var ele in Transaction)
                {
                    CurData.mTransaction.Add(ele);
                }
            }
            CurData.mLock.Unlock();
        }

        public static bool PushTransactionDataInHub(int Symbol, List<TransactionData> NewData, OrdersData Orders, QuotationData Quotation, bool Clear = false)
        {
            bool ret = false;

            if (mDatas.ContainsKey(Symbol))
            {
                SymbolData CurData = mDatas[Symbol];

                CurData.mLock.Lock();

                for (int i = 0; i < 10; ++i)
                {
                    CurData.BidPrice[i] = Quotation.BidPrice[i];
                    CurData.BidVolume[i] = Quotation.BidVolume[i];
                    CurData.AskPrice[i] = Quotation.AskPrice[i];
                    CurData.AskVolume[i] = Quotation.AskVolume[i];
                }

                if (Clear)
                {
                    CurData.mTransaction = NewData;
                }
                else
                {
                    foreach (var ele in NewData)
                    {
                        CurData.mTransaction.Add(ele);
                    }
                }

                CurData.mLock.Unlock();

                ret = true;
            }

            return ret;
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
