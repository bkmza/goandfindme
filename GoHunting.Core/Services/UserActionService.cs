using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GoHunting.Core.Entities;
using GoHunting.Core.Enums;

namespace GoHunting.Core.Services
{
   public class UserActionService : IUserActionService
   {
      IDBService _dbService;

      public UserActionService (IDBService dbService)
      {
         _dbService = dbService;
      }

      public List<UserAction> GetConquers ()
      {
         var dbItems = _dbService.Get<DBUserAction> ().Where (x => x.Type == (int)MapItemType.Point).ToList ();

         var items = Mapper.Map<List<UserAction>> (dbItems);

         return items;
      }

      public List<UserAction> GetQuests ()
      {
         var dbItems = _dbService.Get<DBUserAction> ().Where (x => x.Type == (int)MapItemType.Quest).ToList ();

         var items = Mapper.Map<List<UserAction>> (dbItems);

         return items;
      }

      public void Add (UserAction userAction)
      {
         var dbUserAction = Mapper.Map<DBUserAction> (userAction);

         _dbService.Add<DBUserAction> (dbUserAction);
      }
   }
}

