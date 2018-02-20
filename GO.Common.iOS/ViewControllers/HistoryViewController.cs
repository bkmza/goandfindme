using System;
using System.Collections.Generic;
using System.Linq;
using GO.Common.iOS.Helpers;
using GO.Common.iOS.Views;
using GO.Core.Entities;
using GO.Core.Services;
using MvvmCross.Platform;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryViewController : BaseViewController
   {
      private IUserActionService _userActionService;

      private UIBarButtonItem _actionsButton;
      public UITableView HistoryTableView;
      private HistoryTableViewSource _tableViewSource;

      public HistoryViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.History, 1);
         NavigationItem.Title = "History";

         _userActionService = Mvx.Resolve<IUserActionService>();
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
         _tableViewSource.UpdateSource(HistoryTableView, userActions.OrderByDescending(x => x.Date).ToArray());
      }

      public override void Initialize()
      {
         base.Initialize();

         HistoryTableView = new BaseTableView
         {
            ScrollEnabled = true,
            TableHeaderView = new UIView(),
            TableFooterView = new UIView(),
            SeparatorInset = UIEdgeInsets.Zero,
            LayoutMargins = UIEdgeInsets.Zero,
            EstimatedRowHeight = UITableView.AutomaticDimension,
            RowHeight = UITableView.AutomaticDimension,
            BackgroundColor = UIColor.White
         };

         _tableViewSource = new HistoryTableViewSource(new WeakReference(this));
         HistoryTableView.Source = _tableViewSource;
      }

      public override void Build()
      {
         base.Build();

         View.AddSubviews(HistoryTableView);

         View.ConstrainLayout(() =>
            HistoryTableView.Frame.Top == View.Frame.Top &&
            HistoryTableView.Frame.Left == View.Frame.Left &&
            HistoryTableView.Frame.Right == View.Frame.Right &&
            HistoryTableView.Frame.Bottom == View.Frame.Bottom
         );
      }

      public void ShowMenu(object sender, EventArgs e)
      {
         List<UserAction> userActions = null;
         var alert = UIAlertController.Create("Выберите действие", null, UIAlertControllerStyle.ActionSheet);
         alert.AddAction(UIAlertAction.Create("Все", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            userActions = _userActionService.GetAllTypes();
            _tableViewSource.UpdateSource(HistoryTableView, userActions.OrderByDescending(x => x.Date).ToArray());
         }));

         alert.AddAction(UIAlertAction.Create("Только точки", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            userActions = _userActionService.GetConquers();
            _tableViewSource.UpdateSource(HistoryTableView, userActions.OrderByDescending(x => x.Date).ToArray());
         }));

         alert.AddAction(UIAlertAction.Create("Только квесты", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            userActions = _userActionService.GetQuests();
            _tableViewSource.UpdateSource(HistoryTableView, userActions.OrderByDescending(x => x.Date).ToArray());
         }));
         alert.AddAction(UIAlertAction.Create("Закрыть", UIAlertActionStyle.Cancel, null));
         PresentViewController(alert, true, null);
      }

      public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
      {
         base.TraitCollectionDidChange(previousTraitCollection);

         if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available)
         {
            RegisterForPreviewingWithDelegate(new HistoryViewControllerPreviewingDelegate(this), View);
         }
      }
   }
}