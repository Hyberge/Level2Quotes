using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.WebSockets;
using System.Threading;

namespace Level2Quotes.DataCapture
{
    class SinaStockCapture
    {
        SinaL2DataType mSubscriptionType = SinaL2DataType.None;

        List<int> mSymbols = null;

        String mToken;

        SinaAPI mSina;

        ClientWebSocket mWebSocket = new ClientWebSocket();

        bool mRunning = false;

        String mQuitCode = String.Empty;

        int mSendTimes = 0;
        DateTime mNextSend = new DateTime();

        AtomLock mLock = new AtomLock();

        OrdersData mOrders = new OrdersData();
        QuotationData mQuotation = new QuotationData();
        List<TransactionData> mTransaction = new List<TransactionData>();

        public SinaStockCapture(List<int> Symbols, SinaAPI Sina)
        {
            mSymbols = Symbols;
            mSina = Sina;
        }

        public void SetSubscriptionType(SinaL2DataType DataType)
        {
            mSubscriptionType = DataType;
        }

        public async void ConnectToSina()
        {
            if (mSubscriptionType == SinaL2DataType.None)
                return;

            mRunning = true;

            DataMining.DataHub.InitDataHubSymbolList(mSymbols);

            String qList = String.Empty;
            foreach (var ele in mSymbols)
            {
                if (qList != String.Empty)
                    qList += ",";
                qList += mSina.GenerateQList(Util.SymbolIntToString(ele), mSubscriptionType);
            }

            mToken = mSina.GetToken(qList);

            String Url = "ws://ff.sinajs.cn/wskt?token=" + mToken + "&list=" + qList;

            try
            {
                await mWebSocket.ConnectAsync(new Uri(Url), CancellationToken.None);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var InputSegment = new ArraySegment<byte>(new byte[5120]);

            while (mRunning)
            {
                if (mWebSocket.State == WebSocketState.Open)
                {
                    int Receivecount = 0;
                    try
                    {
                        var Result = await mWebSocket.ReceiveAsync(InputSegment, CancellationToken.None);
                        Receivecount = Result.Count;
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    String Message = System.Text.Encoding.UTF8.GetString(InputSegment.Array, 0, Receivecount);
                    String[] SubMessage = Message.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var ele in SubMessage)
                    {
                        if (ele.Length < 14)
                        {
                            continue;
                        }

                        int IntSymbol = Util.SymbolStringToInt(ele.Substring(7, 6));

                        if (ele[12] == '=')
                        {
                            mSina.ParseQuotationData(ele, ref mQuotation);
                            DataMining.DataHub.PushQuotationDataInHub(IntSymbol, mQuotation);
                        }
                        if (ele[13] == '0' || ele[13] == '1')
                        {
                            mTransaction.Clear();
                            mSina.ParseTransactionData(ele, ref mTransaction);
                            DataMining.DataHub.PushTransactionDataInHub(IntSymbol, mTransaction, false);
                        }
                        if (ele[13] == 'o')
                        {
                            mSina.ParseOrdersData(ele, ref mOrders);
                            DataMining.DataHub.PushOrdersDataInHub(IntSymbol, mOrders);
                        }
                    }

                    if (mNextSend < DateTime.Now)
                    {
                        mNextSend = DateTime.Now.AddSeconds(55);
                        mSendTimes++;
                            
                        try
                        {
	                        var SendSegment = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes("*"+mToken));
	                        await mWebSocket.SendAsync(SendSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    if (mSendTimes > 8)
                    {
                        mSendTimes = 0;
                        mToken = mSina.GetToken(qList);
                    }
                }
                else
                {
                    break;
                }
            }

            await mWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public void Disconnect()
        {
            mRunning = false;
            mQuitCode = "Normal";
        }
    }
}
