using Foundation;
using GO.Common.iOS.Helpers;
using GO.Common.iOS.Views;
using GoHunting.Core.Entities;
using UIKit;

namespace GO.Common.iOS.ViewControllers
{
   public class HistoryCell : UITableViewCell
   {
      public static readonly NSString CellIdentifier = new NSString("HistoryCell");

      private BaseLabel _titleLabel;
      private BaseLabel _descriptionLabel;
      private BaseLabel _dateLabel;

      public HistoryCell()
      {
         Initialize();
         Build();
      }

      private void Initialize()
      {
         _titleLabel = new BaseLabel
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap
         };
         _descriptionLabel = new BaseLabel
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap
         };
         _dateLabel = new BaseLabel
         {
         };
      }

      private void Build()
      {
         ContentView.AddSubviews(_titleLabel, _descriptionLabel, _dateLabel);

         ContentView.ConstrainLayout(() =>
             _titleLabel.Frame.Top == ContentView.Frame.Top + 3 &&
             _titleLabel.Frame.Left == ContentView.Frame.Left + 10 &&
             _titleLabel.Frame.Right == ContentView.Frame.Right &&

             _descriptionLabel.Frame.Top == _titleLabel.Frame.Bottom + 5 &&
             _descriptionLabel.Frame.Left == ContentView.Frame.Left + 10&&
             _descriptionLabel.Frame.Right == ContentView.Frame.Right &&

             _dateLabel.Frame.Top == _descriptionLabel.Frame.Bottom + 5 &&
             _dateLabel.Frame.Left == ContentView.Frame.Left + 10 &&
             _dateLabel.Frame.Right == ContentView.Frame.Right &&
             _dateLabel.Frame.Bottom == ContentView.Frame.Bottom - 3
         );
      }

      public void Update(UserAction userAction)
      {
         _titleLabel.Text = userAction.Title;
         _descriptionLabel.Text = userAction.Description;
         _dateLabel.Text = userAction.Date.ToString();
      }
   }
}