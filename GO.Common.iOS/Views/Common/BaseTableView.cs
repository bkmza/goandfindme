using System;
using CoreGraphics;
using UIKit;

namespace GO.Common.iOS.Views
{
   public class BaseTableView : UITableView
   {
      public BaseTableView()
      {
         SetContentInsetAdjustmentBehaviorToNever();
      }

      public BaseTableView(IntPtr handle) : base(handle)
      {
         SetContentInsetAdjustmentBehaviorToNever();
      }

      public BaseTableView(CGRect frame) : base(frame)
      {
         SetContentInsetAdjustmentBehaviorToNever();
      }

      public BaseTableView(CGRect frame, UITableViewStyle style) : base(frame, style)
      {
         SetContentInsetAdjustmentBehaviorToNever();
      }

      private void SetContentInsetAdjustmentBehaviorToNever()
      {
         if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
         {
            ContentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentBehavior.Never;
         }
      }
   }
}