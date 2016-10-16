using System;

using CoreGraphics;
using UIKit;

namespace GO.Common.iOS.Views
{
   public class WaitActivityIndicator : UIActivityIndicatorView
   {
      private static UIActivityIndicatorViewStyle Style = UIActivityIndicatorViewStyle.White;

      private bool _visible;

      public bool Visible1
      {
         get
         {
            return _visible;
         }
         set
         {
            _visible = value;

            if (!_visible)
            {
               StopAnimating();
            }
            else
            {
               StartAnimating();
            }
         }
      }

      public WaitActivityIndicator() : this(Style)
      {
      }

      public WaitActivityIndicator(UIActivityIndicatorViewStyle style, float scale = 1.4f) : base(style)
      {
         Frame = new CGRect(0, 0, 20, 20);
         Transform = CGAffineTransform.MakeScale(scale, scale);
      }
   }
}

