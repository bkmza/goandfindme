using System;
using UIKit;

namespace GoHunting.iOS.ViewControllers
{
   public class MainTabBarViewController : UITabBarController
   {
      public MainTabBarViewController()
      {
      }

      public override void SetViewControllers(UIViewController[] viewControllers, bool animated)
      {
         base.SetViewControllers(viewControllers, animated);
      }
   }
}

