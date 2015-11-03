
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

namespace DroidMapping
{
   [Activity]
   public class FlyOutActivityBase : ActivityBase
   {
      protected override void OnCreate (Bundle bundle)
      {
         base.OnCreate (bundle);

         SetContentView (LayoutId);

         var menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
         var menuButton = FindViewById (Resource.Id.MenuButton);
         menuButton.Click += (sender, e) => {
            menu.AnimatedOpened = !menu.AnimatedOpened;
         };

         var button1 = FindViewById (Resource.Id.linearLayout1);
         button1.Click += (sender, e) => {
            var intent = new Intent (this, typeof(HomeActivity));
            intent.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
            StartActivity (intent);
         };

         var button2 = FindViewById (Resource.Id.linearLayout2);
         button2.Click += (sender, e) => {

            menu.AnimatedOpened = !menu.AnimatedOpened;

            FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
            PointListFragment frag = new PointListFragment();
            fragmentTx.Replace(Resource.Id.map, frag);
            fragmentTx.AddToBackStack(null);
            fragmentTx.Commit();
         };
      }

      protected virtual int LayoutId
      {
         get {
            return 0;
         }
      }
   }
}

