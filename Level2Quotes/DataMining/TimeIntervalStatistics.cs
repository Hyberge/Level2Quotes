using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataMining
{
    class TimeIntervalStatistics : IDataProcesser
    {
        int mBeginTime;
        int mEndTime;
        float mBigOrderThreshold;

        float mCurrentPrice = 0.0f;
        float mHighestPrice = 0.0f;
        float mLowestPrice = 9999999.0f;

        int mTotalVolume = 0;
        int mTotalTransactionNum = 0;
        float mTotalCount = 0.0f;

        float mAveragePrice = 0.0f;
        float mPriceDeviation = 0.0f;

        float mBigOrdersNumberProportion = 0.0f;
        float mBigOrdersCountProportion = 0.0f;

        Dictionary<int, DealData> mBuyDealData = new Dictionary<int, DealData>();
        Dictionary<int, DealData> mSellDealData = new Dictionary<int, DealData>();

        public TimeIntervalStatistics(int Begin, int End, float Threshold)
        {
            mBeginTime = Begin;
            mEndTime = End;
            mBigOrderThreshold = Threshold;
        }

        private void ConversionTransactionData(TransactionData Transaction)
        {
            if (!mBuyDealData.ContainsKey(Transaction.BuyNumber))
            {
                mBuyDealData[Transaction.BuyNumber] = new DealData();
                mBuyDealData[Transaction.BuyNumber].IsProactive = Transaction.BuyNumber > Transaction.SellNumber;
            }
            mBuyDealData[Transaction.BuyNumber].Volume += Transaction.Volume;
            mBuyDealData[Transaction.BuyNumber].Count += Transaction.Count;

            if (!mSellDealData.ContainsKey(Transaction.SellNumber))
            {
                mSellDealData[Transaction.SellNumber] = new DealData();
                mSellDealData[Transaction.SellNumber].IsProactive = Transaction.SellNumber > Transaction.BuyNumber;
            }
            mSellDealData[Transaction.SellNumber].Volume += Transaction.Volume;
            mSellDealData[Transaction.SellNumber].Count += Transaction.Count;

            mCurrentPrice = Transaction.Price;
            mHighestPrice = Math.Max(mHighestPrice, Transaction.Price);
            mLowestPrice = Math.Min(mLowestPrice, Transaction.Price);

            mTotalCount += Transaction.Count;
            mTotalVolume += Transaction.Volume;
            mTotalTransactionNum++;
        }

        private void StatisticsData()
        {
            mAveragePrice = mTotalCount / mTotalVolume;
            mPriceDeviation = (mHighestPrice - mAveragePrice) / (mAveragePrice - mLowestPrice);

            int BuyOrders = 0, SellOrders = 0;
            float BuyCount = 0.0f, SellCount = 0.0f;

            foreach (var ele in mBuyDealData)
            {
                if (ele.Value.Count > mBigOrderThreshold)
                {
                    BuyOrders++;
                    BuyCount += ele.Value.Count;
                }
            }

            foreach (var ele in mSellDealData)
            {
                if (ele.Value.Count > mBigOrderThreshold)
                {
                    SellOrders++;
                    SellCount += ele.Value.Count;
                }
            }

            mBigOrdersNumberProportion = (float)BuyOrders / SellOrders;
            mBigOrdersCountProportion = BuyCount / SellCount;
        }

        public bool Process(SymbolData Data)
        {
            bool ret = false;

            if (mBeginTime < mEndTime && mBeginTime < 150000000 && mEndTime > 93000000)
            {
                for (int i = 0; i < Data.mTransaction.Count; ++i)
                {
                    if (Data.mTransaction[i].TradingTime > mEndTime)
                        break;

                    if (Data.mTransaction[i].TradingTime > mBeginTime)
                        ConversionTransactionData(Data.mTransaction[i]);
                }

                StatisticsData();

                ret = true;
            }

            return ret;
        }

        public float GetPriceDeviation()
        {
            return mPriceDeviation;
        }

        public float GetBigOrdersNumberProportion()
        {
            return mBigOrdersNumberProportion;
        }

        public float GetBigOrdersCountProportion()
        {
            return mBigOrdersCountProportion;
        }
    }
}
