
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
using GO.Common.Droid.Fragments;

namespace GO.Hunting.Droid.Fragments
{
   public class ActionListFragment : FragmentBase
   {
      View _view;

      public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         _view = inflater.Inflate (Resource.Layout.fragment_actions_list, container, false);

         Activity.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

         AddTab (Resources.GetString (Resource.String.ActionTabConquers), Resource.Mipmap.Icon, new ActionConquerFragment ());
         AddTab (Resources.GetString (Resource.String.ActionTabQuests), Resource.Mipmap.Icon, new ActionQuestFragment ());

         return _view;
      }

      void AddTab (string tabText, int iconResourceId, Fragment fragment)
      {
         var tab = Activity.ActionBar.NewTab ();
         tab.SetText (tabText);
//         tab.SetIcon (iconResourceId);

         tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
            e.FragmentTransaction.Replace (Resource.Id.content_frame, fragment);
         };

         Activity.ActionBar.AddTab (tab);
      }

      public override string FragmentTitle {
         get {
            return Resources.GetString (Resource.String.DrawerAction);
         }
      }
   }
}

