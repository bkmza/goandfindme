using System;
using UIKit;

namespace GoHunting.iOS.ViewControllers
{
   public class SettingsViewController : BaseViewController
   {
      public SettingsViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.More, 2);
      }
   }
}

