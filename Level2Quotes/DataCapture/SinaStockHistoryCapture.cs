using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Level2Quotes.DataCapture
{
    class SinaStockHistoryCapture
    {
        SinaAPI mSina = null;

        List<int> mSymbols = null;

        public SinaStockHistoryCapture(List<int> Symbols, SinaAPI Sina)
        {
            mSina = Sina;
            mSymbols = Symbols;
        }

        public void GetTodayHistoryTransactionData()
        {
            foreach (var ele in mSymbols)
            {
                List<TransactionData> Datas = mSina.GetHistoryTransactionData(Util.SymbolIntToString(ele));

                FileWriter.WriteTransactionDataToFile(Util.SymbolIntToString(ele), Datas, true);
            }
        }

        public void GetTodayHistoryTransactionData(String Symbol)
        {
            List<TransactionData> Datas = mSina.GetHistoryTransactionData(Symbol);

            FileWriter.WriteTransactionDataToFile(Symbol, Datas, true);
        }
    }
}
