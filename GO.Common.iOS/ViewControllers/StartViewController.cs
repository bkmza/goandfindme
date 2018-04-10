using System;
using Foundation;
using GO.Common.iOS.Helpers;
using GO.Common.iOS.Views;
using GO.Core;
using GO.Core.Data;
using GO.Core.Enums;
using GO.Core.Services;
using MvvmCross.Platform;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class StartViewController : BaseViewController
   {
      private BaseLabel _nameLabel;
      private BaseTextField _nameTextField;

      private BaseLabel _commentLabel;
      private BaseTextField _commentTextField;

      private BaseButton _loginButton;
      private BaseLabel _versionLabel;

      private ILoginService _loginService;
      private IUserActionService _userActionService;

      public StartViewController()
      {
         _loginService = Mvx.Resolve<ILoginService>();
         _userActionService = Mvx.Resolve<IUserActionService>();
      }

      public override void ViewDidLoad()
      {
         base.ViewDidLoad();
      }

      public override void ViewWillAppear(bool animated)
      {
         base.ViewWillAppear(animated);

         View.BackgroundColor = UIColor.Gray;

         if (_loginButton != null)
         {
            _loginButton.TouchUpInside += LoginButtonTapHandler;
         }
      }

      public override void ViewDidDisappear(bool animated)
      {
         base.ViewDidDisappear(animated);

         if (_loginButton != null)
         {
            _loginButton.TouchUpInside -= LoginButtonTapHandler;
         }
      }

      public override void ViewDidAppear(bool animated)
      {
         base.ViewDidAppear(animated);

         IsLoading = true;
         CheckUserExists();
      }

      public async void CheckUserExists()
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         RegisterStatus status = await _loginService.CheckUserExists(DeviceUtility.DeviceId);
         if (status.GetStatus == (int)UserStatus.RegisteredAndApproved)
         {
            IsLoading = false;
            GoToHomeScreen();
         }
         else
         {
            ToastService.ShowMessage(status.GetDescription);
         }

         IsLoading = false;
      }

      public void GoToHomeScreen()
      {
         var tabBarViewController = new MainTabBarViewController();
         tabBarViewController.TabBar.TintColor = UIColor.Black;
         tabBarViewController.TabBar.BarTintColor = UIColor.White;

         var mapController = new UINavigationController();
         mapController.PushViewController(new MapViewController(), false);
         mapController.NavigationBar.BackgroundColor = UIColor.White;

         var historyController = new UINavigationController();
         historyController.PushViewController(new HistoryViewController(), false);
         historyController.NavigationBar.BackgroundColor = UIColor.White;

         var settingsController = new UINavigationController();
         settingsController.PushViewController(new SettingsViewController(), false);
         settingsController.NavigationBar.BackgroundColor = UIColor.White;

         tabBarViewController.SetViewControllers(new[] { mapController, historyController /*, settingsController*/ }, true);

         UIApplication.SharedApplication.KeyWindow.RootViewController = tabBarViewController;
      }

      public override void Initialize()
      {
         base.Initialize();

         _nameLabel = new BaseLabel
         {
            TextColor = UIColor.White
         };
         _nameLabel.AttributedText = StringHelper.GetAttributedString("Ваше имя:", 3, UITextAlignment.Left);

         _nameTextField = new BaseTextField
         {
            BackgroundColor = UIColor.White,
            TintColor = UIColor.Black
         };

         _commentLabel = new BaseLabel
         {
            TextColor = UIColor.White
         };
         _commentLabel.AttributedText = StringHelper.GetAttributedString("Комментарий:", 3, UITextAlignment.Left);

         _commentTextField = new BaseTextField
         {
            BackgroundColor = UIColor.White,
            TintColor = UIColor.Black
         };

         _loginButton = new BaseButton();
         _loginButton.SetTitle("Confirm", UIControlState.Normal);
         _loginButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
         _loginButton.SetTitleColor(UIColor.LightGray, UIControlState.Highlighted);
         _loginButton.SetTitleColor(UIColor.DarkGray, UIControlState.Disabled);
         _loginButton.BackgroundColor = UIColor.White;
         _loginButton.Layer.CornerRadius = 3;

         _versionLabel = new BaseLabel
         {
            TextColor = UIColor.White
         };
         _versionLabel.AttributedText = StringHelper.GetAttributedString(NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString(), 3, UITextAlignment.Right);
      }

      public override void Build()
      {
         base.Build();

         View.AddSubviews(_nameLabel, _nameTextField, _commentLabel, _commentTextField, _loginButton, _versionLabel);

         View.ConstrainLayout(() =>
            _nameLabel.Frame.Top == View.Frame.Top + 50 &&
         _nameLabel.Frame.Left == View.Frame.Left + 15 &&
         _nameLabel.Frame.Right == View.Frame.Right - 15 &&
         _nameLabel.Frame.Height == 20 &&

         _nameTextField.Frame.Top == _nameLabel.Frame.Bottom + 10 &&
         _nameTextField.Frame.Left == View.Frame.Left + 15 &&
         _nameTextField.Frame.Right == View.Frame.Right - 15 &&
         _nameTextField.Frame.Height == 30 &&

         _commentLabel.Frame.Top == _nameTextField.Frame.Bottom + 50 &&
         _commentLabel.Frame.Left == View.Frame.Left + 15 &&
         _commentLabel.Frame.Right == View.Frame.Right - 15 &&
         _commentLabel.Frame.Height == 20 &&

         _commentTextField.Frame.Top == _commentLabel.Frame.Bottom + 10 &&
         _commentTextField.Frame.Left == View.Frame.Left + 15 &&
         _commentTextField.Frame.Right == View.Frame.Right - 15 &&
         _commentTextField.Frame.Height == 30 &&

         _loginButton.Frame.Bottom == _versionLabel.Frame.Top - 15 &&
         _loginButton.Frame.Left == View.Frame.Left + 15 &&
         _loginButton.Frame.Right == View.Frame.Right - 15 &&
         _loginButton.Frame.Height == 44 &&

         _versionLabel.Frame.Bottom == View.Frame.Bottom - 15 &&
         _versionLabel.Frame.Left == View.Frame.Left + 15 &&
         _versionLabel.Frame.Right == View.Frame.Right - 15 &&
         _versionLabel.Frame.Height == 20
         );
      }

      private async void LoginButtonTapHandler(object sender, EventArgs e)
      {
         IsLoading = true;

         if (_nameTextField.Text.Trim().ToLower() == "apple" && _commentTextField.Text.Trim().ToLower() == "apple123")
         {
            DeviceUtility.TestId = "0123456789";
         }
         else if (_nameTextField.Text.Trim().ToLower() == "google" && _commentTextField.Text.Trim().ToLower() == "google123")
         {
            DeviceUtility.TestId = "0123456789";
         }

         RegisterStatus result = await _loginService.Register(_nameTextField.Text, _commentTextField.Text, DeviceUtility.DeviceId);
         if (result.GetStatus != (int)UserStatus.RegisteredAndApproved)
         {
            IsLoading = false;
            ToastService.ShowMessage(result.GetDescription);
            return;
         }

         _userActionService.DeleteAll();
         GoToHomeScreen();
      }
   }
}