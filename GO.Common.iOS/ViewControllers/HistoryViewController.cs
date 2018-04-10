using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CoreGraphics;
using Foundation;
using GO.Core.Entities;
using GO.Core.Enums;
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

         var userActions = _userActionService.GetActions().OrderByDescending(x => x.Date);
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

         var userActions = _userActionService.GetActions().OrderByDescending(x => x.Date.DateTime);
         UserActionsItems = userActions.ToList();
         TableView.ReloadData();
      }

      public void ShowMenu(object sender, EventArgs e)
      {
         var alert = UIAlertController.Create("Выберите действие", null, UIAlertControllerStyle.ActionSheet);
         alert.AddAction(UIAlertAction.Create("Все действия", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetActions();
            TableView.ReloadData();
         }));

         alert.AddAction(UIAlertAction.Create("Только действия \"Точка\"", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetActions(ActionType.Point);
            TableView.ReloadData();
         }));

         alert.AddAction(UIAlertAction.Create("Только действия \"Квест\"", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetActions(ActionType.Quest);
            TableView.ReloadData();
         }));

         alert.AddAction(UIAlertAction.Create("Только действия \"Ловушка\"", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetActions(ActionType.Trap);
            TableView.ReloadData();
         }));

         alert.AddAction(UIAlertAction.Create("Только действия \"Поставить\"", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetActions(ActionType.Place);
            TableView.ReloadData();
         }));

         alert.AddAction(UIAlertAction.Create("Только действия \"Снести\"", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetActions(ActionType.Raze);
            TableView.ReloadData();
         }));

         alert.AddAction(UIAlertAction.Create("Только действия \"Атаковать\"", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            UserActionsItems = _userActionService.GetActions(ActionType.Attack);
            TableView.ReloadData();
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