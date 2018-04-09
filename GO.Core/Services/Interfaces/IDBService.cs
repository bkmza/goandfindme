using System.Collections.Generic;
using GO.Core.Entities;

namespace GO.Core.Services
{
   public interface IDBService
   {
      void Add<T>(T data) where T : DBEntityBase;

      void Add<T>(List<T> data) where T : DBEntityBase;

      void Delete<T>(T data) where T : DBEntityBase;

      void DeleteAll<T>() where T : DBEntityBase;

      T Get<T>(string id) where T : DBEntityBase;

      List<T> Get<T>() where T : DBEntityBase;
   }
}