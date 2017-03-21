using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Data.SQLite;

namespace Level2Quotes
{
    class SQLiteDataBase
    {
        SQLiteConnection mDBConnection;

        volatile static SQLiteDataBase sInstance = null;

        public static SQLiteDataBase Instance()
        {
            if (sInstance == null)
            {
                sInstance = new SQLiteDataBase();
                sInstance.ConnectToDataBase();
            }
            return sInstance; 
        }

        private SQLiteDataBase()
        { }

        public void ConnectToDataBase()
        {
            if (!File.Exists("../../HTD/DataBase.db"))
            {
                SQLiteConnection.CreateFile("../../HTD/DataBase.db");
            }

            SQLiteConnectionStringBuilder Builder = new SQLiteConnectionStringBuilder();
            Builder.DataSource = "../../HTD/DataBase.db";
            Builder.Password = "dvsv99zero";
            String Connect = Builder.ToString();
            mDBConnection = new SQLiteConnection(Connect);
            mDBConnection.Open();
        }

        public void CreateTable(String Symbol)
        {
            String Sql = String.Format(@"select count(*) as c from sqlite_master where type='table' and name='{0}'", Symbol);
            SQLiteCommand Command = new SQLiteCommand(Sql, mDBConnection);
            SQLiteDataReader Reader = Command.ExecuteReader();
            Reader.Read();

            Console.WriteLine(Reader["c"]);
            if (Convert.ToInt32(Reader["c"]) == 0)
            {
                Sql = String.Format("create table {0} (Day int primary key not null, BidCount float not null, PBidCount float not null, AskCount float not null, PAskCount float not null, Difference float not null)",
                                 Symbol);
                Command = new SQLiteCommand(Sql, mDBConnection);
                Command.ExecuteNonQuery();
            }
        }

        public void InsertDataToTable(String Symbol, object[] args)
        {
            String Sql = String.Format("insert into {0} (Day, BidCount, PBidCount, AskCount, PAskCount, Difference) values ({1}, {2}, {3}, {4}, {5}, {6})", 
                Symbol, args[1].ToString(), args[2].ToString(), args[3].ToString(), args[4].ToString(), args[5].ToString(), args[6].ToString());

            SQLiteCommand Command = new SQLiteCommand(Sql, mDBConnection);
            Command.ExecuteNonQuery();
        }
    }
}
