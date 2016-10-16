using System;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryViewController : BaseViewController
   {
      public HistoryViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.History, 1);
      }
   }
}

