using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using GoHunting.Core;
using GoHunting.Core.Data;
using GoHunting.Core.Services;

namespace DroidMapping.Adapters
{
	public class CustomInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
	{
		private LayoutInflater _layoutInflater;
		Marker _marker;
		PointInfo _info;
		bool NeedToRefresh;

		public IntPtr IJHandle { get { return IntPtr.Zero; } }

		public CustomInfoWindowAdapter (LayoutInflater inflater)
		{
			_info = new PointInfo ();
			_layoutInflater = inflater;
		}

		public View GetInfoWindow (Marker marker)
		{
			return null;
		}

		private async void SetContents (string deviceId, string pointId)
		{
			_info = await Mvx.Resolve<IApiService> ().GetInfo (deviceId, pointId);

			if(_marker != null && _marker.IsInfoWindowShown)
			{
				_marker.ShowInfoWindow();
			}
		}

		public View GetInfoContents (Marker marker)
		{
			NeedToRefresh = marker.Snippet != _info.GetId.ToString();
			if (NeedToRefresh) {
				SetContents (DeviceUtility.DeviceId, marker.Snippet);
			}

			_marker = marker;
			var customPopup = _layoutInflater.Inflate (Resource.Layout.CustomMarkerPopup, null);

			var nameTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_Name);
			if (nameTextView != null) {
				nameTextView.Text = string.Format ("Name: {0}", marker.Title);
			}

			var latLonTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_LatLonTextView);
			if (latLonTextView != null) {
				latLonTextView.Text = string.Format ("LatLon: {0}; {1}", marker.Position.Latitude, marker.Position.Longitude);
			}

			var ownerTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_OwnerTextView);
			if (ownerTextView != null) {
				ownerTextView.Text = string.Format ("Owner: {0}", _info.owner);
			}

			var allianceTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_AllianceTextView);
			if (allianceTextView != null) {
				allianceTextView.Text = string.Format ("Alliance: {0}", _info.alliance);
			}

			var fractionTextView = customPopup.FindViewById<TextView> (Resource.Id.customInfoWindow_FractionTextView);
			if (fractionTextView != null) {
				fractionTextView.Text = string.Format ("Fraction: {0}", _info.fraction);
			}

			return customPopup;
		}
	}
}

