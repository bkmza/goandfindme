using System;

using UIKit;

using GoHunting.iOS.Views;
using GoHunting.iOS.Helpers;

namespace GoHunting.iOS.ViewControllers
{
   public class StartViewController : BaseViewController
   {
      private BaseLabel _nameLabel;
      private BaseTextField _nameTextField;

      private BaseLabel _commentLabel;
      private BaseTextField _commentTextField;

      private BaseButton _loginButton;

      public StartViewController ()
      {
      }

      public override void ViewWillAppear (bool animated)
      {
         base.ViewWillAppear (animated);

         View.BackgroundColor = UIColor.White;

         if (_loginButton != null)
         {
            _loginButton.TouchUpInside += LoginButtonTouchInsideHandler;
         }
      }

      public override void ViewWillDisappear(bool animated)
      {
         base.ViewWillDisappear(animated);

         if (_loginButton != null)
         {
            _loginButton.TouchUpInside -= LoginButtonTouchInsideHandler;
         }
      }

      public override void Initialize ()
      {
         base.Initialize ();

         _nameLabel = new BaseLabel();
         _nameLabel.AttributedText = StringHelper.GetAttributedString("Test1", 3, UITextAlignment.Center);

         _nameTextField = new BaseTextField();

         _commentLabel = new BaseLabel();
         _commentLabel.AttributedText = StringHelper.GetAttributedString("Test2", 3, UITextAlignment.Center);

         _commentTextField = new BaseTextField();

         _loginButton = new BaseButton();
         _loginButton.SetTitle("Confirm", UIControlState.Normal);
         _loginButton.BackgroundColor = UIColor.Black;
         _loginButton.TintColor = UIColor.White;
      }

      public override void Build ()
      {
         base.Build ();

         View.AddSubviews(_nameLabel, _nameTextField, _commentLabel, _commentTextField, _loginButton);

         View.ConstrainLayout(() =>
            _nameLabel.Frame.Top == View.Frame.Top + 50 &&
         _nameLabel.Frame.Left == View.Frame.Left + 15 &&
         _nameLabel.Frame.Right == View.Frame.Right - 15 &&
         _nameLabel.Frame.Height == 20 &&

         _nameTextField.Frame.Top == _nameLabel.Frame.Bottom + 10 &&
         _nameTextField.Frame.Left == View.Frame.Left + 15 &&
         _nameTextField.Frame.Right == View.Frame.Right - 15 &&
         _nameTextField.Frame.Height == 20 &&

         _commentLabel.Frame.Top == _nameTextField.Frame.Bottom + 50 &&
         _commentLabel.Frame.Left == View.Frame.Left + 15 &&
         _commentLabel.Frame.Right == View.Frame.Right - 15 &&
         _commentLabel.Frame.Height == 20 &&

         _commentTextField.Frame.Top == _commentLabel.Frame.Bottom + 10 &&
         _commentTextField.Frame.Left == View.Frame.Left + 15 &&
         _commentTextField.Frame.Right == View.Frame.Right - 15 &&
         _commentTextField.Frame.Height == 20 &&

         _loginButton.Frame.Top == _commentTextField.Frame.Bottom + 10 &&
         _loginButton.Frame.Left == View.Frame.Left + 15 &&
         _loginButton.Frame.Right == View.Frame.Right - 15 &&
         _loginButton.Frame.Height == 44
         );
      }

      private void LoginButtonTouchInsideHandler(object sender, EventArgs e)
      {
         var tabBarViewController = new MainTabBarViewController();

         var mapController = new UINavigationController();
         mapController.PushViewController(new MapViewController(), false);

         var historyController = new UINavigationController();
         historyController.PushViewController(new HistoryViewController(), false);

         var settingsController = new UINavigationController();
         settingsController.PushViewController(new SettingsViewController(), false);
        
         tabBarViewController.SetViewControllers(new[] { mapController, historyController, settingsController }, true);

         AppDelegate.Shared.SetRootViewController(tabBarViewController);
      }
   }
}

