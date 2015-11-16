
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

namespace DroidMapping.Fragments
{
   public class SettingsFragment : FragmentBase
   {
      View _view;
      Switch _isLoggingQuestsSwitch;

      public override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);
      }

      public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         _view = inflater.Inflate (Resource.Layout.fragment_settings, container, false);
         _isLoggingQuestsSwitch = _view.FindViewById<Switch> (Resource.Id.isLoggingQuestsSwitch);

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
      }

      private void UnregisterHandler()
      {
         _isLoggingQuestsSwitch.CheckedChange -= IsLoggingQuestsSwitchHandler;
      }

      private void IsLoggingQuestsSwitchHandler(object sender, CompoundButton.CheckedChangeEventArgs e)
      {
         var result = e.IsChecked;
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

