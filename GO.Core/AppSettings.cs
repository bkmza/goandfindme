using GO.Core.Services;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;

namespace GO.Core
{
   public static class AppSettings
   {
      static AppSettings() { }

      public static string TrackingId { get; set; }

      public static string BaseHost { get; set; }

      public static string ApplicationName { get; set; }

      public static string PackageName { get; set; }

      public static void RegisterTypes()
      {
         MvxSimpleIoCContainer.Initialize();
         Mvx.RegisterType<ILoginService, LoginService>();
         Mvx.RegisterType<IStopWatchWrapper, StopWatchWrapper>();
         Mvx.RegisterType<IApiService, ApiService>();
      }
   }
}