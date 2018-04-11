using System.Linq;
using GO.Core.Entities;

namespace GO.Core.Services
{
   public class AppSettingsService : IAppSettingsService
   {
      private readonly IDBService _dBService;

      public AppSettingsService(IDBService dBService)
      {
         _dBService = dBService;
      }

      public void SetAppId(string value)
      {
         _dBService.Act(action);

         void action()
         {
            var item = _dBService.Get<DBAppSettings>().First();
            item.AppId = value;
         }
      }

      public string GetAppId()
      {
         var item = _dBService.Get<DBAppSettings>().First();
         return item.AppId;
      }
   }
}