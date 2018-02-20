﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Foundation;
using GO.Core.Entities;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryTableViewSource : UITableViewSource
   {
      public UserAction[] UserActionsItems;

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

      public HistoryTableViewSource(WeakReference weakViewController)
      {
         UserActionsItems = new List<UserAction>().ToArray();
         _weakViewController = weakViewController;
      }

      public override nint RowsInSection(UITableView tableview, nint section)
      {
         return UserActionsItems.Length;
      }

      public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
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

      public void UpdateSource(UITableView tableView, UserAction[] userActions)
      {
         UserActionsItems = userActions;
         tableView.ReloadData();
      }
   }
}