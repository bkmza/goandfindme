using System.Collections.Generic;
using GoHunting.Core.Entities;

namespace GoHunting.Core.Services
{
   public interface IUserActionService
   {
      List<UserAction> GetConquers();

      List<UserAction> GetQuests();

      List<UserAction> GetAllTypes();

      void Add(UserAction userAction);
   }
}