using System;
using SQLite.Net.Attributes;

namespace GoHunting.Core.Entities
{
   public class DBPoint : DBEntityBase
   {
      [Column ("point_id")]
      public string PointId { get; set; }

      [Column ("latitude")]
      public string latitude { get; set; }
      [Column ("longitude")]
      public string longitude { get; set; }

      [Column ("content")]
      public string content { get; set; }
      [Column ("color")]
      public string color { get; set; }
      [Column ("icon")]
      public string icon { get; set; }
      [Column ("type")]
      public string type { get; set; }
   }
}

