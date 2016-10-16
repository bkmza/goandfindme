using System;

using UIKit;
using CoreGraphics;

using GoHunting.iOS.Helpers;

namespace GO.Common.iOS.Views
{
   public class LoadingView : BaseView
   {
      private WaitActivityIndicator _waitIndicator;
      public BaseLabel MessageLabel;
      float _alpha = 0.75f;

      public UIColor WaitIndicatorColor
      {
         set { _waitIndicator.Color = value; }
      }

      public LoadingView(CGRect frame, UIActivityIndicatorViewStyle style, string loadingMessage = null) : base(frame)
      {
         Init(new WaitActivityIndicator(style), loadingMessage);
      }

      public LoadingView(CGRect frame, string loadingMessage = null) : base(frame)
      {
         Init(new WaitActivityIndicator(), loadingMessage);
      }

      private void Init(WaitActivityIndicator indicator, string loadingMessage)
      {
         BackgroundColor = UIColor.Black;
         Alpha = _alpha;
         AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
         _waitIndicator = indicator;

         MessageLabel = new BaseLabel
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            TextAlignment = UITextAlignment.Center,
            AdjustsFontSizeToFitWidth = true,
            Text = loadingMessage,
            TextColor = UIColor.White
         };

         AddSubviews(_waitIndicator, MessageLabel);

         this.ConstrainLayout(() =>
            _waitIndicator.Frame.GetMidY() == Frame.GetMidY() &&
            _waitIndicator.Frame.GetMidX() == Frame.GetMidX() &&
            _waitIndicator.Frame.Width == 20 &&
            _waitIndicator.Frame.Height == 20 &&

            MessageLabel.Frame.GetMidX() == Frame.GetMidX() &&
            MessageLabel.Frame.Top == _waitIndicator.Frame.Bottom + 15
         );
         
         _waitIndicator.StartAnimating();
      }

      public void ShowOnView(UIView view, CGRect frame)
      {
         if (Superview != null && view != Superview && Handle != IntPtr.Zero)
         {
            InvokeOnMainThread(RemoveFromSuperview);
         }

         if (view != null && view != Superview && view.Handle != IntPtr.Zero)
         {
            InvokeOnMainThread(() => { view.Add(this); });
         }
      }

      public void ChangeBackgound(UIColor color, float alpha)
      {
         BackgroundColor = color;
         _alpha = alpha;
         Alpha = alpha;
      }

      public void Hide()
      {
         UIView.Animate(0.3, AnimationHandler, CompletionHandler);
      }

      void AnimationHandler()
      {
         Alpha = 0;
      }

      void CompletionHandler()
      {
         if (Handle == IntPtr.Zero)
            return;

         if (Superview == null)
            return;

         if (Superview.Handle == IntPtr.Zero)
            return;

         if (Superview.Subviews == null)
            return;

         InvokeOnMainThread(() =>
         {
            Alpha = _alpha;
            RemoveFromSuperview();
         });
      }
   }
} 