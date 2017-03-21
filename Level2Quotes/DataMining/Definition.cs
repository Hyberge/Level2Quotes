using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataMining
{
    /*
     * 买卖单成交数据
     */
    class DealData
    {
        //public int Number;           // 委托序号
        public int Volume = 0;       // 成交数量
        public float Count = 0;        // 成交金额
        public bool IsProactive = false; // 是否主动挂单
    }

    /*
     * 买卖力量对比
     */
    class TradingComparison
    {
        public int BigBidNumber = 0;     // 大单买入笔数 
        public int BigBidVolume = 0;     // 大单买入手数
        public float BigBidCount = 0;    // 大单买入金额
        public int PBigBidNumber = 0;    // 主动性大单
        public int PBigBidVolume = 0;
        public float PBigBidCount = 0;

        public int BigAskNumber = 0;     // 卖出
        public int BigAskVolume = 0;
        public float BigAskCount = 0;
        public int PBigAskNumber = 0;    // 主动性大单
        public int PBigAskVolume = 0;
        public float PBigAskCount = 0;
    }

    /*
     * 基础统计数据
     */
    struct StatisticsData
    {
        public float OpenPrice;
        public float ClosePrice;
        public float LastClose;

        public float PriceDeviation;
        public float BigOrdersNumberProportion;
        public float BigOrdersCountProportion;
    }

    /*
     * 训练样本数据,Target可以表示涨跌或者涨幅等
     */
    class TrainingExample
    {
        public double[] SampleData = null;
        public double Target = 0;
    }
}