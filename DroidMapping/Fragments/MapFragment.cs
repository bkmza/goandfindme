
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
   public class MapFragment : Fragment
   {
      public const string ARG_PLANET_NUMBER = "planet_number";

      public MapFragment ()
      {
      }

      public static Android.App.Fragment NewInstance (int position)
      {
         Fragment fragment = new MapFragment ();
         Bundle args = new Bundle ();
         args.PutInt (MapFragment.ARG_PLANET_NUMBER, position);
         fragment.Arguments = args;
         return fragment;
      }

      public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
         Bundle savedInstanceState)
      {
         View rootView = inflater.Inflate (Resource.Layout.fragment_planet, container, false);
         var i = this.Arguments.GetInt (ARG_PLANET_NUMBER);
         var planet = this.Resources.GetStringArray (Resource.Array.drawer_items) [i];
         var imgId = this.Resources.GetIdentifier (planet.ToLower (),
            "drawable", this.Activity.PackageName);
         var iv = rootView.FindViewById<ImageView> (Resource.Id.image);
         iv.SetImageResource (imgId);
         this.Activity.Title = planet;
         return rootView;
      }
   }
}

