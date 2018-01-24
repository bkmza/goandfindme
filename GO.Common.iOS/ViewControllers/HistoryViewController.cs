using System;
using GO.Common.iOS.Helpers;
using GO.Common.iOS.Views;
using GoHunting.Core.Services;
using MvvmCross.Platform;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryViewController : BaseViewController
   {
      private IUserActionService _userActionService;

      private UITableView _tableView;
      private UITableViewSource _tableViewSource;

      public HistoryViewController()
      {
         TabBarItem = new UITabBarItem(UITabBarSystemItem.History, 1);

         _userActionService = Mvx.Resolve<IUserActionService>();
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

         var userActions = _userActionService.GetQuests();
         _tableViewSource = new HistoryTableViewSource(userActions.ToArray());
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
   }
}

