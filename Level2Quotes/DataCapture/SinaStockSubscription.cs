using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.WebSockets;
using System.Threading;

namespace Level2Quotes.DataCapture
{
    enum TerminationCondition
    {
        TC_MarketClosed,
        TC_OneSubscription,
    }

    delegate void OrderDataDelegation(int Symbol, OrdersData Orders);
    delegate void QuotationDataDelegation(int Symbol, QuotationData Quotation);
    delegate void TransactionDataDelegation(int Symbol, List<TransactionData> Transaction);

    class SinaStockSubscription
    {
        Level2DataType mSubscriptionType = Level2DataType.None;

        TerminationCondition mTerminationCondition = TerminationCondition.TC_MarketClosed;

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

        OrderDataDelegation mOrderDataDelegation = null;
        QuotationDataDelegation mQuotationDataDelegation = null;
        TransactionDataDelegation mTransactionDataDelegation = null;

        public SinaStockSubscription(List<int> Symbols, SinaAPI Sina)
        {
            mSymbols = Symbols;
            mSina = Sina;
        }

        public void SetSubscriptionType(Level2DataType DataType)
        {
            mSubscriptionType = DataType;
        }

        public void SetDataDelegation(OrderDataDelegation Order, QuotationDataDelegation Quotation, TransactionDataDelegation Transaction)
        {
            mOrderDataDelegation = Order;
            mQuotationDataDelegation = Quotation;
            mTransactionDataDelegation = Transaction;
        }

        public void SetTerminationCondition(TerminationCondition Condition)
        {
            mTerminationCondition = Condition;
        }

        public async void ConnectToSina()
        {
            if (mSubscriptionType == Level2DataType.None)
                return;

            mRunning = true;

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
                mQuitCode = "Exception: " + ex.Message;
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
                        mQuitCode = "Exception: " + ex.Message;
                    }

                    String Message = System.Text.Encoding.UTF8.GetString(InputSegment.Array, 0, Receivecount);

                    if (Message.Contains("FAILED"))
                        continue;

                    String[] SubMessage = Message.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var ele in SubMessage)
                    {
                        if (ele.Length < 14)
                        {
                            continue;
                        }

                        mTransaction.Clear();

                        int IntSymbol = Util.SymbolStringToInt(ele.Substring(6, 6));

                        if (ele[12] == '=' && mQuotationDataDelegation != null && mSina.ParseQuotationData(ele, ref mQuotation))
                        {
                            mQuotationDataDelegation(IntSymbol, mQuotation);
                        }
                        if ((ele[13] == '0' || ele[13] == '1') && mTransactionDataDelegation != null && mSina.ParseTransactionData(ele, ref mTransaction))
                        {
                            mTransactionDataDelegation(IntSymbol, mTransaction);
                        }
                        if (ele[13] == 'o' && mOrderDataDelegation != null && mSina.ParseOrdersData(ele, ref mOrders))
                        {
                            mOrderDataDelegation(IntSymbol, mOrders);
                        }
                    }

                    if (CheckTerminationCondition())
                    {
                        mQuitCode = "Termination Condition Reached";
                        break;
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
                            mQuitCode = "Exception: " + ex.Message;
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
                    mQuitCode = "WebSocket is terminated by unknown reason";
                    break;
                }
            }

            await mWebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

            await mWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public void Disconnect()
        {
            mRunning = false;
            mQuitCode = "User disconnect";
        }

        private bool CheckTerminationCondition()
        {
            bool ret = false;

            switch (mTerminationCondition)
            {
                case TerminationCondition.TC_MarketClosed:
                    ret = Util.CheckTransactionEnded();
                    break;
                case TerminationCondition.TC_OneSubscription:
                    ret = true;
                    break;
                default:
                    break;
            }

            return ret;
        }
    }
}
