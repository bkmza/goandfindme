using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Cirrious.CrossCore;
using GoHunting.Core.Services;

namespace DroidMapping.Fragments
{
   public class SettingsFragment : FragmentBase
   {
      View _view;
      Switch _isLoggingQuestsSwitch;
      EditText _updateMapFrequencyEditText;

      IMapSettingsService _mapSettingsService;

      public SettingsFragment()
      {
         _mapSettingsService = Mvx.Resolve<IMapSettingsService> ();
      }

      public override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);
      }

      public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         _view = inflater.Inflate (Resource.Layout.fragment_settings, container, false);
         _isLoggingQuestsSwitch = _view.FindViewById<Switch> (Resource.Id.isLoggingQuestsSwitch);
         _updateMapFrequencyEditText = _view.FindViewById<EditText>(Resource.Id.mapUpdateFrequency);

         _updateMapFrequencyEditText.Text = _mapSettingsService.GetUpdateFrequency ().ToString ();

         RegisterHandlers ();

         return _view;
      }

      public override string FragmentTitle {
         get {
            return Resources.GetString (Resource.String.DrawerSettings);
         }
      }

      private void RegisterHandlers()
      {
         _isLoggingQuestsSwitch.CheckedChange += IsLoggingQuestsSwitchHandler;
         _updateMapFrequencyEditText.KeyPress += UpdateMapFrequencyEditTextHandler;
      }

      private void UnregisterHandler()
      {
         _isLoggingQuestsSwitch.CheckedChange -= IsLoggingQuestsSwitchHandler;
         _updateMapFrequencyEditText.KeyPress -= UpdateMapFrequencyEditTextHandler;
      }

      private void IsLoggingQuestsSwitchHandler(object sender, CompoundButton.CheckedChangeEventArgs e)
      {
         var result = e.IsChecked;
      }

      private void UpdateMapFrequencyEditTextHandler(object sender, View.KeyEventArgs e)
      {
         e.Handled = false;
         if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter) {

            InputMethodManager inputManager = (InputMethodManager) this.Activity.GetSystemService(Context.InputMethodService);
            inputManager.HideSoftInputFromWindow(this.Activity.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

            int updateFrequency;
            if (int.TryParse (_updateMapFrequencyEditText.Text, out updateFrequency) && updateFrequency > 0) {
               _mapSettingsService.SetUpdateFrequency (updateFrequency);
               _updateMapFrequencyEditText.ClearFocus ();
            } else {
               ShowAlert ("Please, enter a one or two-digit integer value greater than zero. Settings not updated.");
            }

            e.Handled = true;
         }
      }

      public override void OnStop ()
      {
         UnregisterHandler ();
         base.OnStop ();
      }

      public override void OnDestroy ()
      {
         UnregisterHandler ();
         base.OnDestroy ();
      }
   }
}

