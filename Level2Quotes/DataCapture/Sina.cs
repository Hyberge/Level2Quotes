using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Level2Quotes.DataCapture
{
    public class SinaAPI
    {
        private CookieContainer Cookies = new CookieContainer();

        private List<String> Query = new List<string>(){ "quotation", "orders", "transaction" };

        private string GetResponseKeyValue(string key, string responseContent)
        {
            Regex regex = new Regex(key + "\\\":\\\"?(.*?)(\\,|\\\")");

            var match = regex.Match(responseContent);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        private string GetLoginRedirectUrl(string response)
        {
            Regex reg = new Regex("location\\.replace\\('(.*)'");
            var match = reg.Match(response);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return string.Empty;
        }

        public bool Login(String Uid, String PassWD, String VerifyCode)
        {
            string uidbase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Uid));

            string url =
                "http://login.sina.com.cn/sso/prelogin.php?entry=finance&callback=sinaSSOController.preloginCallBack&su=&rsakt=mod&checkpin=1&client=ssologin.js(v1.4.18)&_=" +
                DateTime.Now.Ticks;

            HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(new Uri(url)); //获取servertime和 nonce
            webRequest1.CookieContainer = Cookies;
            HttpWebResponse response1 = (HttpWebResponse)webRequest1.GetResponse();
            StreamReader sr1 = new StreamReader(response1.GetResponseStream(), Encoding.UTF8);
            string res = sr1.ReadToEnd();

            string servertime = GetResponseKeyValue("servertime", res);
            string nonce = GetResponseKeyValue("nonce", res);
            string pubkey = GetResponseKeyValue("pubkey", res);
            string rsakv = GetResponseKeyValue("rsakv", res);

            JSRSAUtil rsaUtil = new JSRSAUtil();
            rsaUtil.RSASetPublic(pubkey, "10001");
            var encryPwd = servertime + '\t' + nonce + '\n' + PassWD;
            string password = rsaUtil.RSAEncrypt(encryPwd);//密码RSA加密

            string str = "entry=finance&gateway=1&from=&savestate=30&useticket=0&pagerefer=&vsnf=1&su=" +
                uidbase64 + "&service=sso&servertime=" + servertime + "&nonce=" + nonce + "&pwencode=rsa2&rsakv=" + rsakv + "&sp=" + password +
                "&sr=1920*1080&encoding=UTF-8&cdult=3&domain=sina.com.cn&prelt=72&returntype=TEXT&door=";

            byte[] bytes;
            ASCIIEncoding encoding = new ASCIIEncoding();
            bytes = encoding.GetBytes(str);
            // bytes = System.Text.Encoding.UTF8.GetBytes(HttpUtility.UrlEncode(str));
            HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create("http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.18)&_=" + DateTime.Now.Ticks);
            webRequest2.Method = "POST";
            webRequest2.ContentType = "application/x-www-form-urlencoded";
            webRequest2.ContentLength = bytes.Length;
            webRequest2.CookieContainer = Cookies;

            Stream stream;
            stream = webRequest2.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();

            HttpWebResponse response2 = (HttpWebResponse)webRequest2.GetResponse();
            StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.Default);
            string res2 = sr2.ReadToEnd();

            if (res2.IndexOf("reason") >= 0)
            {
                Console.WriteLine("登录失败...");
                return false;
            }

            return true;
        }

        public void Logout()
        {

        }

        private String GetClientIP()
        {
            String Url = "https://ff.sinajs.cn/?_=" + DateTime.Now.Ticks + "&list=sys_clientip";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(Url));
            webRequest.CookieContainer = Cookies;
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String res = sr.ReadToEnd();
            response.Close();

            return res;
        }

        public String GetToken(String qList)
        {
            String Url = "https://current.sina.com.cn/auth/api/jsonp.php/var%20KKE_auth_OSfOoonMj=/AuthSign_Service.getSignCode?query=hq_pjb&ip=218.24.136.211" + "&_=0.199&kick=1&list=" + qList;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(Url));
            webRequest.CookieContainer = Cookies;
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String res = sr.ReadToEnd();
            response.Close();

            String[] subStr = res.Split(new char[]{'\"'});

            if (subStr.Length > 2 && subStr[0].IndexOf("msg_code:1") != -1)
            {
            	return subStr[1];
            }
            else
            {
                Console.WriteLine("Token获取失败...");
                return "";
            }
        }

        public String GenerateQList(String Symbol, Level2DataType DataType)
        {
            String QList = String.Empty;

            // 3秒一条的Level2 10档行情
            if ((DataType & Level2DataType.Quotation) != 0)
            {
                QList += "2cn_" + Symbol;
            }

            // 逐笔数据
            if ((DataType & Level2DataType.Transaction) != 0)
            {
                if (QList != String.Empty)
                    QList += ",";
                QList += "2cn_" + Symbol + "_0";
                QList += ",";
                QList += "2cn_" + Symbol + "_1";
            }

            // 挂单数据
            if ((DataType & Level2DataType.Orders) != 0)
            {
                if (QList != String.Empty)
                    QList += ",";
                QList += "2cn_" + Symbol + "_orders";
            }

            return QList;
        }

        public List<String> GetAllSymbol()
        {
            // 所有沪深A股
            String Url = "http://vip.stock.finance.sina.com.cn/quotes_service/api/json_v2.php/Market_Center.getHQNodeData?num=5000&sort=symbol&asc=0&node=hs_a&symbol=&_s_r_a=page&page=1";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(Url));
            webRequest.CookieContainer = Cookies;
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr3 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            String res = sr3.ReadToEnd();
            response.Close();

            String[] subStr = res.Split(new String[] { "{symbol:" }, StringSplitOptions.RemoveEmptyEntries);

            List<String> Symbols = new List<string>();

            foreach(var Symbol in subStr)
            {
                if (Symbol.Length > 8)
                {
                	Symbols.Add(Symbol.Substring(1, 8));
                }
            }

            return Symbols;
        }

        public List<TransactionData> GetHistoryTransactionData(String Symbol)
        {
            List<TransactionData> ret = new List<TransactionData>();

            int Page = 1;
            while (true)
            {
                String res;
	            String Url = "http://stock.finance.sina.com.cn/stock/api/openapi.php/StockLevel2Service.getTransactionList?symbol=" +
                    Symbol + "&callback=jQuery17209838373235483986_" + DateTime.Now.Ticks + "&pageNum=10000&page=" + Page + "&stime=09:25:00&etime=15:05:00&sign=&num=20&_=" + DateTime.Now.Ticks;
	            try
	            {
		            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(Url));
		            webRequest.CookieContainer = Cookies;
		            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
		            StreamReader sr3 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
		            res = sr3.ReadToEnd();
	                response.Close();
	            }
	            catch (System.Exception ex)
	            {
                    Console.WriteLine(ex.Message);
                    continue;
	            }

                int StartIndex = res.IndexOf('[');
                int EndIndex = res.IndexOf(']');
                if (res == String.Empty || StartIndex == -1 || EndIndex == -1)
                {
                    break;
                }

                String[] strDatas = res.Substring(StartIndex + 1, EndIndex - StartIndex - 1).Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var ele in strDatas)
                {
                    if (ele.Length == 1)
                        continue;

                    String[] strTDatas = ele.Split(new String[] { "\":\"", "\",\"", "\"" }, StringSplitOptions.RemoveEmptyEntries);

                    if (strTDatas.Length < 15)
                        continue;

                    TransactionData TData = new TransactionData();
                    TData.Index = Convert.ToInt32(strTDatas[3]);
                    TData.Price = (float)Convert.ToDouble(strTDatas[11]);
                    TData.Volume = (int)Convert.ToDouble(strTDatas[13]);
                    TData.Count = (float)Convert.ToDouble(strTDatas[15]);
                    TData.BuyNumber = Convert.ToInt32(strTDatas[17]);
                    TData.SellNumber = Convert.ToInt32(strTDatas[19]);
                    TData.IOType = Convert.ToInt32(strTDatas[21]);
                    TData.Channel = Convert.ToInt32(strTDatas[23]);
                    TData.TradingTime = Convert.ToInt32(strTDatas[9]);

                    if(strTDatas[7] != Util.TradingTimeIntToString(TData.TradingTime) ||
                        TData.TradingTime != Util.TradingTimeStringToInt(strTDatas[7]))
                    {
                        Console.WriteLine("cacaca!!!!");
                    }

                    ret.Add(TData);
                }
                Page++;
            }

            return ret;
        }

        public bool ParseOrdersData(String Message, ref OrdersData Orders)
        {
            String[] SubMessage = Message.Substring(20).Split(new char[] { ',' });

            Orders.TradingTime = SubMessage[0];
            Orders.BidPrice = (float)Convert.ToDouble(SubMessage[2]);
            Orders.BidVolume = Convert.ToInt32(SubMessage[3]) / 100;
            Orders.BidNumber = Convert.ToInt32(SubMessage[4]);
            Orders.AskPrice = (float)Convert.ToDouble(SubMessage[5]);
            Orders.AskVolume = Convert.ToInt32(SubMessage[6]) / 100;
            Orders.AskNumber = Convert.ToInt32(SubMessage[7]);

            String[] BidOrders = SubMessage[8].Split(new char[] { '|' });
            foreach (var ele in BidOrders)
            {
                if (ele == String.Empty)
                    continue;
                Orders.BidOrders.Add(Convert.ToInt32(ele) / 100);
            }

            String[] AskOrders = SubMessage[10].Split(new char[] { '|' });
            foreach (var ele in AskOrders)
            {
                if (ele == String.Empty)
                    continue;
                Orders.AskOrders.Add(Convert.ToInt32(ele) / 100);
            }

            return true;
        }
        public bool ParseTransactionData(String Message, ref List<TransactionData> Transaction)
        {
            bool bUseChannelSeparator = false;
            String Separator = ",";
            if (Message.IndexOf(',') == -1)
            {
	            String[] SubChannel = Message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
	            if (SubChannel.Length < 9)
	            {
	                Console.WriteLine("Transaction Data Error !!!");
	                return false;
	            }
                Separator = SubChannel[8].Substring(0, 4);
                bUseChannelSeparator = true;
            }

            String[] SubMessage = Message.Substring(15).Split(new String[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var ele in SubMessage)
            {
                String[] Datas = ele.Split(new char[] { '|' });

                if (Datas.Length < 9)
                {
                    Console.WriteLine("Transaction Data Error !!!");
                    continue;
                }

                TransactionData TData = new TransactionData();

                TData.Index = Convert.ToInt32(Datas[0]);
                TData.TradingTime = Util.TradingTimeStringToInt(Datas[1]);
                TData.Price = (float)Convert.ToDouble(Datas[2]);
                TData.Volume = Convert.ToInt32(Datas[3]) / 100;
                TData.Count = (float)Convert.ToDouble(Datas[4]);
                TData.BuyNumber = Convert.ToInt32(Datas[5]);
                TData.SellNumber = Convert.ToInt32(Datas[6]);
                TData.IOType = Convert.ToInt32(Datas[7]);
                TData.Channel = Convert.ToInt32(bUseChannelSeparator ? Separator : Datas[8]);

                Transaction.Add(TData);
            }

            return true;
        }
        private TradingStatus ConvertToTradingStatus(String Status)
        {
            if (Status == "PH")
                return TradingStatus.PH;
            else if (Status == "PZ")
                return TradingStatus.PZ;
            else if (Status == "TP")
                return TradingStatus.TP;
            else if (Status == "WX")
                return TradingStatus.WX;
            else if (Status == "LT")
                return TradingStatus.LT;
            else if (Status == "KJ")
                return TradingStatus.KJ;
            else
                return TradingStatus.ER;

        }
        public bool ParseQuotationData(String Message, ref QuotationData Quotation)
        {
            String[] SubMessage = Message.Split(new char[] { ',' });

            if (SubMessage.Length != 66 || SubMessage[8] != "PZ")
            {
                return false;
            }

            Quotation.TradingTime = SubMessage[2] + " " + SubMessage[1];
            Quotation.LastClose = (float)Convert.ToDouble(SubMessage[3]);
            Quotation.OpenPrice = (float)Convert.ToDouble(SubMessage[4]);
            Quotation.HighPrice = (float)Convert.ToDouble(SubMessage[5]);
            Quotation.LowPrice = (float)Convert.ToDouble(SubMessage[6]);
            Quotation.NowPrice = (float)Convert.ToDouble(SubMessage[7]);
            Quotation.Status = ConvertToTradingStatus(SubMessage[8]);
            Quotation.TransactionCount = Convert.ToInt32(SubMessage[9]);
            Quotation.TotalVolume = Convert.ToInt32(SubMessage[10]) / 100;
            Quotation.TotalAmount = (float)Convert.ToDouble(SubMessage[11]);
            Quotation.CurBidAmount = (float)Convert.ToDouble(SubMessage[12]);
            Quotation.AverBidPrice = (float)Convert.ToDouble(SubMessage[13]);
            Quotation.CurAskAmount = (float)Convert.ToDouble(SubMessage[14]);
            Quotation.AverAskPrice = (float)Convert.ToDouble(SubMessage[15]);

            for (int i = 0, n = 26; i < 10; ++i, ++n)
            {
                Quotation.BidPrice[i] = (float)Convert.ToDouble(SubMessage[n]);
                Quotation.BidVolume[i] = Convert.ToInt32(SubMessage[n + 10]) / 100;
                Quotation.AskPrice[i] = (float)Convert.ToDouble(SubMessage[n + 20]);
                Quotation.AskVolume[i] = Convert.ToInt32(SubMessage[n + 30]) / 100;
            }

            return true;
        }
    }
}
