using System;
using UIKit;

namespace GO.Common.iOS.ViewControllers
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

