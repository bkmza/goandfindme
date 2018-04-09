using System;
using System.Collections.Generic;
using System.Linq;
using GO.Core.Entities;
using GO.Core.Enums;
using Realms;

namespace GO.Core.Services
{
   public class DBService : IDBService
   {
      private Realm _realm;

      public DBService()
      {
         _realm = Realm.GetInstance();

         CreateDB();
         CreateDBSettings();

#if DEBUG
         CreateTestData();
#endif
      }

      private void CreateDB()
      {
         //Connection.BeginTransaction();
         //Connection.CreateTable<DBMapSettings>();
         //Connection.CreateTable<DBPoint>();
         //Connection.CreateTable<DBUserAction>();
         //Connection.CreateTable<DBAppSettings>();
         //Connection.Commit();
      }

      private void CreateDBSettings()
      {
         var initialMapSettings = new DBMapSettings
         {
            UpdateFrequency = 5
         };
         _realm.Add(initialMapSettings);

         var initialAppSettings = new DBAppSettings();
         _realm.Add(initialAppSettings);
      }

      private void CreateTestData()
      {
         var predefinedActions = new[] {
            new DBUserAction { Type = (int)MapItemType.Point, Title = "Conquer0", Description = "Description0", Date = DateTime.Now },
            new DBUserAction { Type = (int)MapItemType.Point, Title = "Conquer1", Description = "Description1", Date = DateTime.Now.AddMinutes(1) },
            new DBUserAction { Type = (int)MapItemType.Point, Title = "Conquer2", Description = "Description2", Date = DateTime.Now.AddDays(1) },

            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest0", Description = "Description0", Date = DateTime.Now },
            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest1", Description = "Description1", Date = DateTime.Now.AddMinutes(1) },
            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest2", Description = "Description2", Date = DateTime.Now.AddDays(1) },
            new DBUserAction { Type = (int)MapItemType.Quest, Title = "Quest3", Description = "Description3 with URL https://docs.google.com/spreadsheets/d/11FY9vt-7hJ4R15azA97droPWXSMHA5l6hG24y6JgLFI/edit#gid=0 and some additional notes", Date = DateTime.Now.AddDays(2) },
         };
         foreach (var item in predefinedActions)
         {
            _realm.Add(item);
         }
      }

      public void Add<T>(T data) where T : DBEntityBase
      {
         _realm.Add(data, false);
      }

      public void Add<T>(List<T> data) where T : DBEntityBase
      {
         _realm.Write(() =>
         {
            foreach (var item in data)
            {
               _realm.Add(item);
            }
         });
      }

      public void Delete<T>(T data) where T : DBEntityBase
      {
         _realm.Remove(data);
      }

      public void DeleteAll<T>() where T : DBEntityBase
      {
         _realm.RemoveAll<T>();
      }

      public T Get<T>(string id) where T : DBEntityBase
      {
         return _realm.All<T>().Where(x => x.Id == id).FirstOrDefault();
      }

      public List<T> Get<T>() where T : DBEntityBase
      {
         return _realm.All<T>().ToList();
      }
   }
}