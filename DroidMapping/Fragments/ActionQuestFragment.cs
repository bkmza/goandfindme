
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
using GoHunting.Core.Services;
using Cirrious.CrossCore;

namespace DroidMapping.Fragments
{
   public class ActionQuestFragment : ListFragment
   {
      IUserActionService _userActionService;

      public override void OnActivityCreated (Bundle savedInstanceState)
      {
         base.OnActivityCreated (savedInstanceState);

         _userActionService = Mvx.Resolve<IUserActionService> ();

         var userActions = _userActionService.GetQuests ();

         var items = userActions.Select (x => new Tuple<string,string> (x.Title, string.Format ("{0}, {1}", x.Date.ToString (), x.Description))).ToList ();

         this.ListAdapter = new SimpleListItem2_Adapter (this.Activity, items);
      }

      public override void OnListItemClick (ListView l, View v, int position, long id)
      {
         base.OnListItemClick (l, v, position, id);
      }
   }
}

