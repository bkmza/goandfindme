
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
   [Activity (MainLauncher = true)]
   public class HomeActivity : FlyOutActivityBase
   {
      protected override void OnCreate (Bundle bundle)
      {
         base.OnCreate (bundle);


      }

      protected override int LayoutId {
         get {
            return Resource.Layout.HomeLayout;
         }
      }
   }
}

