
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
   public class ActionQuestFragment : ListFragment
   {
      public override void OnActivityCreated (Bundle savedInstanceState)
      {
         base.OnActivityCreated (savedInstanceState);

         var items = new[] { 
            new Tuple<string,string> ("1", "2")
         };
         this.ListAdapter = new SimpleListItem2_Adapter (this.Activity, items);
      }

      public override void OnListItemClick (ListView l, View v, int position, long id)
      {
         base.OnListItemClick (l, v, position, id);
      }
   }
}

