
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
   public class ActionListFragment : FragmentBase
   {
      View _view;

      public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         _view = inflater.Inflate (Resource.Layout.fragment_actions_list, container, false);

         this.Activity.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

         AddTab (Resources.GetString (Resource.String.ActionTabConquers), Resource.Drawable.Icon, new ActionConquerFragment ());
         AddTab (Resources.GetString (Resource.String.ActionTabQuests), Resource.Drawable.Icon, new ActionQuestFragment ());

         return _view;
      }

      void AddTab (string tabText, int iconResourceId, Fragment fragment)
      {
         var tab = this.Activity.ActionBar.NewTab ();
         tab.SetText (tabText);
//         tab.SetIcon (iconResourceId);

         tab.TabSelected += delegate(object sender, ActionBar.TabEventArgs e) {
            e.FragmentTransaction.Replace (Resource.Id.content_frame, fragment);
         };

         this.Activity.ActionBar.AddTab (tab);
      }

      public override string Titile {
         get {
            return "List";
         }
      }
   }
}

