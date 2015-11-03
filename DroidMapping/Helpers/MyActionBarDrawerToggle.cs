
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
using Android.Support.V4.App;
using Android.Support.V4.Widget;

namespace DroidMapping
{
   internal class MyActionBarDrawerToggle : ActionBarDrawerToggle
   {
      DrawerActivityBase owner;

      public MyActionBarDrawerToggle (DrawerActivityBase activity, DrawerLayout layout, int imgRes, int openRes, int closeRes)
         : base (activity, layout, imgRes, openRes, closeRes)
      {
         owner = activity;
      }

      public override void OnDrawerClosed (View drawerView)
      {
         owner.ActionBar.Title = owner.Title;
         owner.InvalidateOptionsMenu ();
      }

      public override void OnDrawerOpened (View drawerView)
      {
         owner.ActionBar.Title = owner.mDrawerTitle;
         owner.InvalidateOptionsMenu ();
      }
   }

}

