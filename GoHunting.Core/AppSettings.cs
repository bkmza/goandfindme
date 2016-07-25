using System;
using AutoMapper;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using GoHunting.Core.Entities;
using GoHunting.Core.Services;

namespace GoHunting.Core
{
   public static class AppSettings
   {
      static AppSettings ()
      {
         // GoHunting // packageName: com.go.goandfindme
         //BaseHost = "http://gohunting.greyorder.su/";
         //ApplicationName = @"GOhunting";
         //PackageName = "com.go.goandfindme";

         // Paranoia // packageName: com.go.paranoia
         BaseHost = "http://goandpay.greyorder.su/";
         ApplicationName = @"Paranoia";
         PackageName = "com.go.paranoia";
      }

      public static string TrackingId { get; set; }

      public static string BaseHost { get; set; }

      public static string ApplicationName { get; set; }

      public static string PackageName { get; set; }

      public static void RegisterTypes ()
      {
         MvxSimpleIoCContainer.Initialize ();
         Mvx.RegisterType<ILoginService, LoginService> ();
         Mvx.RegisterType<IStopWatchWrapper, StopWatchWrapper> ();
         Mvx.RegisterType<IApiService, ApiService> ();
      }

      public static void RegisterMapper ()
      {
         Mapper.CreateMap<UserAction, DBUserAction> ();
         Mapper.CreateMap<DBUserAction, UserAction> ();
         Mapper.AssertConfigurationIsValid ();
      }
   }
}

