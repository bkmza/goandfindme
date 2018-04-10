using System;
using Android.Views;

namespace GO.Common.Droid.Fragments
{
   public class CMapFragmentBase : FragmentBase
   {
      protected IMenu MapMenu;
      protected MenuInflater MapMenuInflater;

      public CMapFragmentBase()
      {
      }

      public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
      {
         MapMenu = menu;
         MapMenuInflater = inflater;

         MapMenu.Add(0, 0, 0, Resource.String.ConquerMenuTitle);
         MapMenu.Add(0, 1, 1, Resource.String.QuestMenuTitle);
         MapMenu.Add(0, 2, 2, Resource.String.TrapMenuTitle);
         MapMenu.Add(0, 3, 3, Resource.String.PlaceMenuTitle);
         MapMenu.Add(0, 4, 4, Resource.String.RazeMenuTitle);
         MapMenu.Add(0, 5, 5, Resource.String.AttackMenuTitle);
         MapMenu.Add(0, 6, 6, Resource.String.RefreshMenuTitle);
         MapMenu.Add(0, 7, 7, Resource.String.OnlyPointsMenuTitle);
         MapMenu.Add(0, 8, 8, Resource.String.OnlyQuestsMenuTitle);
         MapMenu.Add(0, 9, 9, Resource.String.AllObjectsMenuTitle);
         MapMenu.Add(0, 10, 10, Resource.String.LogoutMenuTitle);
      }
   }
}