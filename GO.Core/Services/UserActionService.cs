using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.Core.Data;
using GO.Core.Entities;
using GO.Core.Enums;

namespace GO.Core.Services
{
   public class UserActionService : IUserActionService
   {
      private readonly IDBService _dbService;
      private readonly IApiService _apiService;

      public UserActionService(IDBService dbService, IApiService apiService)
      {
         _dbService = dbService;
         _apiService = apiService;
      }

      public List<UserAction> GetActions(ActionType? type = null)
      {
         var dbItems = _dbService.Get<DBUserAction>();

         if (type != null)
         {
            dbItems = dbItems.Where(x => x.Type == (int)type).ToList();
         }

         var items = dbItems.Select(item => new UserAction(item)).ToList();

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

      public async Task<ActionResponseBase> MakeAction(ActionType type, string deviceId, string lat, string lon, string objectCode = null)
      {
         ActionResponseBase result = null;
         switch (type)
         {
            case ActionType.Point:
               result = await _apiService.Conquer(deviceId, lat, lon);
               break;
            case ActionType.Quest:
               result = await _apiService.Quest(deviceId, lat, lon);
               break;
            case ActionType.Trap:
               result = await _apiService.Trap(deviceId, lat, lon);
               break;
            case ActionType.Place:
               result = await _apiService.Place(deviceId, lat, lon);
               break;
            case ActionType.Raze:
               result = await _apiService.Raze(deviceId, lat, lon);
               break;
            case ActionType.Attack:
               result = await _apiService.Attack(deviceId, lat, lon);
               break;
            case ActionType.Use:
               result = await _apiService.Use(deviceId, lat, lon, objectCode);
               break;
            default:
               throw new NotImplementedException($"MakeAction doesn't support such kind of actions.");
         }
         return result;
      }
   }
}