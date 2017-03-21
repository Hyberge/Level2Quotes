using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.Task
{
    enum SubscribeIntervalMode
    {
        SIM_OneMinInterval,
        SIM_FiveMinInterval,
        SIM_TenMinInterval,
        SIM_FiftenMinInterval,
        SIM_HalfHourInterval,
    }
    class QuotationDataSubscriptionTask: ITask
    {
        List<int> mSymbols = null;

        public QuotationDataSubscriptionTask(ITask Next): base(Next)
        { }

        public void AddSymbolsList(List<int> Symbols)
        {
            mSymbols = Symbols;
        }

        public override bool TransactionProcessing()
        {
            bool ret = false;



            return ret;
        }
    }
}
