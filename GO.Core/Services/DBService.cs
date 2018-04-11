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
      public DBService()
      {
         CreateDBSettings();

#if DEBUG
         CreateTestData();
#endif
      }

      private void CreateDBSettings()
      {
         var realmInstance = Realm.GetInstance();
         var initialMapSettings = new DBMapSettings
         {
            Id = Guid.NewGuid().ToString(),
            UpdateFrequency = 5
         };
         var initialAppSettings = new DBAppSettings
         {
            Id = Guid.NewGuid().ToString()
         };
         realmInstance.Write(() =>
         {
            realmInstance.Add(initialMapSettings);
            realmInstance.Add(initialAppSettings);
         });
      }

      private void CreateTestData()
      {
         var predefinedActions = new[] {
            new DBUserAction { Type = (int)ActionType.Point, Title = "Conquer0", Description = "Description0", Date = DateTime.Now },
            new DBUserAction { Type = (int)ActionType.Point, Title = "Conquer1", Description = "Description1", Date = DateTime.Now.AddMinutes(1) },
            new DBUserAction { Type = (int)ActionType.Point, Title = "Conquer2", Description = "Description2", Date = DateTime.Now.AddDays(1) },

            new DBUserAction { Type = (int)ActionType.Quest, Title = "Quest0", Description = "Description0", Date = DateTime.Now },
            new DBUserAction { Type = (int)ActionType.Quest, Title = "Quest1", Description = "Description1", Date = DateTime.Now.AddMinutes(1) },
            new DBUserAction { Type = (int)ActionType.Quest, Title = "Quest2", Description = "Description2", Date = DateTime.Now.AddDays(1) },
            new DBUserAction { Type = (int)ActionType.Quest, Title = "Quest3", Description = "Description3 with URL https://docs.google.com/spreadsheets/d/11FY9vt-7hJ4R15azA97droPWXSMHA5l6hG24y6JgLFI/edit#gid=0 and some additional notes", Date = DateTime.Now.AddDays(2) },

            new DBUserAction { Type = (int)ActionType.Trap, Title = "Trap0", Description = "Trap0_Description", Date = DateTime.Now },
            new DBUserAction { Type = (int)ActionType.Place, Title = "Place0", Description = "Place0_Description", Date = DateTime.Now.AddMinutes(1) },
            new DBUserAction { Type = (int)ActionType.Raze, Title = "Raze0", Description = "Raze0_Description", Date = DateTime.Now.AddDays(1) },
            new DBUserAction { Type = (int)ActionType.Attack, Title = "Attack0", Description = "Attack0_Description with URL https://docs.google.com/spreadsheets/d/11FY9vt-7hJ4R15azA97droPWXSMHA5l6hG24y6JgLFI/edit#gid=0 and some additional notes", Date = DateTime.Now.AddDays(3) },
         };

         var realmInstance = Realm.GetInstance();
         realmInstance.Write(() =>
         {
            foreach (var item in predefinedActions)
            {
               realmInstance.Add(item);
            }
         });
      }

      public void Add<T>(T data) where T : RealmObject
      {
         var realmInstance = Realm.GetInstance();
         realmInstance.Add(data, false);
      }

      public void Add<T>(List<T> data) where T : RealmObject
      {
         var realmInstance = Realm.GetInstance();
         realmInstance.Write(() =>
         {
            foreach (var item in data)
            {
               realmInstance.Add(item);
            }
         });
      }

      public void Delete<T>(T data) where T : RealmObject
      {
         var realmInstance = Realm.GetInstance();
         realmInstance.Remove(data);
      }

      public void DeleteAll<T>() where T : RealmObject
      {
         var realmInstance = Realm.GetInstance();
         realmInstance.RemoveAll<T>();
      }

      public List<T> Get<T>() where T : RealmObject
      {
         var realmInstance = Realm.GetInstance();
         return realmInstance.All<T>().ToList();
      }

      public void Act(Action action)
      {
         var realmInstance = Realm.GetInstance();
         if (realmInstance.IsInTransaction)
         {
            action();
         }
         else
         {
            realmInstance.Write(action);
         }
      }
   }
}