using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Cirrious.CrossCore;
using GoHunting.Core.Services;
using GoHunting.Core.Enums;

namespace DroidMapping.Fragments
{
   public class SettingsFragment : FragmentBase
   {
      View _view;
      Switch _isLoggingQuestsSwitch;
      EditText _updateMapFrequencyEditText;
      RadioGroup _mapTypesRadioGroup;

      private RadioButton _mapNormalType;
      private RadioButton _mapHybridType;
      private RadioButton _mapTerrainType;

      IMapSettingsService _mapSettingsService;

      public SettingsFragment ()
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
         _updateMapFrequencyEditText = _view.FindViewById<EditText> (Resource.Id.mapUpdateFrequency);
         _mapTypesRadioGroup = _view.FindViewById<RadioGroup> (Resource.Id.mapTypeRadioGroup);
         _mapNormalType = _view.FindViewById<RadioButton> (Resource.Id.mapNormalTypeButton);
         _mapHybridType = _view.FindViewById<RadioButton> (Resource.Id.mapHybridTypeButton);
         _mapTerrainType = _view.FindViewById<RadioButton> (Resource.Id.mapTerrainTypeButton);

         _updateMapFrequencyEditText.Text = _mapSettingsService.GetUpdateFrequency ().ToString ();

         MapType mapType = (MapType)_mapSettingsService.GetMapType ();
         switch (mapType) {
         case MapType.Normal:
            _mapNormalType.Checked = true;
            break;
         case MapType.Hybrid:
            _mapHybridType.Checked = true;
            break;
         case MapType.Terrain:
            _mapTerrainType.Checked = true;
            break;
         default:
            _mapNormalType.Checked = true;
            break;
         }

         RegisterHandlers ();

         return _view;
      }

      public override string FragmentTitle {
         get {
            return Resources.GetString (Resource.String.DrawerSettings);
         }
      }

      private void RegisterHandlers ()
      {
         _isLoggingQuestsSwitch.CheckedChange += IsLoggingQuestsSwitchHandler;
         _updateMapFrequencyEditText.KeyPress += UpdateMapFrequencyEditTextHandler;
         _mapTypesRadioGroup.CheckedChange += MapTypeChanged;
      }

      private void UnregisterHandler ()
      {
         _isLoggingQuestsSwitch.CheckedChange -= IsLoggingQuestsSwitchHandler;
         _updateMapFrequencyEditText.KeyPress -= UpdateMapFrequencyEditTextHandler;
         _mapTypesRadioGroup.CheckedChange -= MapTypeChanged;
      }

      private void IsLoggingQuestsSwitchHandler (object sender, CompoundButton.CheckedChangeEventArgs e)
      {
         var result = e.IsChecked;
      }

      private void UpdateMapFrequencyEditTextHandler (object sender, View.KeyEventArgs e)
      {
         e.Handled = false;
         if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter) {

            InputMethodManager inputManager = (InputMethodManager)this.Activity.GetSystemService (Context.InputMethodService);
            inputManager.HideSoftInputFromWindow (this.Activity.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);

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

      private void MapTypeChanged (object sender, RadioGroup.CheckedChangeEventArgs e)
      {
         if (_mapNormalType.Checked) {
            _mapSettingsService.SetMapType ((int)MapType.Normal);
         } else if (_mapHybridType.Checked) {
            _mapSettingsService.SetMapType ((int)MapType.Hybrid);
         } else {
            _mapSettingsService.SetMapType ((int)MapType.Terrain);
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

