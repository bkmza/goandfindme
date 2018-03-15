using Android.App;
using Android.Net;
using Android.OS;
using GO.Core.Services;
using MvvmCross.Platform;

namespace GO.Common.Droid.Fragments
{
   public class FragmentBase : Fragment
   {
      private ConnectivityManager _connectivityManager;
      private IToastService _toastService;
      private ProgressDialog _progressDialog;
      protected IAnalyticsService AnalyticsService;

      public override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

         _toastService = Mvx.Resolve<IToastService>();
         AnalyticsService = Mvx.Resolve<IAnalyticsService>();

         _connectivityManager = (ConnectivityManager)Activity.GetSystemService(Activity.ConnectivityService);

         this.Activity.Title = this.FragmentTitle;
      }

      public bool CheckInternetConnection()
      {
         var activeConnection = _connectivityManager.ActiveNetworkInfo;
         if ((activeConnection != null) && activeConnection.IsConnected)
         {
            return true;
         }
         else
         {
            _toastService.ShowMessage(this.Resources.GetString(Resource.String.NeedInternetConnect));
            return false;
         }
      }

      protected bool IsLoading
      {
         get => _isLoading;
         set
         {
            _isLoading = value;
            Activity.RunOnUiThread(() =>
            {
               if (_isLoading)
               {
                  _progressDialog = ProgressDialog.Show(this.Activity, string.Empty, Resources.GetString(Resource.String.Wait), true, false);
               }
               else
               {
                  if (_progressDialog != null && _progressDialog.IsShowing)
                  {

                     _progressDialog.Dismiss();
                  }
               }
            });
         }
      }

      private bool _isLoading;

      protected void ShowAlert(string message)
      {
         this.Activity.RunOnUiThread(() =>
         {
            var alert = new AlertDialog.Builder(this.Activity);
            alert.SetMessage(message);
            alert.SetCancelable(true);
            alert.Show();
         });
      }

      public virtual string FragmentTitle => string.Empty;
   }
}