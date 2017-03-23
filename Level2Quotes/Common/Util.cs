using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes
{
    class Util
    {
        public static int TransactionBeginTime = 93000000;
        public static int TransactionLanuchHourBegin = 113000000;
        public static int TransactionLanuchHourEnd = 130000000;
        public static int TransactionEndTime = 150000000;

        public static int SymbolStringToInt(String Symbol)
        {
            if (Symbol.Length == 8)
                return Convert.ToInt32(Symbol.Substring(2));
            else
                return Convert.ToInt32(Symbol);
        }

        public static String SymbolIntToString(int Symbol)
        {
            if (Symbol >= 600000)
                return "sh" + Symbol.ToString("000000");
            else
                return "sz" + Symbol.ToString("000000");
        }

        public static int TradingTimeStringToInt(String TradingTime)
        {
            int IntTime = -1;
            if (TradingTime.IndexOf(':') == -1)
            {
                IntTime = Convert.ToInt32(TradingTime);
            }
            else
            {
	            IntTime = Convert.ToInt32(TradingTime.Substring(0, 2)) * 10000000;
	            IntTime += Convert.ToInt32(TradingTime.Substring(3, 2)) * 100000;
	            IntTime += Convert.ToInt32(TradingTime.Substring(6, 2)) * 1000;
	            IntTime += Convert.ToInt32(TradingTime.Substring(9, 3));
            }

            return IntTime;
        }

        public static String TradingTimeIntToString(int TradingTime)
        {
            String StrTime = String.Empty;

            int TempInt = TradingTime / 10000000;
            StrTime += TempInt.ToString("00") + ":";

            TempInt = TradingTime % 10000000 / 100000;
            StrTime += TempInt.ToString("00") + ":";

            TempInt = TradingTime % 100000 / 1000;
            StrTime += TempInt.ToString("00") + ".";

            TempInt = TradingTime % 1000;
            StrTime += TempInt.ToString("000");

            return StrTime;
        }

        public static int DateTimeToTradingTime(DateTime Time)
        {
            return Time.Hour * 10000000 + Time.Minute * 100000 + Time.Second * 1000 + Time.Millisecond;
        }

        public static double GetRatioByTime()
        {
            double Ratio = 1;

            if (DateTime.Now.Hour > 12)
            {
                DateTime StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0);
                double Minutes = (DateTime.Now - StartTime).TotalMinutes;
                Ratio += (120 - Minutes) / (120 + Minutes);
            }
            else
            {
                DateTime StartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 30, 0);
                double Minutes = (DateTime.Now - StartTime).TotalMinutes;
                Ratio = 240 / Minutes;
            }

            return Ratio;
        }

        public static bool CheckInTransactionHours()
        {
            int NowTime = DateTimeToTradingTime(DateTime.Now);
            return (NowTime >= TransactionBeginTime && NowTime <= TransactionLanuchHourBegin) ||
                   (NowTime >= TransactionLanuchHourEnd && NowTime <= TransactionEndTime);
        }

        public static bool CheckTransactionStarted()
        {
            return DateTimeToTradingTime(DateTime.Now) >= TransactionBeginTime;
        }

        public static bool CheckTransactionEnded()
        {
            return DateTimeToTradingTime(DateTime.Now) <= TransactionEndTime;
        }
    }
}
