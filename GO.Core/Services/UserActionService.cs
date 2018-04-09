using System.Collections.Generic;
using System.Linq;
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

         List<UserAction> items = dbItems.Select(item => new UserAction(item)).ToList();

         return items;
      }

      public List<UserAction> GetQuests()
      {
         var dbItems = _dbService.Get<DBUserAction>().Where(x => x.Type == (int)MapItemType.Quest).ToList();

         List<UserAction> items = dbItems.Select(item => new UserAction(item)).ToList();

         return items;
      }

      public List<UserAction> GetAllTypes()
      {
         var dbItems = _dbService.Get<DBUserAction>().ToList();

         List<UserAction> items = dbItems.Select(item => new UserAction(item)).ToList();

         return items;
      }

      public void Add(UserAction userAction)
      {
         _dbService.Act(action);

         void action()
         {
            var dbUserAction = new DBUserAction
            {
               Type = userAction.Type,
               Title = userAction.Title,
               Description = userAction.Description,
               Date = userAction.Date,
               Number = userAction.Number
            };
            _dbService.Add(dbUserAction);
         }
      }

      public void DeleteAll()
      {
         _dbService.Act(action);

         void action()
         {
            _dbService.DeleteAll<DBUserAction>();
         }
      }
   }
}