using System;
using UIKit;

namespace GoHunting.iOS.ViewControllers
{
   public class HistoryViewController : BaseViewController
   {
      public HistoryViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.History, 1);
      }
   }
}

