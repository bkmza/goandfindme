using System;
using GO.Core.Entities;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryDetailsViewController : BaseViewController
   {
      public UserAction Item;

      public HistoryDetailsViewController()
      {
      }

      public override IUIPreviewActionItem[] PreviewActionItems => PreviewActions;

      private IUIPreviewActionItem[] PreviewActions
      {
         get
         {
            var action1 = PreviewActionForTitle("Default Action");
            var action2 = PreviewActionForTitle("Destructive Action", UIPreviewActionStyle.Destructive);

            var subAction1 = PreviewActionForTitle("Sub Action 1");
            var subAction2 = PreviewActionForTitle("Sub Action 2");
            var groupedActions = UIPreviewActionGroup.Create("Sub Actions…", UIPreviewActionStyle.Default, new[] { subAction1, subAction2 });

            return new IUIPreviewActionItem[] { action1, action2, groupedActions };
         }
      }

      public void SetDetailItem(UserAction item)
      {
         Item = item;
      }

      public override void ViewDidLoad()
      {
         base.ViewDidLoad();

         NavigationItem.LeftBarButtonItem = SplitViewController?.DisplayModeButtonItem;
         NavigationItem.LeftItemsSupplementBackButton = true;
      }

      static UIPreviewAction PreviewActionForTitle(string title, UIPreviewActionStyle style = UIPreviewActionStyle.Default)
      {
         return UIPreviewAction.Create(title, style, (action, previewViewController) =>
         {
            var detailViewController = (HistoryDetailsViewController)previewViewController;
            var item = detailViewController.Item.Date;

            Console.WriteLine("{0} triggered from `DetailViewController` for item: {1}", action.Title, item);
         });
      }
   }
}