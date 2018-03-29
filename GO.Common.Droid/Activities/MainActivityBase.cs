using Android.OS;
using Android.Support.V4.App;
using Plugin.CurrentActivity;

namespace GO.Common.Droid.Activities
{
   public class MainActivityBase : ActivityBase, ActivityCompat.IOnRequestPermissionsResultCallback
   {
      public MainActivityBase()
      {
      }

      protected override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

         CrossCurrentActivity.Current.Activity = this;
      }
   }
}