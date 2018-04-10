using System;
using Android.Locations;
using Android.Views;
using GO.Core.Enums;
using GO.Core.Services;
using MvvmCross.Platform;

namespace GO.Common.Droid.Fragments
{
   public class CMapFragmentBase : FragmentBase
   {
      protected View MapView;

      protected IMenu MapMenu;
      protected MenuInflater MapMenuInflater;

      protected LayoutInflater MapLayoutInflater;

      protected IApiService ApiService;
      protected IToastService ToastService;
      protected IDBService DbService;
      protected IUserActionService UserActionService;
      protected IMapSettingsService MapSettingsService;
      protected IAppSettingsService AppSettingsService;

      protected Location MapCurrentLocation;

      protected double DistanceToNearestPoint;
      protected string NameOfNearestPoint;
      protected double UpdateFrequency;
      protected MapType MapType;
      protected DateTime LastUpdated;
      protected MapItemType? MapItemFilterType;

      public CMapFragmentBase()
      {
         DbService = Mvx.Resolve<IDBService>();
         ApiService = Mvx.Resolve<IApiService>();
         ToastService = Mvx.Resolve<IToastService>();
         UserActionService = Mvx.Resolve<IUserActionService>();
         MapSettingsService = Mvx.Resolve<IMapSettingsService>();
         AppSettingsService = Mvx.Resolve<IAppSettingsService>();
      }

      public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
      {
         MapMenu = menu;
         MapMenuInflater = inflater;

         MapMenu.Add(0, 0, 0, Resource.String.ConquerMenuTitle);
         MapMenu.Add(0, 1, 1, Resource.String.QuestMenuTitle);
         MapMenu.Add(0, 2, 2, Resource.String.TrapMenuTitle);
         MapMenu.Add(0, 3, 3, Resource.String.PlaceMenuTitle);
         MapMenu.Add(0, 4, 4, Resource.String.RazeMenuTitle);
         MapMenu.Add(0, 5, 5, Resource.String.AttackMenuTitle);
         MapMenu.Add(0, 6, 6, Resource.String.RefreshMenuTitle);
         MapMenu.Add(0, 7, 7, Resource.String.OnlyPointsMenuTitle);
         MapMenu.Add(0, 8, 8, Resource.String.OnlyQuestsMenuTitle);
         MapMenu.Add(0, 9, 9, Resource.String.AllObjectsMenuTitle);
         MapMenu.Add(0, 10, 10, Resource.String.LogoutMenuTitle);
      }
   }
}