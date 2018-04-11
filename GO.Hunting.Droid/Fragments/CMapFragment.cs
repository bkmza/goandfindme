using Android.Gms.Maps;
using Android.OS;
using Android.Views;
using GO.Common.Droid.Fragments;

namespace GO.Hunting.Droid.Fragments
{
   public class CMapFragment : CMapFragmentBase
   {
      public CMapFragment() { }

      public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         if (container == null)
         {
            return null;
         }

         MapLayoutInflater = inflater;

         if (MapView == null)
         {
            MapView = inflater.Inflate(Resource.Layout.fragment_cmap, container, false);
         }

         MapFragmentView = MapView.FindViewById<MapView>(Resource.Id.map);
         if (MapFragmentView != null)
         {
            MapFragmentView.OnCreate(savedInstanceState);
            MapFragmentView.OnResume();
            MapFragmentView.GetMapAsync(this);
         }

         return MapView;
      }

      public override string FragmentTitle => Resources.GetString(Resource.String.DrawerMap);
   }
}