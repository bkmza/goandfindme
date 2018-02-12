using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using GO.Core.Entities;
using GO.Core.Services;
using GO.Paranoia.Droid.Adapters;
using MvvmCross.Platform;

namespace GO.Paranoia.Droid.Fragments
{
   public class ActionQuestFragment : ListFragment
   {
      IUserActionService _userActionService;
      List<UserAction> _userActions;

      public override void OnActivityCreated(Bundle savedInstanceState)
      {
         base.OnActivityCreated(savedInstanceState);

         _userActionService = Mvx.Resolve<IUserActionService>();

         _userActions = _userActionService.GetQuests().OrderByDescending(x => x.Date).ToList();

         var items = _userActions
            .Select(x => new Tuple<string, string>(string.Format("{0} - {1}", x.Number, x.Title), string.Format("{0}, {1}", x.Date.ToString(), x.Description)))
            .ToList();

         this.ListAdapter = new SimpleListItem2_Adapter(this.Activity, items);
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
   }
}