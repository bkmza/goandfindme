using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using CoreLocation;
using GO.Common.iOS.Utilities;
using GO.Common.iOS.Views;
using GO.Core;
using GO.Core.Data;
using GO.Core.Entities;
using GO.Core.Enums;
using GO.Core.Helpers;
using GO.Core.Services;
using GO.Core.Utilities;
using Google.Maps;
using MvvmCross.Platform;
using Newtonsoft.Json;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class MapViewController : BaseViewController
   {
      private UIBarButtonItem _actionsButton;

      private MapView _mapView;
      private List<Marker> _markersList;
      private MapItemType? _currentMapItemType;

      private IApiService _apiService;
      private IUserActionService _userActionService;

      private CLLocation _currentLocation;
      private double _distanceToNearestPoint;
      private string _nameOfNearestPoint;

      private object _lockObject = new object();

      public static bool UserInterfaceIdiomIsPhone => UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone;
      public static LocationManager Manager { get; set; }

      public MapViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.Search, 0);

         _apiService = Mvx.Resolve<IApiService>();
         _userActionService = Mvx.Resolve<IUserActionService>();
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

      public override void Initialize()
      {
         base.Initialize();

         var camera = CameraPosition.FromCamera(latitude: 53.900819, longitude: 27.558823, zoom: 10);
         _mapView = MapView.FromCamera(CGRect.Empty, camera);
         _mapView.MyLocationEnabled = true;
         _mapView.Settings.MyLocationButton = true;

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
         _markersList = new List<Marker>();

         UpdateMap();
         Manager = new LocationManager();
         Manager.StartLocationUpdates();

         View = _mapView;
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

      private async Task<bool> ValidateUserAccess()
      {
         ErrorInfo errorInfo = await _apiService.CheckUserAccess(DeviceUtility.DeviceId);
         if (errorInfo.status == "blocked")
         {
            ToastService.ShowMessage(errorInfo.message);
            return false;
         }
         return true;
      }

      private async Task UpdateMapAsync()
      {
         if (await ValidateUserAccess() == false)
         {
            return;
         }

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

      private UIView MarkerInfoWindow(UIView view, Marker marker) => new MarkerInfoWindowView(marker);

      public void ShowMenu(object sender, EventArgs e)
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

         if (await ValidateUserAccess() == false)
         {
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

               _userActionService.Add(new UserAction
               {
                  Type = (int)MapItemType.Point,
                  Title = result.title,
                  Number = result.number,
                  Description = result.GetDescription,
                  Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
               });
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

         if (await ValidateUserAccess() == false)
         {
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

               _userActionService.Add(new UserAction
               {
                  Type = (int)MapItemType.Quest,
                  Title = result.title,
                  Number = result.number,
                  Description = result.GetDescription,
                  Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
               });
            }
         }

         IsLoading = false;
         ToastService.ShowMessage(description);
      }

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

      protected override string LoadingMessage => "Please, wait...";
   }
}