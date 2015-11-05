using System;
using System.Collections.Generic;
using GoHunting.Core.Entities;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;

namespace GoHunting.Core.Services
{
   public class DBService : IDBService
   {
      SQLiteConnection Connection;
      SQLiteConnectionString ConnectionString;
      ISQLitePlatform Platform;

      public DBService (ISQLite sqlite)
      {
         Connection = sqlite.GetConnection ();

         CreateDB ();
      }

      private void CreateDB()
      {
         List<SQLiteConnection.ColumnInfo> tableInfo = Connection.GetTableInfo ("DBPoint");

         Connection.BeginTransaction ();
         Connection.CreateTable<DBPoint> ();
         Connection.Commit ();
      }

      public void Add<T> (T data)
      {
         Connection.Insert (data);
      }

      public void Add<T> (List<T> data, bool isRunInTransaction = true)
      {
         Connection.InsertAll (data, isRunInTransaction);
      }

      public void Update<T> (T data)
      {
         Connection.Update (data);
      }

      public void Delete<T> (T data)
      {
         Connection.Delete (data);
      }

      public T Get<T> (long id) where T : DBEntityBase
      {
         return Connection.Get<T> (x => x.Id == id);
      }

      public List<T> Get<T> () where T : class, new()
      {
         return Connection.GetAllWithChildren<T> ();
      }
   }
}

