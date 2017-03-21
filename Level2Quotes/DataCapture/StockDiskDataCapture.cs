using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Level2Quotes.DataCapture
{
    class StockDiskDataCapture
    {
        List<int> mSymbols = null;
        public StockDiskDataCapture(List<int> Symbols)
        {
            mSymbols = Symbols;
        }

        public void SimulationCapture(DateTime Day)
        {
            if (mSymbols == null || mSymbols.Count == 0)
                return;

            DataMining.DataHub.InitDataHubSymbolList(mSymbols);

            foreach (var ele in mSymbols)
            {
                if (FileWriter.IsTransactionDataExists(Util.SymbolIntToString(ele), Day))
                {
                    List<TransactionData> Data = new List<TransactionData>();

                    FileWriter.ReadTransactionDataFromFile(Util.SymbolIntToString(ele), Day, Data);

                    DataMining.DataHub.PushTransactionDataInHub(ele, Data, true);
                }
            }
        }
    }
}
