
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using GO.Core.Services;
using MvvmCross.Platform;

namespace DroidMapping.Fragments
{
   public class ActionConquerFragment : ListFragment
   {
      IUserActionService _userActionService;

      public override void OnActivityCreated (Bundle savedInstanceState)
      {
         base.OnActivityCreated (savedInstanceState);

         _userActionService = Mvx.Resolve<IUserActionService> ();

         var userActions = _userActionService.GetConquers ();

         var items = userActions.OrderByDescending(x=>x.Date)
            .Select (x => new Tuple<string,string> (string.Format("{0} - {1}", x.Number, x.Title), string.Format ("{0}, {1}", x.Date, x.Description))).ToList ();

         this.ListAdapter = new SimpleListItem2_Adapter (this.Activity, items);
      }

      public override void OnListItemClick (ListView l, View v, int position, long id)
      {
         base.OnListItemClick (l, v, position, id);
      }
   }
}

