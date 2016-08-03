using Foundation;
using UIKit;

using GoHunting.iOS.ViewControllers;

namespace GoHunting.iOS
{
   [Register ("AppDelegate")]
   public class AppDelegate : UIApplicationDelegate
   {
      public static AppDelegate Shared;

      private UIWindow _window;

      public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
      {
         Shared = this;

         _window = new UIWindow (UIScreen.MainScreen.Bounds);

         _window.RootViewController = new StartViewController ();

         _window.MakeKeyAndVisible ();

         return true;
      }

      public override void OnResignActivation (UIApplication application)
      {

      }

      public override void DidEnterBackground (UIApplication application)
      {

      }

      public override void WillEnterForeground (UIApplication application)
      {

      }

      public override void OnActivated (UIApplication application)
      {
      }

      public override void WillTerminate (UIApplication application)
      {
      }

      public void SetRootViewController(UIViewController controller)
      {
         _window.RootViewController = controller;
      }
   }
}


