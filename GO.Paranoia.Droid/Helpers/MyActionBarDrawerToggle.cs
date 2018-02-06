using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Views;

namespace GO.Paranoia.Droid
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

