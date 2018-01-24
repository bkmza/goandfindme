using System;
using Foundation;
using GoHunting.Core.Entities;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryTableViewSource : UITableViewSource
   {
      private readonly UserAction[] _userActions;

      public HistoryTableViewSource(UserAction[] userActions)
      {
         _userActions = userActions;
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

         if(cell is HistoryCell historyCell)
         {
            historyCell.Update(item);
         }
         return cell;
      }
   }
}
