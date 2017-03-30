using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes
{
    public enum Level2DataType
    {
        None = 0x0,
        Transaction = 0x1,
        Quotation = 0x2,
        Orders = 0x4,
    }

    /*
     * 买一卖一挂单数据
     */
    public class OrdersData
    {
        //public String Symbol;
        public String TradingTime;
        public float  BidPrice;
        public int    BidVolume;
        public int    BidNumber;
        public float  AskPrice;
        public int    AskVolume;
        public int    AskNumber;
        public List<int> BidOrders = new List<int>();
        public List<int> AskOrders = new List<int>();
    }

    /*
     * 逐笔成交数据
     */
    public struct TransactionData
    {
        //public int   Symbol = -1;
        public int   Index;       // 成交序号
        public int   TradingTime;
        public float Price;
        public int   Volume;
        public float Count;       // 成交金额
        public int   BuyNumber;   // 买单委托序号
        public int   SellNumber;  // 卖单委托序号
        public int   IOType;      // 主动性买卖标识
        public int   Channel;     // 交易所的一个标记

        public void Copy(TransactionData Other)
        {
            Index = Other.Index;
            TradingTime = Other.TradingTime;
            Price = Other.Price;
            Volume = Other.Volume;
            Count = Other.Count;
            BuyNumber = Other.BuyNumber;
            SellNumber = Other.SellNumber;
            IOType = Other.IOType;
            Channel = Other.Channel;
        }
    }

    /*
     *  交易状态
     */
    public enum TradingStatus
    {
        PH, // 盘后
        PZ, // 盘中
        TP, // 停牌
        WX, // 午休
        LT, // 临时停牌
        KJ, // 开盘集合竞价
        ER, // 错误
    }
    /*
     * 10档行情
     */
    public struct BaseStatus
    {
        public float LastClose;
        public float OpenPrice;
        public float HighPrice;
        public float LowPrice;
        public float NowPrice;
        public TradingStatus Status;

        public void Copy(BaseStatus Other)
        {
            LastClose = Other.LastClose;
            OpenPrice = Other.OpenPrice;
            HighPrice = Other.HighPrice;
            LowPrice = Other.LowPrice;
            NowPrice = Other.NowPrice;
            Status = Other.Status;
        }
    }
    public struct StatisticsStatus
    {
        public int TransactionCount; // 成交笔数
        public int TotalVolume;      // 成交总量
        public float TotalAmount;      // 总成交金额
        public float CurBidAmount;     // 当前委买总金额
        public float AverBidPrice;     // 加权平均委买价格
        public float CurAskAmount;     // 当前委卖总金额
        public float AverAskPrice;     // 加权平均委卖价格
        public int CancelBidNum;     // 买入撤单笔数
        public float CancelBidAmount;  // 买入撤单金额
        public int CancelAskNum;     // 卖出撤单笔数
        public float CancelAskAmount;  // 卖出撤金额
        public int TotalBidNum;      // 委买总笔数
        public int TotalAskNum;      // 委卖总笔数

        public void Copy(StatisticsStatus Other)
        {
            TransactionCount = Other.TransactionCount;
            TotalVolume = Other.TotalVolume;
            TotalAmount = Other.TotalAmount;
            CurBidAmount = Other.CurBidAmount;
            AverBidPrice = Other.AverBidPrice;
            CurAskAmount = Other.CurAskAmount;
            AverAskPrice = Other.AverAskPrice;
            CancelBidNum = Other.CancelBidNum;
            CancelBidAmount = Other.CancelBidAmount;
            CancelAskNum = Other.CancelAskNum;
            CancelAskAmount = Other.CancelAskAmount;
            TotalBidNum = Other.TotalBidNum;
            TotalAskNum = Other.TotalAskNum;
        }
    }
    public class CrossQuotesStatus
    {
        public float[] BidPrice = new float[10];     // 买1~10价
        public int[] BidVolume = new int[10];      // 买1~10量
        public float[] AskPrice = new float[10];     // 卖1~10价
        public int[] AskVolume = new int[10];      // 卖1~10量

        public void Copy(CrossQuotesStatus Other)
        {
            for (int i = 0; i < 10; ++i)
            {
                BidPrice[i] = Other.BidPrice[i];
                BidVolume[i] = Other.BidVolume[i];
                AskPrice[i] = Other.AskPrice[i];
                AskVolume[i] = Other.AskVolume[i];
            }
        }
    }
    public class QuotationData
    {
        //public String Symbol;
        public String Name;
        public String TradingTime;

        public BaseStatus Base = new BaseStatus();
        public StatisticsStatus Statistics = new StatisticsStatus();
        public CrossQuotesStatus CrossQuotes = new CrossQuotesStatus();

        public void Copy(QuotationData Other)
        {
            Name = Other.Name;
            TradingTime = Other.TradingTime;

            Base.Copy(Other.Base);
            Statistics.Copy(Other.Statistics);
            CrossQuotes.Copy(Other.CrossQuotes);
        }
    }
}