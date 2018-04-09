using System;
using Realms;

namespace GO.Core.Entities
{
   public class DBPoint : RealmObject
   {
      [PrimaryKey, MapTo("id")]
      public string Id { get; private set; }

      public DBPoint()
      {
         Id = Guid.NewGuid().ToString();
      }

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