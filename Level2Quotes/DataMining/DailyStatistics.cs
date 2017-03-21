using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataMining
{
    class DailyStatistics: IDataProcesser
    {
        Dictionary<int, DealData> mBuyDealData = new Dictionary<int, DealData>();
        Dictionary<int, DealData> mSellDealData = new Dictionary<int, DealData>();

        private void StatisticData(List<TransactionData> Transaction)
        {
            foreach (var ele in Transaction)
            {
                if (!mBuyDealData.ContainsKey(ele.BuyNumber))
                {
                    mBuyDealData[ele.BuyNumber] = new DealData();
                    mBuyDealData[ele.BuyNumber].IsProactive = ele.BuyNumber > ele.SellNumber;
                }
                mBuyDealData[ele.BuyNumber].Volume += ele.Volume;
                mBuyDealData[ele.BuyNumber].Count += ele.Count;

                if (!mSellDealData.ContainsKey(ele.SellNumber))
                {
                    mSellDealData[ele.SellNumber] = new DealData();
                    mSellDealData[ele.SellNumber].IsProactive = ele.SellNumber > ele.BuyNumber;
                }
                mSellDealData[ele.SellNumber].Volume += ele.Volume;
                mSellDealData[ele.SellNumber].Count += ele.Count;
            }
        }

        public bool Process(SymbolData Data)
        {
            StatisticData(Data.mTransaction);

            return true;
        }

        public TradingComparison GetDealData(int VolumeThreshold)
        {
            TradingComparison Comp = new TradingComparison();

            foreach (var ele in mBuyDealData)
            {
                if (ele.Value.Volume >= VolumeThreshold)
                {
                    Comp.BigBidNumber++;
                    Comp.BigBidVolume += ele.Value.Volume;
                    Comp.BigBidCount += ele.Value.Count;

                    if (ele.Value.IsProactive)
                    {
                        Comp.PBigBidNumber++;
                        Comp.PBigBidVolume += ele.Value.Volume;
                        Comp.PBigBidCount += ele.Value.Count;
                    }
                }
            }

            foreach (var ele in mSellDealData)
            {
                if (ele.Value.Volume >= VolumeThreshold)
                {
                    Comp.BigAskNumber++;
                    Comp.BigAskVolume += ele.Value.Volume;
                    Comp.BigAskCount += ele.Value.Count;

                    if (ele.Value.IsProactive)
                    {
                        Comp.PBigAskNumber++;
                        Comp.PBigAskVolume += ele.Value.Volume;
                        Comp.PBigAskCount += ele.Value.Count;
                    }
                }
            }

            return Comp;
        }

        public TradingComparison GetDealData(float CountThreshold)
        {
            TradingComparison Comp = new TradingComparison();

            foreach (var ele in mBuyDealData)
            {
                if (ele.Value.Count >= CountThreshold)
                {
                    Comp.BigBidNumber++;
                    Comp.BigBidVolume += ele.Value.Volume;
                    Comp.BigBidCount += ele.Value.Count;

                    if (ele.Value.IsProactive)
                    {
                        Comp.PBigBidNumber++;
                        Comp.PBigBidVolume += ele.Value.Volume;
                        Comp.PBigBidCount += ele.Value.Count;
                    }
                }
            }

            foreach (var ele in mSellDealData)
            {
                if (ele.Value.Count >= CountThreshold)
                {
                    Comp.BigAskNumber++;
                    Comp.BigAskVolume += ele.Value.Volume;
                    Comp.BigAskCount += ele.Value.Count;

                    if (ele.Value.IsProactive)
                    {
                        Comp.PBigAskNumber++;
                        Comp.PBigAskVolume += ele.Value.Volume;
                        Comp.PBigAskCount += ele.Value.Count;
                    }
                }
            }

            return Comp;
        }
    }
}
