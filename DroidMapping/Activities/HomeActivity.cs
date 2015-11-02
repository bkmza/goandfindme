
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
   [Activity (Label = "FlyInMenu", MainLauncher = true)]
   public class HomeActivity : BaseActivity
   {
      protected override void OnCreate (Bundle bundle)
      {
         base.OnCreate (bundle);

         SetContentView (Resource.Layout.HomeLayout);

         var menu = FindViewById<FlyOutContainer> (Resource.Id.FlyOutContainer);
         var menuButton = FindViewById (Resource.Id.MenuButton);
         menuButton.Click += (sender, e) => {
            menu.AnimatedOpened = !menu.AnimatedOpened;
         };
      }
   }
}

