using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Foundation;
using GoHunting.Core.Entities;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryTableViewSource : UITableViewSource
   {
      private UserAction[] _userActions;

      private readonly WeakReference _weakViewController;

      protected HistoryViewController ParentViewController
      {
         get
         {
            if (_weakViewController == null || !_weakViewController.IsAlive)
               return null;
            return _weakViewController.Target as HistoryViewController;
         }
      }

      public HistoryTableViewSource(IEnumerable<UserAction> userActions, WeakReference weakViewController)
      {
         _userActions = userActions.ToArray();
         _weakViewController = weakViewController;
      }

      public override nint RowsInSection(UITableView tableview, nint section)
      {
         return _userActions.Length;
      }

      public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
      {
         var cell = tableView.DequeueReusableCell(HistoryCell.CellIdentifier);
         var item = _userActions[indexPath.Row];

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
         var item = _userActions[indexPath.Row];
         // Description can contains URL to the web
         // if yes - open webview
         foreach (Match match in Regex.Matches(item.Description, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?"))
         {
            NSUrl url = NSUrl.FromString(match.Value);
            UIApplication.SharedApplication.OpenUrl(url);
            return;
         }
      }

      public void UpdateSource(UITableView tableView, UserAction[] userActions)
      {
         _userActions = userActions;
         tableView.ReloadData();
      }
   }
}