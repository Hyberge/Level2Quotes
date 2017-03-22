﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows;

namespace Level2Quotes
{
    class FileWriter
    {
        public static String sDataRootPath = "../../HTD/";
        public static String sTransactionDataPath = sDataRootPath + "TransactionData/";
        public static String sQuotationDataPath = sDataRootPath + "QuotationData/";

        public static bool IsTransactionDataExists(String Symbol, DateTime Day)
        {
            String FileName = sTransactionDataPath + Symbol + "/" + Day.ToString("yyyy-MM-dd");

            return File.Exists(FileName);
        }

        public static bool WriteTransactionDataToFile(String Symbol, List<TransactionData> Data, bool Rewriting)
        {
            bool bRet = false;

            String FilePath = sTransactionDataPath + Symbol;
            if (Directory.Exists(FilePath) == false)//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(FilePath);
            }

            FileStream file = null;
            String FileName = FilePath + "/" + DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(FileName) == false)
            {
                file = File.Create(FileName);
            }
            else
            {
                file = new FileStream(FileName, FileMode.Open);
            }

            if (file.CanWrite)
            {
                if (Rewriting)
                {
                	file.SetLength(0);
                }
                else
                {
                    file.Seek(0, SeekOrigin.End);
                }
                StreamWriter writer = new StreamWriter(file);

                for (int i = Data.Count - 1; i >= 0; i--)
                {
                    var ele = Data[i];
                    String str = ele.Index + "|";
                    str += ele.Price + "|";
                    str += ele.Volume + "|";
                    str += ele.Count + "|";
                    str += ele.BuyNumber + "|";
                    str += ele.SellNumber + "|";
                    str += ele.IOType + "|";
                    str += ele.Channel + "|";
                    str += ele.TradingTime;

                    writer.WriteLine(str);
                }

                writer.Flush();
            }

            file.Close();

            return bRet;
        }

        public static void ReadTransactionDataFromFile(String Symbol, DateTime Day, List<TransactionData> Data)
        {
            Data.Clear();

            String FileName = sTransactionDataPath + Symbol + "/" + Day.ToString("yyyy-MM-dd");
            if (File.Exists(FileName) == false)
            {
                MessageBox.Show("路径错误!!!");
                return;
            }

            FileStream file = new FileStream(FileName, FileMode.Open);

            if (file.CanRead)
            {
                StreamReader reader = new StreamReader(file);
                String str;

                while ((str = reader.ReadLine()) != null)
                {
                    string[] separator = new string[] { "|" };
                    var strElement = str.Split(new char[]{'|'}, StringSplitOptions.RemoveEmptyEntries);

                    if (strElement.Count() == 9)
                    {
                        TransactionData ele = new TransactionData();
                        ele.Index = Convert.ToInt32(strElement[0]);
                        ele.Price = (float)Convert.ToDouble(strElement[1]);
                        ele.Volume = Convert.ToInt32(strElement[2]);
                        ele.Count = (float)Convert.ToDouble(strElement[3]);
                        ele.BuyNumber = Convert.ToInt32(strElement[4]);
                        ele.SellNumber = Convert.ToInt32(strElement[5]);
                        ele.IOType = Convert.ToInt32(strElement[6]);
                        ele.Channel = Convert.ToInt32(strElement[7]);
                        ele.TradingTime = Util.TradingTimeStringToInt(strElement[8]);

                        Data.Add(ele);
                    }
                }
            }

            file.Close();
        }

        public static bool WriteQuotationDataToFile(String Symbol, QuotationData Data, bool Rewriting)
        {
            bool bRet = false;

            String FilePath = sQuotationDataPath + Symbol;
            if (Directory.Exists(FilePath) == false)//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(FilePath);
            }

            FileStream file = null;
            String FileName = FilePath + "/" + DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists(FileName) == false)
            {
                file = File.Create(FileName);
            }
            else
            {
                file = new FileStream(FileName, FileMode.Open);
            }

            if (file.CanWrite)
            {
                if (Rewriting)
                {
                    file.SetLength(0);
                }
                else
                {
                    file.Seek(0, SeekOrigin.End);
                }
                StreamWriter writer = new StreamWriter(file);

                String str = Data.TradingTime + "|";
                str += Data.LastClose + "|";
                str += Data.OpenPrice + "|";
                str += Data.CurBidAmount + "|";
                str += Data.AverBidPrice + "|";
                str += Data.CurAskAmount + "|";
                str += Data.AverAskPrice + "|";
                str += Data.CancelBidNum + "|";
                str += Data.CancelBidAmount + "|";
                str += Data.CancelAskNum + "|";
                str += Data.CancelAskAmount + "|";
                str += Data.TotalBidNum + "|";
                str += Data.TotalAskNum;

                writer.WriteLine(str);

                writer.Flush();
            }

            file.Close();

            return bRet;
        }

        public static void ReadQuotationDataFromFile(String Symbol, DateTime Day, List<QuotationData> Data)
        {
            String FileName = sQuotationDataPath + Symbol + "/" + Day.ToString("yyyy-MM-dd");
            if (File.Exists(FileName) == false)
            {
                MessageBox.Show("路径错误!!!");
                return;
            }

            FileStream file = new FileStream(FileName, FileMode.Open);

            if (file.CanRead)
            {
                StreamReader reader = new StreamReader(file);
                String str;

                while ((str = reader.ReadLine()) != null)
                {
                    string[] separator = new string[] { "|" };
                    var strElement = str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    if (strElement.Count() == 13)
                    {
                        QuotationData ele = new QuotationData();
                        ele.TradingTime = strElement[0];
                        ele.LastClose = (float)Convert.ToDouble(strElement[1]);
                        ele.OpenPrice = (float)Convert.ToDouble(strElement[2]);
                        ele.CurBidAmount = (float)Convert.ToDouble(strElement[3]);
                        ele.AverBidPrice = (float)Convert.ToDouble(strElement[4]);
                        ele.CurAskAmount = (float)Convert.ToDouble(strElement[5]);
                        ele.AverAskPrice = (float)Convert.ToDouble(strElement[6]);
                        ele.CancelBidNum = Convert.ToInt32(strElement[7]);
                        ele.CancelBidAmount = (float)Convert.ToDouble(strElement[8]);
                        ele.CancelAskNum = Convert.ToInt32(strElement[9]);
                        ele.CancelAskAmount = (float)Convert.ToDouble(strElement[10]);
                        ele.TotalBidNum = Convert.ToInt32(strElement[11]);
                        ele.TotalAskNum = Convert.ToInt32(strElement[12]);

                        Data.Add(ele);
                    }
                }
            }

            file.Close();
        }
    }
}
