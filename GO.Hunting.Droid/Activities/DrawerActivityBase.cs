
using System;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Maps;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GO.Common.Droid;
using GO.Common.Droid.Activities;
using GO.Common.Droid.Fragments;
using GO.Hunting.Droid.Adapters;
using GO.Hunting.Droid.Fragments;
using static GO.Hunting.Droid.Adapters.DrawerAdapter;

namespace GO.Hunting.Droid
{
   [Activity]
   public class DrawerActivityBase : ActivityBase, OnItemClickListener, IOnMapReadyCallback
   {
      DrawerLayout mDrawerLayout;
      RecyclerView mDrawerList;
      ActionBarDrawerToggle mDrawerToggle;
      String[] drawerItems;

      public string mDrawerTitle;

      public void OnMapReady (GoogleMap googleMap)
      {
      }

      protected override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);

         // Preventing screen capturing
         //this.Window.SetFlags (WindowManagerFlags.Secure, WindowManagerFlags.Secure);

         SetContentView (Resource.Layout.activity_navigation_drawer);

         mDrawerTitle = this.Title;
         drawerItems = this.Resources.GetStringArray (Resource.Array.drawer_items);
         mDrawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
         mDrawerList = FindViewById<RecyclerView> (Resource.Id.left_drawer);

//         mDrawerLayout.SetDrawerShadow (Resource.Drawable.drawer_shadow, GravityCompat.Start);
         mDrawerList.HasFixedSize = true;
         mDrawerList.SetLayoutManager (new LinearLayoutManager (this));

         mDrawerList.SetAdapter (new DrawerAdapter (drawerItems, this));
         this.ActionBar.SetDisplayHomeAsUpEnabled (true);
         this.ActionBar.SetHomeButtonEnabled (true);

         mDrawerToggle = new MyActionBarDrawerToggle (this, mDrawerLayout,
                                                      Resource.Drawable.ic_drawer, 
            Resource.String.drawer_open, 
            Resource.String.drawer_close);

         mDrawerLayout.SetDrawerListener (mDrawerToggle);
         if (savedInstanceState == null) //first launch
            selectItem (0);
      }

      public override bool OnCreateOptionsMenu (IMenu menu)
      {
         MenuInflater.Inflate (Resource.Menu.navigation_drawer, menu);
         return true;
      }

      public override bool OnPrepareOptionsMenu (IMenu menu)
      {
         bool drawerOpen = mDrawerLayout.IsDrawerOpen (mDrawerList);
         menu.FindItem (Resource.Id.action_websearch).SetVisible (!drawerOpen);
         return base.OnPrepareOptionsMenu (menu);
      }

      public override bool OnOptionsItemSelected (IMenuItem item)
      {
         if (mDrawerToggle.OnOptionsItemSelected (item)) {
            return true;
         }

         switch (item.ItemId) {
         case Resource.Id.action_websearch:
            Intent intent = new Intent (Intent.ActionWebSearch);
            intent.PutExtra (SearchManager.Query, this.ActionBar.Title);
            if (intent.ResolveActivity (this.PackageManager) != null) {
               StartActivity (intent);
            } else {
               Toast.MakeText (this, Resource.String.app_not_available, ToastLength.Long).Show ();
            }
            return true;
         default:
            return base.OnOptionsItemSelected (item);
         }
      }

      public void OnClick (View view, int position)
      {
         selectItem (position);
      }

      private CMapFragment _cMapFrag;

      private void selectItem (int position)
      {
         if (this.ActionBar.TabCount > 0) {
            this.ActionBar.RemoveAllTabs ();
            this.ActionBar.NavigationMode = ActionBarNavigationMode.Standard;
         }

         FragmentBase fragment;
         switch (position) {
         case 0:
            if (_cMapFrag == null) {
               _cMapFrag = new CMapFragment ();
            }
            fragment = _cMapFrag;
            break;
         case 1:
            fragment = new ActionListFragment ();
            break;
         case 2:
            fragment = new SettingsFragment ();
            break;
         default:
            fragment = new CMapFragment ();
            break;
         }

         var fragmentManager = this.FragmentManager;
         var ft = fragmentManager.BeginTransaction ();
         ft.Replace (Resource.Id.content_frame, fragment);
         ft.Commit ();

         mDrawerLayout.CloseDrawer (mDrawerList);
      }

      //    private void SetTitle (string title)
      //    {
      //       this.Title = title;
      //       this.ActionBar.Title = title;
      //    }

      protected override void OnTitleChanged (Java.Lang.ICharSequence title, Android.Graphics.Color color)
      {
         ActionBar.Title = title.ToString ();
      }

      protected override void OnPostCreate (Bundle savedInstanceState)
      {
         base.OnPostCreate (savedInstanceState);
         mDrawerToggle.SyncState ();
      }

      public override void OnConfigurationChanged (Configuration newConfig)
      {
         base.OnConfigurationChanged (newConfig);
         mDrawerToggle.OnConfigurationChanged (newConfig);
      }

      protected override void OnDestroy ()
      {
         AppLocation.StopLocationService ();

         base.OnDestroy ();
      }
   }
}


