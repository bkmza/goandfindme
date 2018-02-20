using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CoreGraphics;
using Foundation;
using GO.Core.Entities;
using GO.Core.Services;
using MvvmCross.Platform;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryViewController : UITableViewController, IUIViewControllerPreviewingDelegate
   {
      private IUserActionService _userActionService;

      public List<UserAction> UserActionsItems;

      private UIBarButtonItem _actionsButton;

      public HistoryViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.History, 1);
         NavigationItem.Title = "History";

         _userActionService = Mvx.Resolve<IUserActionService>();
      }

      public override void ViewDidLoad()
      {
         base.ViewDidLoad();

         var userActions = _userActionService.GetAllTypes().OrderByDescending(x => x.Date);
         UserActionsItems = userActions.ToList();
      }

      public override void ViewWillAppear(bool animated)
      {
         base.ViewWillAppear(animated);

         _actionsButton = new UIBarButtonItem(UIBarButtonSystemItem.Search, ShowMenu)
         {
            TintColor = UIColor.Black
         };
         NavigationItem.SetRightBarButtonItems(new[] { _actionsButton }, true);

         var userActions = _userActionService.GetAllTypes().OrderByDescending(x => x.Date);
         UserActionsItems = userActions.ToList();
         TableView.ReloadData();
      }

      public void ShowMenu(object sender, EventArgs e)
      {
         var alert = UIAlertController.Create("Выберите действие", null, UIAlertControllerStyle.ActionSheet);
         alert.AddAction(UIAlertAction.Create("Все", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetAllTypes();
         }));

         alert.AddAction(UIAlertAction.Create("Только точки", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetConquers();
         }));

         alert.AddAction(UIAlertAction.Create("Только квесты", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetQuests();
         }));
         alert.AddAction(UIAlertAction.Create("Закрыть", UIAlertActionStyle.Cancel, null));
         PresentViewController(alert, true, null);
      }

      public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
      {
         base.TraitCollectionDidChange(previousTraitCollection);

         if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available)
         {
            RegisterForPreviewingWithDelegate(this, View);
         }
      }

      public override nint RowsInSection(UITableView tableView, nint section) => UserActionsItems.Count;

      public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
      {
         var cell = tableView.DequeueReusableCell(HistoryCell.CellIdentifier);
         var item = UserActionsItems[indexPath.Row];

         if (cell == null)
            cell = new HistoryCell();

         if (cell is HistoryCell historyCell)
         {
            historyCell.Update(item);
         }
         return cell;
      }

      public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
      {
         var item = UserActionsItems[indexPath.Row];
         // Description can contains URL to the web
         // if yes - open webview
         foreach (Match match in Regex.Matches(item.Description, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?"))
         {
            NSUrl url = NSUrl.FromString(match.Value);
            UIApplication.SharedApplication.OpenUrl(url);
            return;
         }
      }

      public UIViewController GetViewControllerForPreview(IUIViewControllerPreviewing previewingContext, CGPoint location)
      {
         var indexPath = TableView.IndexPathForRowAtPoint(location);
         var cell = TableView.CellAt(indexPath);
         var item = UserActionsItems[indexPath.Row];

         var controller = new HistoryDetailsViewController();
         controller.PreferredContentSize = new CGSize(0, 0);
         controller.SetDetailItem(item);

         previewingContext.SourceRect = cell.Frame;

         return controller;
      }

      public void CommitViewController(IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
      {
         ShowViewController(viewControllerToCommit, this);
      }
   }
}