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
         _dbService.Act(action);

         void action()
         {
            var item = _dbService.Get<DBMapSettings>().First();
            item.UpdateFrequency = value;
            _dbService.Add(item);
         }
      }

      public int GetMapType()
      {
         var dbMapSettings = _dbService.Get<DBMapSettings>().First();
         return dbMapSettings.MapType;
      }

      public void SetMapType(int mapType)
      {
         _dbService.Act(action);

         void action()
         {
            var item = _dbService.Get<DBMapSettings>().First();
            item.MapType = mapType;
            _dbService.Add(item);
         }
      }
   }
}