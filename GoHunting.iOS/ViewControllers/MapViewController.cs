﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using UIKit;
using CoreGraphics;
using CoreLocation;

using Google.Maps;
using Cirrious.CrossCore;

using Newtonsoft.Json;

using GoHunting.Core.Services;
using GoHunting.Core;
using GoHunting.Core.Enums;
using GoHunting.Core.Helpers;
using GoHunting.iOS.Utilities;
using GoHunting.Core.Data;
using GoHunting.iOS.Views;
using GoHunting.Core.Utilities;
using Foundation;

namespace GoHunting.iOS.ViewControllers
{
   public class MapViewController : BaseViewController
   {
      private UIBarButtonItem _actionsButton;

      private MapView _mapView;
      private List<Marker> _markersList;
      private MapItemType? _currentMapItemType;

      private IApiService _apiService;

      private CLLocation _currentLocation;
      private double _distanceToNearestPoint;
      private string _nameOfNearestPoint;

      public static bool UserInterfaceIdiomIsPhone
      {
         get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
      }

      public static LocationManager Manager { get; set; }

      public MapViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.Favorites, 0);

         _apiService = Mvx.Resolve<IApiService>();
      }

      public override void ViewWillAppear(bool animated)
      {
         base.ViewWillAppear(animated);

         _actionsButton = new UIBarButtonItem(UIBarButtonSystemItem.Action, ShowMenu)
         {
            TintColor = UIColor.Black
         };
         NavigationItem.SetRightBarButtonItems(new[] { _actionsButton }, true);
         Manager.LocationUpdated += HandleLocationChanged;
      }

      public override void ViewDidDisappear(bool animated)
      {
         base.ViewDidDisappear(animated);

         if (Manager != null)
         {
            Manager.LocationUpdated -= HandleLocationChanged;
         }
      }

      public override void ViewDidLoad()
      {
         base.ViewDidLoad();
      }

      public override void Initialize()
      {
         base.Initialize();

         var camera = CameraPosition.FromCamera(latitude: 53.900819, longitude: 27.558823, zoom: 10);
         _mapView = MapView.FromCamera(CGRect.Empty, camera);
         _mapView.MyLocationEnabled = true;

         _mapView.TappedMarker = (MapView mapView, Marker marker) =>
         {
            try
            {
               Point item = JsonConvert.DeserializeObject<Point>(marker.Snippet);
               PointInfo info = Mvx.Resolve<IApiService>().GetInfo(DeviceUtility.DeviceId, item.id, item.type);
               marker.Title = JsonConvert.SerializeObject(info);
            }
            catch (Exception ex)
            {
               Logger.Instance.Debug(string.Format("mapView.TappedMarker exception: {0}", ex.Message));
            }
            _mapView.MarkerInfoWindow = new GMSInfoFor(MarkerInfoWindow);
            return false;
         };

         View = _mapView;

         _markersList = new List<Marker>();

         UpdateMap();
         Manager = new LocationManager();
         Manager.StartLocationUpdates();
      }

      private void UpdateMap()
      {
         IsLoading = true;
         _mapView.Clear();
         Task.Run(async () =>
         {
            await UpdateMapAsync();
            IsLoading = false;
         });
      }

      private async Task UpdateMapAsync()
      {
         var points = await _apiService.GetAll(DeviceUtility.DeviceId);

         if (_currentMapItemType != null)
         {
            points = points.Where(x => x.GetMapItemType == _currentMapItemType);
         }
         InvokeOnMainThread(() =>
         {
            foreach (Point point in points)
            {
               Marker marker = new Marker();

               marker.Title = point.title;
               marker.Position = new CLLocationCoordinate2D(point.GetLatitude, point.GetLongitude);
               marker.Icon = UIImage.FromBundle("Images/" + point.GetIconName);
               marker.Map = _mapView;
               marker.Snippet = JsonConvert.SerializeObject(point);

               _markersList.Add(marker);
            }
         });
      }

      private UIView MarkerInfoWindow(UIView view, Marker marker)
      {
         MarkerInfoWindowView v = new MarkerInfoWindowView(marker);
         return v;
      }

      public void ShowMenu(object sender, EventArgs e)
      {
         if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
         {
            var alert = UIAlertController.Create("Выберите действие", null, UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create("Захват", UIAlertActionStyle.Default, (UIAlertAction obj) =>
            {
               AnalyticsService.TrackState("Conquer", "Hit on Conquer button", string.Format("User {0} is tryuing to conquer point {1}", DeviceUtility.DeviceId, _nameOfNearestPoint));
               ConquerHandler();
            }));

            alert.AddAction(UIAlertAction.Create("Квест", UIAlertActionStyle.Default, (UIAlertAction obj) =>
            {
               AnalyticsService.TrackState("Conquer", "Hit on Quest button", string.Format("User {0} is tryuing to conquer point {1}", DeviceUtility.DeviceId, _nameOfNearestPoint));
               QuestHandler();
            }));

            alert.AddAction(UIAlertAction.Create("Обновить", UIAlertActionStyle.Default, (UIAlertAction obj) =>
            {
               AnalyticsService.TrackState("Refresh map", "Hit on Refresh button", string.Format("User {0} updated map manually", DeviceUtility.DeviceId));
               UpdateMap();
            }));

            alert.AddAction(UIAlertAction.Create("Только точки", UIAlertActionStyle.Default, (UIAlertAction obj) =>
            {
               AnalyticsService.TrackState("Set Point filter on map", "Hit on Point filter button", string.Format("User {0} updated filter to points only", DeviceUtility.DeviceId));
               _currentMapItemType = MapItemType.Point;
               UpdateMap();
            }));

            alert.AddAction(UIAlertAction.Create("Только квесты", UIAlertActionStyle.Default, (UIAlertAction obj) =>
            {
               AnalyticsService.TrackState("Set Quest filter on map", "Hit on Quest filter button", string.Format("User {0} updated filter to quests only", DeviceUtility.DeviceId));
               _currentMapItemType = MapItemType.Quest;
               UpdateMap();
            }));

            alert.AddAction(UIAlertAction.Create("Все объекты", UIAlertActionStyle.Default, (UIAlertAction obj) =>
            {
               AnalyticsService.TrackState("Set All items filter on map", "Hit on All items filter button", string.Format("User {0} updated filter to all items", DeviceUtility.DeviceId));
               _currentMapItemType = null;
               UpdateMap();
            }));

            alert.AddAction(UIAlertAction.Create("Закрыть", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
         }
         else 
         {
            UIActionSheet actionSheetAlert = new UIActionSheet("Выберите действие");

            actionSheetAlert.AddButton("Захват");
            actionSheetAlert.AddButton("Квест");
            actionSheetAlert.AddButton("Обновить");
            actionSheetAlert.AddButton("Только точки");
            actionSheetAlert.AddButton("Только квесты");
            actionSheetAlert.AddButton("Все объекты");
            actionSheetAlert.Clicked += delegate (object a, UIButtonEventArgs b)
            {
               switch (b.ButtonIndex)
               {
                  case 0:
                     ConquerHandler();
                     break;
                  case 1:
                     QuestHandler();
                     break;
                  case 2:
                     UpdateMap();
                     break;
                  case 3:
                     _currentMapItemType = MapItemType.Point; UpdateMap();
                     break;
                  case 4:
                     _currentMapItemType = MapItemType.Quest; UpdateMap();
                     break;
                  case 5:
                     _currentMapItemType = null; UpdateMap();
                     break;
                  default:
                     UpdateMap();
                     break;
               }
            };

            //actionSheetAlert.AddAction(UIAlertAction.Create("Захват", UIAlertActionStyle.Default, (action) => { ConquerHandler(); }));
            //actionSheetAlert.AddAction(UIAlertAction.Create("Квест", UIAlertActionStyle.Default, (action) => { QuestHandler(); }));
            //actionSheetAlert.AddAction(UIAlertAction.Create("Обновить", UIAlertActionStyle.Default, (action) => { UpdateMap(); }));
            //actionSheetAlert.AddAction(UIAlertAction.Create("Только точки", UIAlertActionStyle.Default, (action) => { _currentMapItemType = MapItemType.Point; UpdateMap(); }));
            //actionSheetAlert.AddAction(UIAlertAction.Create("Только квесты", UIAlertActionStyle.Default, (action) => { _currentMapItemType = MapItemType.Quest; UpdateMap(); }));
            //actionSheetAlert.AddAction(UIAlertAction.Create("Все объекты", UIAlertActionStyle.Default, (action) => { _currentMapItemType = null; UpdateMap(); }));

            //actionSheetAlert.AddAction(UIAlertAction.Create("Закрыть", UIAlertActionStyle.Cancel, (action) => Console.WriteLine("Cancel button pressed.")));
            //PresentViewController(actionSheetAlert, true, null);
            actionSheetAlert.ShowInView(View);
         }

      }

      public void HandleLocationChanged(object sender, LocationUpdatedEventArgs e)
      {
         _currentLocation = e.Location;
         if (_currentLocation != null)
         {
            UpdateNearestPointInformation();
            NavigationItem.Title = string.Format("{0} м: {1}", _distanceToNearestPoint.ToString("0.00"), _nameOfNearestPoint);
         }
      }

      private async void ConquerHandler()
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         string description = "GPS-координаты не определены, повторите попытку позже";
         if (_currentLocation != null)
         {
            Conquer result = await _apiService.Conquer(DeviceUtility.DeviceId, _currentLocation.Coordinate.Latitude.ProcessCoordinate(), _currentLocation.Coordinate.Longitude.ProcessCoordinate());
            description = result.GetDescription;
            if (result.IsSuccess)
            {
               await UpdateMapAsync();

               //_userActionService.Add(new UserAction
               //{
               //   Type = (int)MapItemType.Point,
               //   Title = result.title,
               //   Number = result.number,
               //   Description = result.GetDescription,
               //   Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
               //});
            }
         }

         IsLoading = false;
         ToastService.ShowMessage(description);
      }

      private async void QuestHandler()
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         string description = "GPS-координаты не определены, повторите попытку позже";
         if (_currentLocation != null)
         {
            Conquer result = await _apiService.Quest(DeviceUtility.DeviceId, _currentLocation.Coordinate.Latitude.ProcessCoordinate(), _currentLocation.Coordinate.Longitude.ProcessCoordinate());
            description = result.GetDescription;
            if (result.IsSuccess)
            {
               await UpdateMapAsync();

               //_userActionService.Add(new UserAction
               //{
               //   Type = (int)MapItemType.Quest,
               //   Title = result.title,
               //   Number = result.number,
               //   Description = result.GetDescription,
               //   Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
               //});
            }
         }

         IsLoading = false;
         ToastService.ShowMessage(description);
      }

      private object _lockObject = new object();

      private void UpdateNearestPointInformation()
      {
         _distanceToNearestPoint = float.MaxValue;
         Marker nearestMarker = new Marker();
         if (_currentLocation != null)
         {
            lock (_lockObject)
            {
               foreach (var marker in _markersList)
               {
                  double distance = _currentLocation.DistanceFrom(new CLLocation(marker.Position.Latitude, marker.Position.Longitude));
                  if (_distanceToNearestPoint > distance)
                  {
                     _distanceToNearestPoint = distance;
                     nearestMarker = marker;
                  }
               }
            }
         }
         _distanceToNearestPoint = Math.Round(_distanceToNearestPoint, 2);
         _nameOfNearestPoint = nearestMarker.Title;
      }

      protected override string LoadingMessage
      {
         get
         {
            return "Please, wait...";
         }
      }
   }
}

