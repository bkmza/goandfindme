using System;
using Cirrious.CrossCore;
using GoHunting.Core.Services;
using Cirrious.CrossCore.IoC;

namespace GoHunting.Core
{
   public static class AppSettings
   {
      static AppSettings ()
      {
      }

      public static void RegisterTypes()
      {
         MvxSimpleIoCContainer.Initialize ();
         Mvx.RegisterType<ILoginService, LoginService> ();
         Mvx.RegisterType<IStopWatchWrapper, StopWatchWrapper> ();
         Mvx.RegisterType<IApiService, ApiService> ();
      }
   }
}

