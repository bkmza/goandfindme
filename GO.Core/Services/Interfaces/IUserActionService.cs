using System.Collections.Generic;
using GO.Core.Entities;

namespace GO.Core.Services
{
   public interface IUserActionService
   {
      List<UserAction> GetConquers();

      List<UserAction> GetQuests();

      List<UserAction> GetAllTypes();

      void Add(UserAction userAction);
   }
}