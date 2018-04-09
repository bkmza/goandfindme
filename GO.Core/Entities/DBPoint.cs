using Realms;

namespace GO.Core.Entities
{
   public class DBPoint : DBEntityBase
   {
      [MapTo("point_id")]
      public string PointId { get; set; }

      [MapTo("latitude")]
      public string latitude { get; set; }
      [MapTo("longitude")]
      public string longitude { get; set; }

      [MapTo("content")]
      public string content { get; set; }
      [MapTo("color")]
      public string color { get; set; }
      [MapTo("icon")]
      public string icon { get; set; }
      [MapTo("type")]
      public string type { get; set; }
   }
}