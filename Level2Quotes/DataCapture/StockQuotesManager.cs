using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataCapture
{
    class StockQuotesManager
    {
        bool mIsLogin = false;

        String mUid = String.Empty;
        String mPassWD = String.Empty;

        SinaAPI mSina = new SinaAPI();

        List<String> mSymbols = new List<String>();
        List<int> mIntSymbols = new List<int>();

        volatile static StockQuotesManager sInstance = null;

        public static StockQuotesManager Instance() 
        {
            if (sInstance == null)
                sInstance = new StockQuotesManager();
            return sInstance; 
        }

        private StockQuotesManager()
        {
        }

        public bool IsLogin()
        {
            return mIsLogin;
        }

        public bool Login(String Uid, String PassWD, String VerifyCode)
        {
            mUid = Uid;
            mPassWD = PassWD;

            mIsLogin = mSina.Login(Uid, PassWD, VerifyCode);

            return mIsLogin;
        }

        public void Logout()
        {
            mSina.Logout();
            mIsLogin = false;
        }

        public void Reset()
        {
            Logout();

            mSymbols.Clear();
            mIntSymbols.Clear();

            mUid = String.Empty;
            mPassWD = String.Empty;
        }

        public SinaAPI GetSinaAPI()
        {
            return mSina;
        }

        public String GetLoginedUserID()
        {
            return mUid;
        }

        public List<String> GetAllSymbol()
        {
            if (mSymbols.Count == 0)
            {
                mSymbols = mSina.GetAllSymbol();

                foreach(var ele in mSymbols)
                {
                    mIntSymbols.Add(Util.SymbolStringToInt(ele));
                }
            }

            return mSymbols;
        }

        public List<int> GetAllIntSymbol()
        {
            if (mIntSymbols.Count == 0)
            {
                mSymbols = mSina.GetAllSymbol();

                foreach (var ele in mSymbols)
                {
                    mIntSymbols.Add(Util.SymbolStringToInt(ele));
                }
            }

            return mIntSymbols;
        }

        public SinaStockSubscription CreateSinaStockCapture(List<int> Symbols = null)
        {
            if (Symbols == null || Symbols.Count == 0)
            {
            	return new SinaStockSubscription(GetAllIntSymbol(), mSina);
            } 
            else
            {
                return new SinaStockSubscription(Symbols, mSina);
            }
        }

        public SinaStockHistoryCapture CreateSinaStockHistoryCapture(List<int> Symbols = null)
        {
            if (Symbols == null || Symbols.Count == 0)
            {
                return new SinaStockHistoryCapture(GetAllIntSymbol(), mSina);
            }
            else
            {
                return new SinaStockHistoryCapture(Symbols, mSina);
            }
        }

        public StockDiskDataSimulation CreateStockDiskDataCapture(List<int> Symbols = null)
        {
            if (Symbols == null || Symbols.Count == 0)
            {
                return new StockDiskDataSimulation(GetAllIntSymbol());
            }
            else
            {
                return new StockDiskDataSimulation(Symbols);
            }
        }
    }
}
