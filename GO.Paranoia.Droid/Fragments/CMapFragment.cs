using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.OS;
using Android.Views;
using GO.Common.Droid.Fragments;
using GO.Core.Enums;
using GO.Paranoia.Droid.Adapters;

namespace GO.Paranoia.Droid.Fragments
{
   public class CMapFragment : CMapFragmentBase, IOnMapReadyCallback
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

      public void OnMapReady(GoogleMap googleMap)
      {
         GMap = googleMap;
         switch (MapType)
         {
            case MapType.Normal:
               GMap.MapType = GoogleMap.MapTypeNormal;
               break;
            case MapType.Hybrid:
               GMap.MapType = GoogleMap.MapTypeHybrid;
               break;
            case MapType.Terrain:
               GMap.MapType = GoogleMap.MapTypeTerrain;
               break;
            default:
               GMap.MapType = GoogleMap.MapTypeNormal;
               break;
         }
         GMap.MyLocationEnabled = true;
         GMap.UiSettings.MyLocationButtonEnabled = true;
         GMap.UiSettings.ZoomControlsEnabled = true;
         GMap.SetInfoWindowAdapter(new CustomInfoWindowAdapter(MapLayoutInflater, ToastService, AppSettingsService));

         CameraUpdate update = CameraUpdateFactory.NewLatLngZoom(Location_Minsk, 11);
         GMap.MoveCamera(update);

         Task.Run(async () =>
         {
            await UpdateMarkersAsync();
         });
      }

      public override string FragmentTitle => Resources.GetString(Resource.String.DrawerMap);
   }
}