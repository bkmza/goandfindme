using System;
using System.Collections.Generic;
using System.Linq;
using GO.Common.iOS.Helpers;
using GO.Common.iOS.Views;
using GoHunting.Core.Entities;
using GoHunting.Core.Services;
using MvvmCross.Platform;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryViewController : BaseViewController
   {
      private IUserActionService _userActionService;

      private UIBarButtonItem _actionsButton;
      private UITableView _tableView;
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
      }

      public override void Initialize()
      {
         base.Initialize();

         _tableView = new BaseTableView
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

         var userActions = _userActionService.GetAllTypes().OrderByDescending(x => x.Date);
         _tableViewSource = new HistoryTableViewSource(userActions, new WeakReference(this));
         _tableView.Source = _tableViewSource;
      }

      public override void Build()
      {
         base.Build();

         View.AddSubviews(_tableView);

         View.ConstrainLayout(() =>
            _tableView.Frame.Top == View.Frame.Top &&
            _tableView.Frame.Left == View.Frame.Left &&
            _tableView.Frame.Right == View.Frame.Right &&
            _tableView.Frame.Bottom == View.Frame.Bottom
         );
      }

      public void ShowMenu(object sender, EventArgs e)
      {
         List<UserAction> userActions = null;
         var alert = UIAlertController.Create("Выберите действие", null, UIAlertControllerStyle.ActionSheet);
         alert.AddAction(UIAlertAction.Create("Все", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            userActions = _userActionService.GetAllTypes();
            _tableViewSource.UpdateSource(_tableView, userActions.OrderByDescending(x => x.Date).ToArray());
         }));

         alert.AddAction(UIAlertAction.Create("Только точки", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            userActions = _userActionService.GetConquers();
            _tableViewSource.UpdateSource(_tableView, userActions.OrderByDescending(x => x.Date).ToArray());
         }));

         alert.AddAction(UIAlertAction.Create("Только квесты", UIAlertActionStyle.Default, (UIAlertAction obj) =>
         {
            userActions = _userActionService.GetQuests();
            _tableViewSource.UpdateSource(_tableView, userActions.OrderByDescending(x => x.Date).ToArray());
         }));
         alert.AddAction(UIAlertAction.Create("Закрыть", UIAlertActionStyle.Cancel, null));
         PresentViewController(alert, true, null);
      }
   }
}