using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataCapture
{
    class StockCaptureManager
    {
        bool mIsLogin = false;

        String mUid;
        String mPassWD;

        SinaAPI mSina = new SinaAPI();

        List<String> mSymbols = new List<String>();
        List<int> mIntSymbols = new List<int>();

        volatile static StockCaptureManager sInstance = null;

        public static StockCaptureManager Instance() 
        {
            if (sInstance == null)
                sInstance = new StockCaptureManager();
            return sInstance; 
        }

        private StockCaptureManager()
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

        public void Stop()
        {
            mIsLogin = false;
        }

        public SinaAPI GetSinaAPI()
        {
            return mSina;
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

        public SinaStockCapture CreateSinaStockCapture(List<int> Symbols = null)
        {
            if (Symbols == null || Symbols.Count == 0)
            {
            	return new SinaStockCapture(GetAllIntSymbol(), mSina);
            } 
            else
            {
                return new SinaStockCapture(Symbols, mSina);
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

        public StockDiskDataCapture CreateStockDiskDataCapture(List<int> Symbols = null)
        {
            if (Symbols == null || Symbols.Count == 0)
            {
                return new StockDiskDataCapture(GetAllIntSymbol());
            }
            else
            {
                return new StockDiskDataCapture(Symbols);
            }
        }
    }
}
