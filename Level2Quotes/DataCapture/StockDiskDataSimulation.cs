using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Level2Quotes.DataCapture
{
    class StockDiskDataSimulation
    {
        List<int> mSymbols = null;

        Level2DataType mDataType;

        public List<TransactionData> Transaction = new List<TransactionData>();

        public List<QuotationData> Quotation = new List<QuotationData>();

        public List<OrdersData> Orders = new List<OrdersData>();

        public StockDiskDataSimulation(List<int> Symbols)
        {
            mSymbols = Symbols;
        }

        public void SetSimulationType(Level2DataType DataType)
        {
            mDataType = DataType;
        }

        public void SimulationCapture(DateTime Day)
        {
            if (mSymbols == null || mSymbols.Count == 0)
                return;

            DataMining.DataHub.InitDataHubSymbolList(mSymbols);

            foreach (var ele in mSymbols)
            {
                String Symbol = Util.SymbolIntToString(ele);

                if (((mDataType&Level2DataType.Transaction) == Level2DataType.Transaction) && FileWriter.IsTransactionDataExists(Symbol, Day))
                {
                    FileWriter.ReadTransactionDataFromFile(Symbol, Day, Transaction);

                    DataMining.DataHub.PushTransactionDataInHub(ele, Transaction, true);
                }

                if (((mDataType & Level2DataType.Transaction) == Level2DataType.Transaction) && FileWriter.IsQuotationDataExists(Symbol, Day))
                {
                    FileWriter.ReadQuotationDataFromFile(Symbol, Day, Quotation);

                    DataMining.DataHub.PushQuotationDataInHub(ele, Quotation);
                }

                if (((mDataType & Level2DataType.Orders) == Level2DataType.Orders) && FileWriter.IsQuotationDataExists(Symbol, Day))
                {
                    
                }
            }
        }
    }
}
