using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryViewControllerPreviewingDelegate : UIViewControllerPreviewingDelegate
   {
      private HistoryViewController _historyViewController;

      public HistoryViewControllerPreviewingDelegate(HistoryViewController historyViewController)
      {
         _historyViewController = historyViewController;
      }

      public HistoryViewControllerPreviewingDelegate(NSObjectFlag t) : base(t) { }

      public HistoryViewControllerPreviewingDelegate(IntPtr handle) : base(handle) { }

      public override void CommitViewController(IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
      {
         _historyViewController.ShowViewController(viewControllerToCommit, this);
      }

      public override UIViewController GetViewControllerForPreview(IUIViewControllerPreviewing previewingContext, CGPoint location)
      {
         var indexPath = _historyViewController.HistoryTableView.IndexPathForRowAtPoint(location);
         var cell = _historyViewController.HistoryTableView.CellAt(indexPath);
         var source = _historyViewController.HistoryTableView.Source as HistoryTableViewSource;
         var item = source.UserActionsItems[indexPath.Row];

         var controller = new HistoryDetailsViewController();
         controller.SetDetailItem(item);

         previewingContext.SourceRect = cell.Frame;

         return controller;
      }
   }
}
