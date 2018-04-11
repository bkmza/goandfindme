using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using GO.Common.Droid.Services;
using GO.Common.Droid.Utilities;
using GO.Core;
using GO.Core.Services;
using GO.Core.Utilities;
using MvvmCross.Platform;

namespace GO.Common.Droid.Activities
{
   [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
   public class ActivityBase : Activity
   {
      private ConnectivityManager _connectivityManager;
      private IToastService _toastService;
      private ProgressDialog _progressDialog;
      protected IAnalyticsService AnalyticsService;
      protected IAppSettingsService AppSettingsService;

      protected override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

         _toastService = Mvx.Resolve<IToastService>();
         AnalyticsService = Mvx.Resolve<IAnalyticsService>();

         _connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
      }

      protected void InitializeTypes()
      {
         Logger.Instance = new AndroidLogger();
         Mvx.RegisterSingleton<IDBService>(new DBService());

         Mvx.RegisterType<IToastService, ToastService>();
         AppSettingsService = Mvx.Resolve<IAppSettingsService>();
         Mvx.RegisterType<IAnalyticsService>(() => new AnalyticsService(AppSettingsService));
         Mvx.RegisterType<IUserActionService, UserActionService>();
         Mvx.RegisterType<IMapSettingsService, MapSettingsService>();
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
            if (_isLoading)
            {
               _progressDialog = ProgressDialog.Show(this, string.Empty, Resources.GetString(Resource.String.Wait), true, false);
            }
            else
            {
               if (_progressDialog != null && _progressDialog.IsShowing)
               {
                  _progressDialog.Dismiss();
               }
            }
         }
      }

      private bool _isLoading;

      protected void ShowAlert(string message)
      {
         RunOnUiThread(() =>
         {
            var alert = new AlertDialog.Builder(this);
            alert.SetMessage(message);
            alert.SetCancelable(true);
            alert.Show();
         });
      }
   }
}