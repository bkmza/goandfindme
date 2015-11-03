﻿
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
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Content.Res;

namespace DroidMapping
{
   [Activity]
   public class DrawerActivityBase : ActivityBase, DrawerAdapter.OnItemClickListener
   {
      private DrawerLayout mDrawerLayout;
      private RecyclerView mDrawerList;
      private ActionBarDrawerToggle mDrawerToggle;

      public string mDrawerTitle;
      private String[] mPlanetTitles;

      protected override void OnCreate (Bundle savedInstanceState)
      {

         base.OnCreate (savedInstanceState);
         SetContentView (Resource.Layout.activity_navigation_drawer);

         mDrawerTitle = this.Title;
         mPlanetTitles = this.Resources.GetStringArray (Resource.Array.drawer_items);
         mDrawerLayout = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
         mDrawerList = FindViewById<RecyclerView> (Resource.Id.left_drawer);

//         mDrawerLayout.SetDrawerShadow (Resource.Drawable.drawer_shadow, GravityCompat.Start);
         mDrawerList.HasFixedSize = true;
         mDrawerList.SetLayoutManager (new LinearLayoutManager (this));

         mDrawerList.SetAdapter (new DrawerAdapter (mPlanetTitles, this));
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
         // Inflate the menu; this adds items to the action bar if it is present.
         this.MenuInflater.Inflate (Resource.Menu.navigation_drawer, menu);
         return true;
      }

      /* Called whenever we call invalidateOptionsMenu() */
      public override bool OnPrepareOptionsMenu (IMenu menu)
      {
         // If the nav drawer is open, hide action items related to the content view
         bool drawerOpen = mDrawerLayout.IsDrawerOpen (mDrawerList);
         menu.FindItem (Resource.Id.action_websearch).SetVisible (!drawerOpen);
         return base.OnPrepareOptionsMenu (menu);
      }

      public override bool OnOptionsItemSelected (IMenuItem item)
      {
         // The action bar home/up action should open or close the drawer.
         // ActionBarDrawerToggle will take care of this.
         if (mDrawerToggle.OnOptionsItemSelected (item)) {
            return true;
         }
         // Handle action buttons
         switch (item.ItemId) {
         case Resource.Id.action_websearch:
            // create intent to perform web search for this planet
            Intent intent = new Intent (Intent.ActionWebSearch);
            intent.PutExtra (SearchManager.Query, this.ActionBar.Title);
            // catch event that there's no activity to handle intent
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

      private void selectItem (int position)
      {
         Android.App.Fragment fragment;
         switch (position) {
         case 1:
            fragment = MapFragment.NewInstance (position);
            break;
         case 2:
            fragment = PointListFragment.NewInstance (position);
            break;
         default:
            fragment = MapFragment.NewInstance (position);
            break;
         }

         var fragmentManager = this.FragmentManager;
         var ft = fragmentManager.BeginTransaction ();
         ft.Replace (Resource.Id.content_frame, fragment);
         ft.Commit ();

         // update selected item title, then close the drawer
         Title = mPlanetTitles [position];
         mDrawerLayout.CloseDrawer (mDrawerList);
      }

      //    private void SetTitle (string title)
      //    {
      //       this.Title = title;
      //       this.ActionBar.Title = title;
      //    }

      protected override void OnTitleChanged (Java.Lang.ICharSequence title, Android.Graphics.Color color)
      {
         //base.OnTitleChanged (title, color);
         this.ActionBar.Title = title.ToString ();
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
   }
}

