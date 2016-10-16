using System;

using CoreGraphics;
using UIKit;
using Foundation;

using GO.Common.iOS.Views;
using GO.Common.iOS.Utilities;
using GoHunting.Core.Services;
using MvvmCross.Platform;

namespace GO.Common.iOS.ViewControllers
{
   public class BaseViewController : UIViewController
   {
      protected LoadingView LoadingView;

      protected IToastService ToastService;
      protected IAnalyticsService AnalyticsService;

      public BaseViewController ()
      {
         ToastService = Mvx.Resolve<IToastService>();
         AnalyticsService = Mvx.Resolve<IAnalyticsService>();
      }

      public override void TouchesBegan(NSSet touches, UIEvent evt)
      {
         View.EndEditing(true);
         base.TouchesBegan(touches, evt);
      }

      public override void ViewDidLoad ()
      {
         base.ViewDidLoad ();

         Initialize ();

         Build ();
      }

      public virtual void Initialize ()
      {
         LoadingView = new LoadingView(this.LoaderFrame, this.LoadingMessage);
      }

      public virtual void Build ()
      {
      }

      public virtual bool IsLoading
      {
         get
         {
            return _isLoading;
         }
         set
         {
            if (_isLoading != value)
            {
               if (value)
               {
                  //ViewIsUpdating();
                  if (UIApplication.SharedApplication?.KeyWindow?.RootViewController?.View != null)
                  {
                     LoadingView.ShowOnView(UIApplication.SharedApplication.KeyWindow.RootViewController.View, UIScreen.MainScreen.Bounds);
                  }
                  else
                  {
                     LoadingView.ShowOnView(View, UIScreen.MainScreen.Bounds);
                  }
               }
               else
               {
                  //ViewIsUpdated();
                  InvokeOnMainThread(LoadingView.Hide);
               }
               _isLoading = value;
            }
         }
      }

      private bool _isLoading;

      protected virtual CGRect LoaderFrame
      {
         get
         {
            return View.Frame;
         }
      }

      protected virtual string LoadingMessage
      {
         get
         {
            return string.Empty;
         }
      }

      protected bool CheckInternetConnection()
      {
         if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
         {
            if (ReachabilityHost.IsHostReachable("http://google.com"))
            {
               return true;
            }

            ToastService.ShowMessage("Необходимо подключение к интернету");
            return false;
         }
         else
         {
            return true;
         }
      }
   }
}

