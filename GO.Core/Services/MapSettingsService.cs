using System.Linq;
using GO.Core.Entities;

namespace GO.Core.Services
{
   public class MapSettingsService : IMapSettingsService
   {
      private readonly IDBService _dbService;

      public MapSettingsService(IDBService dbService)
      {
         _dbService = dbService;
      }

      public int GetUpdateFrequency()
      {
         var dbMapSettings = _dbService.Get<DBMapSettings>().First();
         return dbMapSettings.UpdateFrequency;
      }

      public void SetUpdateFrequency(int value)
      {
         var dbMapSettings = _dbService.Get<DBMapSettings>().First();
         dbMapSettings.UpdateFrequency = value;
         _dbService.Add(dbMapSettings);
      }

      public int GetMapType()
      {
         var dbMapSettings = _dbService.Get<DBMapSettings>().First();
         return dbMapSettings.MapType;
      }

      public void SetMapType(int mapType)
      {
         var dbMapSettings = _dbService.Get<DBMapSettings>().First();
         dbMapSettings.MapType = mapType;
         _dbService.Add(dbMapSettings);
      }
   }
}