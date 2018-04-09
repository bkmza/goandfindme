using System;
using System.Collections.Generic;
using Realms;

namespace GO.Core.Services
{
   public interface IDBService
   {
      void Add<T>(T data) where T : RealmObject;

      void Add<T>(List<T> data) where T : RealmObject;

      void Delete<T>(T data) where T : RealmObject;

      void DeleteAll<T>() where T : RealmObject;

      List<T> Get<T>() where T : RealmObject;

      void Act(Action action);
   }
}