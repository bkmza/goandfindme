using System;
using CoreGraphics;
using GoHunting.Core.Data;
using GoHunting.Core.Enums;
using GoHunting.Core.Utilities;
using GO.Common.iOS.Helpers;
using Google.Maps;
using Newtonsoft.Json;
using UIKit;

namespace GO.Common.iOS.Views
{
   public class MarkerInfoWindowView : BaseView
   {
      private BaseLabel _titleLabel;
      private BaseLabel _coordLabel;
      private BaseLabel _allianceLabel;
      private BaseLabel _fractionLabel;
      private BaseLabel _descriptionLabel;

      public MarkerInfoWindowView(IntPtr handle) : base(handle)
      {
      }

      public MarkerInfoWindowView(Marker marker)
      {
         PointInfo item;
         try
         {
            item = JsonConvert.DeserializeObject<PointInfo>(marker.Title);
            marker.Title = item.name;
         }
         catch (Exception ex)
         {
            Logger.Instance.Debug(string.Format("MarkerInfoWindowView exception: {0}", ex.Message));
            item = new PointInfo();
         }

         _titleLabel = new BaseLabel(new CGRect(0, 0, UIHelper.ScreenWidth, 20))
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            Text = string.Format("Название: {0}", item.name)
         };
         AddSubview(_titleLabel);

         _coordLabel = new BaseLabel(new CGRect(0, _titleLabel.Frame.Bottom, UIHelper.ScreenWidth, 20))
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            Text = string.Format("Координаты: {0} ; {1}", marker.Position.Latitude, marker.Position.Longitude)
         };
         AddSubview(_coordLabel);

         _allianceLabel = new BaseLabel(new CGRect(0, _coordLabel.Frame.Bottom, UIHelper.ScreenWidth, 20))
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            Text = string.Format("Альянс: {0}", item.alliance ?? "-")
         };
         AddSubview(_allianceLabel);

         _fractionLabel = new BaseLabel(new CGRect(0, _allianceLabel.Frame.Bottom, UIHelper.ScreenWidth, 20))
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            Text = string.Format("Фракция: {0}", item.fraction ?? "-")
         };
         AddSubview(_fractionLabel);

         _descriptionLabel = new BaseLabel(new CGRect(0, _fractionLabel.Frame.Bottom, UIHelper.ScreenWidth, 0))
         {
            Lines = 0,
            LineBreakMode = UILineBreakMode.WordWrap,
            Text = string.Format("Описание: {0}", item.description ?? "-")
         };
         _descriptionLabel.SizeToFit();
         _descriptionLabel.Frame = new CGRect(0, _fractionLabel.Frame.Bottom, UIHelper.ScreenWidth, _descriptionLabel.Frame.Height);
         AddSubview(_descriptionLabel);

         Frame = new CGRect(0, 0, UIHelper.ScreenWidth, _descriptionLabel.Frame.Bottom);
         BackgroundColor = UIColor.White;
      }
   }
}

