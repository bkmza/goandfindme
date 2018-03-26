namespace GO.Core.Services
{
   public interface IAppSettingsService
   {
      void SetAppId(string value);

      string GetAppId();
   }
}