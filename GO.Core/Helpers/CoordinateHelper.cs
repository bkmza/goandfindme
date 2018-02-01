namespace GO.Core.Helpers
{
   public static class CoordinateHelper
   {
      public static string ProcessCoordinate(this double coord)
      {
         return coord.ToString().Replace(',', '.');
      }
   }
}