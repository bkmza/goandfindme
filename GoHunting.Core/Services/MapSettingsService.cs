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

      public int GetMapType()
      {
         var dbMapSettings = _dbService.Get<DBMapSettings> (1);
         return dbMapSettings.MapType;
      }

      public void SetMapType(int mapType)
      {
         var dbMapSettings = _dbService.Get<DBMapSettings> (1);
         dbMapSettings.MapType = mapType;
         _dbService.Update (dbMapSettings);
      }
   }
}

