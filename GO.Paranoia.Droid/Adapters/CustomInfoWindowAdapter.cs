using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Widget;
using GO.Core;
using GO.Core.Data;
using GO.Core.Enums;
using GO.Core.Services;
using MvvmCross.Platform;
using Newtonsoft.Json;

namespace GO.Paranoia.Droid.Adapters
{
   public class CustomInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
   {
      LayoutInflater _layoutInflater;
      Marker _marker;
      PointInfo _info;

      IToastService _toastService;

      public IntPtr IJHandle { get { return IntPtr.Zero; } }

      public CustomInfoWindowAdapter (LayoutInflater inflater, IToastService toastService)
      {
         _info = new PointInfo ();
         _layoutInflater = inflater;
         _toastService = toastService;
      }

      public View GetInfoWindow (Marker marker)
      {
         return null;
      }

      async void SetContents (string deviceId, string pointId, string type)
      {
         _info = new PointInfo ();
         _info = await Mvx.Resolve<IApiService> ().GetInfoAsync (deviceId, pointId, type);

         if (_info == null)
         {
            _toastService.ShowMessage(string.Format("Невозможно получить или обновить информацию. Проверьте подключение к интернету."));
            return;
         }

         if (_marker != null && _marker.IsInfoWindowShown) {
            _marker.ShowInfoWindow ();
         }
      }

      public View GetInfoContents (Marker marker)
      {
         var info = _info ?? new PointInfo();

         Point item = JsonConvert.DeserializeObject<Point> (marker.Snippet);
         bool needToRefresh = item.GetId != info.GetId;
         if (needToRefresh) {
            SetContents (DeviceUtility.DeviceId, item.id, item.type);
         }

         _marker = marker;

         int customPopupId;
         if (item.GetMapItemType == MapItemType.Point) {
            customPopupId = Resource.Layout.CustomMarkerPopupPoint;
         } else {
            customPopupId = Resource.Layout.CustomMarkerPopupQuest;
         }
         var customPopup = _layoutInflater.Inflate (customPopupId, null);

         var nameTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_Name);
         if (nameTextView != null) {
            nameTextView.Text = string.Format ("Название: {0}", marker.Title);
            nameTextView.SetTextColor(Android.Graphics.Color.ParseColor("#bdbdbd"));
         }

         var latLonTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_LatLonTextView);
         if (latLonTextView != null) {
            latLonTextView.Text = string.Format ("Координаты: {0}; {1}", marker.Position.Latitude, marker.Position.Longitude);
            latLonTextView.SetTextColor(Android.Graphics.Color.ParseColor("#bdbdbd"));
         }

         if (item.GetMapItemType == MapItemType.Point) {
            var allianceTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_AllianceTextView);
            if (allianceTextView != null) {
               allianceTextView.Text = string.Format ("Альянс: {0}", info.alliance);
               allianceTextView.SetTextColor(Android.Graphics.Color.ParseColor("#bdbdbd"));
            }

            var fractionTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_FractionTextView);
            if (fractionTextView != null) {
               fractionTextView.Text = string.Format ("Фракция: {0}", info.fraction);
               fractionTextView.SetTextColor(Android.Graphics.Color.ParseColor("#bdbdbd"));
            }
         }

         var descriptionTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_DescriptionTextView);
         if (descriptionTextView != null) {
            descriptionTextView.Text = string.Format ("Описание: {0}", info.description);
            descriptionTextView.SetTextColor(Android.Graphics.Color.ParseColor("#bdbdbd"));
         }

         return customPopup;
      }
   }
}

