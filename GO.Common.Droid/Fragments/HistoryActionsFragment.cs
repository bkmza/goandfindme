using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using GO.Common.Droid.Adapters;
using GO.Core.Entities;
using GO.Core.Enums;
using GO.Core.Services;
using MvvmCross.Platform;

namespace GO.Common.Droid.Fragments
{
   public class HistoryActionsFragment : ListFragment
   {
      public HistoryActionsFragment() { }

      List<UserAction> _userActions;

      private IMenu _menu;
      private MenuInflater _menuInflater;

      public override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

         SetHasOptionsMenu(true);
         BuildFilteredList(null);
      }

      public override void OnListItemClick(ListView l, View v, int position, long id)
      {
         base.OnListItemClick(l, v, position, id);

         // TODO
         // move it to helper
         // Description can contains URL to the web
         // if yes - open webview
         foreach (Match match in Regex.Matches(_userActions[(int)id].Description, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&:/~\+#]*[\w\-\@?^=%&/~\+#])?"))
         {
            var uri = Android.Net.Uri.Parse(match.Value);
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
            return;
         }
      }

      private void BuildFilteredList(ActionType? type)
      {
         _userActions = Mvx.Resolve<IUserActionService>().GetActions(type).OrderByDescending(x => x.Date.DateTime).ToList();
         var items = _userActions
            .Select(x => new Tuple<string, string>(string.Format("{0} - {1}", x.Number, x.Title), string.Format("{0}, {1}", x.Date.DateTime.ToString(), x.Description)))
            .ToList();

         ListAdapter = new SimpleListItem2_Adapter(Activity, items);
      }

      public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
      {
         _menu = menu;
         _menuInflater = inflater;

         _menu.Add(0, 0, 0, Resource.String.ConquerMenuTitle);
         _menu.Add(0, 1, 1, Resource.String.QuestMenuTitle);
         _menu.Add(0, 2, 2, Resource.String.TrapMenuTitle);
         _menu.Add(0, 3, 3, Resource.String.PlaceMenuTitle);
         _menu.Add(0, 4, 4, Resource.String.RazeMenuTitle);
         _menu.Add(0, 5, 5, Resource.String.AttackMenuTitle);
         _menu.Add(0, 6, 6, Resource.String.AllObjectsMenuTitle);
      }

      public override bool OnOptionsItemSelected(IMenuItem item)
      {
         switch (item.ItemId)
         {
            case 0:
               BuildFilteredList(ActionType.Point);
               return true;
            case 1:
               BuildFilteredList(ActionType.Quest);
               return true;
            case 2:
               BuildFilteredList(ActionType.Trap);
               return true;
            case 3:
               BuildFilteredList(ActionType.Place);
               return true;
            case 4:
               BuildFilteredList(ActionType.Raze);
               return true;
            case 5:
               BuildFilteredList(ActionType.Attack);
               return true;
            case 6:
               BuildFilteredList(null);
               return true;
            default:
               return base.OnOptionsItemSelected(item);
         }
      }
   }
}