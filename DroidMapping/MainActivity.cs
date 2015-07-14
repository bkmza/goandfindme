using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using DroidMapping.Adapters;
using DroidMapping.Services;
using GoHunting.Core;
using GoHunting.Core.Data;
using GoHunting.Core.Helpers;
using GoHunting.Core.Services;
using Newtonsoft.Json;
using Android.Content;

namespace DroidMapping
{
	[Activity (Label = "Searching GPS...", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : BaseActivity
	{
		public LatLng SelectedPoint;

		IApiService _apiService;
		IToastService _toastService;

		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			_apiService = Mvx.Resolve<IApiService> ();
			_toastService = Mvx.Resolve<IToastService> ();

			InitDrawer ();

			// This event fires when the ServiceConnection lets the client (our App class) know that
			// the Service is connected. We use this event to start updating the UI with location
			// updates from the Service
//			App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
////				Log.Debug (logTag, "ServiceConnected Event Raised");
////				// notifies us of location changes from the system
////				App.Current.LocationService.LocationChanged += HandleLocationChanged;
////				//notifies us of user changes to the location provider (ie the user disables or enables GPS)
////				App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
////				App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
////				// notifies us of the changing status of a provider (ie GPS no longer available)
////				App.Current.LocationService.StatusChanged += HandleStatusChanged;
//			};
//			App.StartLocationService ();
		}

		private void InitDrawer ()
		{
			_drawer = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView> (Resource.Id.left_drawer);

			_drawerList.Adapter = new ArrayAdapter<string> (this,
				Resource.Layout.DrawerListItem, Resources.GetStringArray (Resource.Array.DrawerItemsArray));
			_drawerList.ItemClick += (sender, args) => SelectItem (args.Position);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			_drawerToggle = new MyActionBarDrawerToggle (this, _drawer,
				Resource.Drawable.ic_drawer_light,
				Resource.String.DrawerOpen,
				Resource.String.DrawerClose);

			_drawerToggle.DrawerClosed += delegate {
				InvalidateOptionsMenu ();
			};

			_drawerToggle.DrawerOpened += delegate {
				InvalidateOptionsMenu ();
			};

			_drawer.SetDrawerListener (_drawerToggle);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (_drawerToggle.OnOptionsItemSelected (item)) {
				return true;
			} else {
				return base.OnOptionsItemSelected (item);
			}
		}

		private void SelectItem (int position)
		{
			switch (position) {
			case 0:
				var intent = new Intent (this, typeof(MapActivity));
				StartActivity (intent);
				break;
			default:
				break;
			}

			_drawerList.SetItemChecked (position, true);
			_drawer.CloseDrawer (_drawerList);
		}
	}
}


