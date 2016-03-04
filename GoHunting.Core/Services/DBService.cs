﻿using System;
using System.Collections.Generic;
using GoHunting.Core.Entities;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;
using GoHunting.Core.Enums;

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
//         CreateTestData ();
      }

      private void CreateDB ()
      {
         Connection.BeginTransaction ();
         Connection.CreateTable<DBPoint> ();
         Connection.CreateTable<DBUserAction> ();
         Connection.Commit ();
      }

      private void CreateTestData()
      {
         var predefinedActions = new[] {
            new DBUserAction { Type = (int)MapItemType.Point, Title = "Conquer0", Description = "Description0", Date = DateTime.Now },
            new DBUserAction { Type = (int)MapItemType.Point, Title = "Conquer1", Description = "Description1", Date = DateTime.Now },
            new DBUserAction { Type = (int)MapItemType.Point, Title = "Conquer2", Description = "Description2", Date = DateTime.Now },

            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest0", Description = "Description0", Date = DateTime.Now },
            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest1", Description = "Description1", Date = DateTime.Now },
            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest2", Description = "Description2", Date = DateTime.Now },
            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest3", Description = "Description3", Date = DateTime.Now },
         };
         foreach (var item in predefinedActions) {
            Add (item);
         }
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
