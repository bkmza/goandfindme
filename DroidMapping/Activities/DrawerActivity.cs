
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;

namespace DroidMapping
{
	[Activity (Label = "DrawerActivity")]
	public class DrawerActivity : BaseActivity
	{
		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			InitDrawer ();
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

