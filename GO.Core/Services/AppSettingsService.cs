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
         var dbAppSettings = _dbService.Get<DBAppSettings>(1);
         dbAppSettings.AppId = value;
         _dbService.Update(dbAppSettings);
      }

      public string GetAppId()
      {
         var dbAppSettings = _dbService.Get<DBAppSettings>(1);
         return dbAppSettings.AppId;
      }
   }
}