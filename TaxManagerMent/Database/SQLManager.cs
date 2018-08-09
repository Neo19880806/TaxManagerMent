using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TaxManagerMent.Model;
using TaxManagerMent.Models;
using Windows.Storage;

namespace SellerManagerment.Database

{
    class SQLManager
    {
        //private static string DB_LOCALPATH = "C:\\";
        private static string DB_NAME = "TaxManager.db";
        private static string DB_TABLE_SETTING = "TaxSetting";
        private static string DB_TABLE_DETAIL = "TaxDetail";
        
        //单例模式使用SQLManager;
        private static SQLManager _Instance = null;
        public static SQLManager DefalutInstance { get {return getInstance(); }}
        public static SQLManager getInstance()
        {
            if (null== _Instance)
            {
                _Instance = new SQLManager();
            }
            return _Instance;
        }
        //首次使用本软件，创建数据表
        public bool createTables(string tablePath)
        {
            string localPath = ApplicationData.Current.LocalFolder.Path;
            string dbfullpath = Path.Combine(localPath, DB_NAME);

            ISQLitePlatform platform = new SQLitePlatformWinRT();
            // 连接对象
            SQLiteConnection conn = new SQLiteConnection(platform, dbfullpath);

            // 创建表
            int rnSetting = conn.CreateTable<TaxSetting>(CreateFlags.None);
            int rnDetail = conn.CreateTable<TaxDetail>(CreateFlags.None);

            conn.Dispose();
            return rnSetting == 0 && rnDetail==0;
        }

        public bool checkTableExist()
        {
            string localPath = ApplicationData.Current.LocalFolder.Path;
            string dbfullpath = Path.Combine(localPath, DB_NAME);

            bool result = File.Exists(dbfullpath);
            return result;
        }

        //插入一条数据,并返回状态
        public bool Insert(Object obj)
        {
            string localPath = ApplicationData.Current.LocalFolder.Path;
            //string localPath = DB_LOCALPATH;
            string dbFullPath = Path.Combine(localPath, DB_NAME);
            SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), dbFullPath);
            // 插入数据
            int n = conn.InsertOrIgnore(obj);
            bool result = (n !=0);
            return result;
        }


        public bool Update(Object obj)
        {
            string localPath = ApplicationData.Current.LocalFolder.Path;
            string dbFullPath = Path.Combine(localPath, DB_NAME);
            SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), dbFullPath);

            int n = conn.InsertOrReplace(obj);
            bool result = (n != 0);
            return result;
        }

        //查询全表数据
        public List<TaxSetting> Query()
        {
            string localPath = ApplicationData.Current.LocalFolder.Path;
            string dbFullpath = Path.Combine(localPath, DB_NAME);
            SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), dbFullpath);

            conn.CreateCommand("select * from {0}", DB_TABLE_SETTING);
            // 获取列表
            TableQuery<TaxSetting> list = conn.Table<TaxSetting>();
            return list.ToList();
        }

        //查询数据
        public List<TaxDetail> Query(RequestInfo request)
        {
            string localPath = ApplicationData.Current.LocalFolder.Path;
            string dbFullpath = Path.Combine(localPath, DB_NAME);
            SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), dbFullpath);

            List<TaxDetail> list = null;
            if (request.Type == RequestInfo.TYPE_TYPE)
            {
                list = conn.Table<TaxDetail>().Where(v => v.Type == request.Name).
                    ToList();
            }
            else if (request.Type == RequestInfo.TYPE_DATE)
            {
                string strQuery = String.Format("select * from {0} where Time between '{1}' and '{2}'"
                    , DB_TABLE_DETAIL, request.StartDate.ToString("yyyy-MM-dd"),
                    request.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
                // 获取列表
                list = conn.Query<TaxDetail>(strQuery);
            }
            else if(request.Type==RequestInfo.TYPE_BOTH)
            {
                string strQuery = String.Format("select * from {0} where Time between '{1}' and '{2}'"
                    , DB_TABLE_DETAIL, request.StartDate.ToString("yyyy-MM-dd"),
                    request.EndDate.ToString("yyyy-MM-dd HH:mm:ss"));
                // 获取列表
                list = conn.Query<TaxDetail>(strQuery).Where(v=>v.Type ==request.Name).ToList();
            }


            return list;
        }


        public bool Delete(Object obj)
        {
            string localPath = ApplicationData.Current.LocalFolder.Path;
            string dbFullpath = Path.Combine(localPath, DB_NAME);
            SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), dbFullpath);

            int n = conn.Delete(obj);
            return n != 0;
        }
    }
}
