using System;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class SettingsViewController : BaseViewController
   {
      public SettingsViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.More, 2);
      }
   }
}

