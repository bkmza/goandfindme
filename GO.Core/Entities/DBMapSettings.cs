using System;
using Realms;

namespace GO.Core.Entities
{
   public class DBMapSettings : RealmObject
   {
      [PrimaryKey, MapTo("id")]
      public string Id { get; private set; }

      public DBMapSettings()
      {
         Id = Guid.NewGuid().ToString();
      }

      // Map update frequency in minutes
      [MapTo("update_frequency")]
      public int UpdateFrequency { get; set; }

      [MapTo("map_type")]
      public int MapType { get; set; }
   }
}