using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataMining
{
    public interface IDataProcesser
    {
        bool Process(SymbolData Data);
    }

    public interface ITickableTask
    {
        void Tick();
    }

    public interface IDataGraph
    {
        void Draw();
    }
}
