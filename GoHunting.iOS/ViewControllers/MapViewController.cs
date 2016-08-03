using System;
using UIKit;
using CoreGraphics;

using Google.Maps;

namespace GoHunting.iOS.ViewControllers
{
   public class MapViewController : BaseViewController
   {
      private UIBarButtonItem _actionsButton;

      private MapView mapView;

      public MapViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.Favorites, 0);
      }

      public override void ViewWillAppear(bool animated)
      {
         base.ViewWillAppear(animated);

         _actionsButton = new UIBarButtonItem("Действия", UIBarButtonItemStyle.Plain, ShowMenu);
         NavigationItem.SetRightBarButtonItems(new[] { _actionsButton }, true);
      }

      public override void Initialize()
      {
         base.Initialize();

         var camera = CameraPosition.FromCamera(latitude: 37.79, 
                                         longitude: -122.40,
                                         zoom: 6);
         mapView = MapView.FromCamera(CGRect.Empty, camera);
         mapView.MyLocationEnabled = true;
         View = mapView;
      }

      public void ShowMenu(object sender, EventArgs e)
      {
         var alert = UIAlertController.Create("Выберите действие", null, UIAlertControllerStyle.ActionSheet);
         alert.AddAction(UIAlertAction.Create("Захват", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
         }));

         alert.AddAction(UIAlertAction.Create("Квест", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
         }));

         alert.AddAction(UIAlertAction.Create("Обновить", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
         }));

         alert.AddAction(UIAlertAction.Create("Только точки", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
         }));

         alert.AddAction(UIAlertAction.Create("Только квесты", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
         }));

         alert.AddAction(UIAlertAction.Create("Все объекты", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
         }));

         alert.AddAction(UIAlertAction.Create("Закрыть", UIAlertActionStyle.Cancel, null));
         PresentViewController(alert, true, null);
      }
   }
}

