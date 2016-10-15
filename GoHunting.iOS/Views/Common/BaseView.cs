﻿using System;

using CoreGraphics;
using UIKit;

namespace GoHunting.iOS.Views
{
   public class BaseView : UIView
   {
      public BaseView ()
      {
      }

      public BaseView (IntPtr handle) : base (handle)
      {
      }

      public BaseView(CGRect frame) : base(frame)
      {
      }
   }
}

