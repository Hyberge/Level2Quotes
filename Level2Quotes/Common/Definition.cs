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

        public void Copy(TransactionData Src)
        {
            Index = Src.Index;
            TradingTime = Src.TradingTime;
            Price = Src.Price;
            Volume = Src.Volume;
            Count = Src.Count;
            BuyNumber = Src.BuyNumber;
            SellNumber = Src.SellNumber;
            IOType = Src.IOType;
            Channel = Src.Channel;
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
    public class QuotationData
    {
        //public String Symbol;
        public String Name;
        public String TradingTime;
        public float LastClose;
        public float OpenPrice;
        public float HighPrice;
        public float LowPrice;
        public float NowPrice;
        public TradingStatus Status;
        public int   TransactionCount; // 成交笔数
        public int   TotalVolume;      // 成交总量
        public float TotalAmount;      // 总成交金额
        public float CurBidAmount;     // 当前委买总金额
        public float AverBidPrice;     // 加权平均委买价格
        public float CurAskAmount;     // 当前委卖总金额
        public float AverAskPrice;     // 加权平均委卖价格
        public int   CancelBidNum;     // 买入撤单笔数
        public float CancelBidAmount;  // 买入撤单金额
        public int   CancelAskNum;     // 卖出撤单笔数
        public float CancelAskAmount;  // 卖出撤金额
        public int   TotalBidNum;      // 委买总笔数
        public int   TotalAskNum;      // 委卖总笔数
        public float[] BidPrice = new float[10];     // 买1~10价
        public int[]   BidVolume = new int[10];      // 买1~10量
        public float[] AskPrice = new float[10];     // 卖1~10价
        public int[]   AskVolume = new int[10];      // 卖1~10量

        public void Copy(QuotationData Src)
        {
            Name = Src.Name;
            TradingTime = Src.TradingTime;
            LastClose = Src.LastClose;
            OpenPrice = Src.OpenPrice;
            HighPrice = Src.HighPrice;
            LowPrice = Src.LowPrice;
            NowPrice = Src.NowPrice;
            Status = Src.Status;
            TransactionCount = Src.TransactionCount;
            TotalVolume = Src.TotalVolume;
            TotalAmount = Src.TotalAmount;
            CurBidAmount = Src.CurBidAmount;
            AverBidPrice = Src.AverBidPrice;
            CurAskAmount = Src.CurAskAmount;
            AverAskPrice = Src.AverAskPrice;
            CancelBidNum = Src.CancelBidNum;
            CancelBidAmount = Src.CancelBidAmount;
            CancelAskNum = Src.CancelAskNum;
            CancelAskAmount = Src.CancelAskAmount;
            TotalBidNum = Src.TotalBidNum;
            TotalAskNum = Src.TotalAskNum;

            for (int i=0; i<10; ++i)
            {
                BidPrice[i] = Src.BidPrice[i];
                BidVolume[i] = Src.BidVolume[i];
                AskPrice[i] = Src.AskPrice[i];
                AskVolume[i] = Src.AskVolume[i];
            }
        }
    }
}