using System;
using GoHunting.Core.Entities;

namespace GoHunting.Core.Services
{
   public class MapSettingsService : IMapSettingsService
   {
      private readonly IDBService _dbService;

      public MapSettingsService (IDBService dbService)
      {
         _dbService = dbService;
      }

      public int GetUpdateFrequency ()
      {
         var dbMapSettings = _dbService.Get<DBMapSettings> (1);
         return dbMapSettings.UpdateFrequency;
      }

      public void SetUpdateFrequency (int value)
      {
         var dbMapSettings = _dbService.Get<DBMapSettings> (1);
         dbMapSettings.UpdateFrequency = value;
         _dbService.Update (dbMapSettings);
      }
   }
}

