using System.Linq;
using GO.Core.Entities;

namespace GO.Core.Services
{
   public class AppSettingsService : IAppSettingsService
   {
      private readonly IDBService _dbService;

      public AppSettingsService(IDBService dbService)
      {
         _dbService = dbService;
      }

      public void SetAppId(string value)
      {
         var dbAppSettings = _dbService.Get<DBAppSettings>().First();
         dbAppSettings.AppId = value;
         _dbService.Add(dbAppSettings);
      }

      public string GetAppId()
      {
         var dbAppSettings = _dbService.Get<DBAppSettings>().First();
         return dbAppSettings.AppId;
      }
   }
}