using System.Collections.Generic;
using System.Threading.Tasks;
using GO.Core.Data;
using GO.Core.Entities;
using GO.Core.Enums;

namespace GO.Core.Services
{
   public interface IUserActionService
   {
      List<UserAction> GetActions(ActionType? type  = null);

      void Add(UserAction userAction);

      void DeleteAll();

      Task<ActionResponseBase> MakeAction(ActionType type, string deviceId, string lat, string lon, string objectCode = null);
   }
}