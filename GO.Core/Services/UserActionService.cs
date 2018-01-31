using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GO.Core.Entities;
using GO.Core.Enums;

namespace GO.Core.Services
{
   public class UserActionService : IUserActionService
   {
      IDBService _dbService;

      public UserActionService(IDBService dbService)
      {
         _dbService = dbService;
      }

      public List<UserAction> GetConquers()
      {
         var dbItems = _dbService.Get<DBUserAction>().Where(x => x.Type == (int)MapItemType.Point).ToList();

         var items = AppSettings.MapperInstance.Map<List<UserAction>>(dbItems);

         return items;
      }

      public List<UserAction> GetQuests()
      {
         var dbItems = _dbService.Get<DBUserAction>().Where(x => x.Type == (int)MapItemType.Quest).ToList();

         var items = AppSettings.MapperInstance.Map<List<UserAction>>(dbItems);

         return items;
      }

      public List<UserAction> GetAllTypes()
      {
         var dbItems = _dbService.Get<DBUserAction>().ToList();

         var items = AppSettings.MapperInstance.Map<List<UserAction>>(dbItems);

         return items;
      }

      public void Add(UserAction userAction)
      {
         var dbUserAction = AppSettings.MapperInstance.Map<DBUserAction>(userAction);

         _dbService.Add(dbUserAction);
      }
   }
}