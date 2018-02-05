using System.Collections.Generic;
using GO.Core.Entities;

namespace GO.Core.Services
{
   public interface IDBService
   {
      void Add<T>(T data);

      void Add<T>(List<T> data, bool isRunInTransaction = true);

      void Update<T>(T data);

      void Delete<T>(T data);

      void DeleteAll<T>();

      T Get<T>(long id) where T : DBEntityBase;

      List<T> Get<T>() where T : class, new();
   }
}