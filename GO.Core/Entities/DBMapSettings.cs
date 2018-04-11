using Realms;

namespace GO.Core.Entities
{
   public class DBMapSettings : RealmObject
   {
      [PrimaryKey, MapTo("id")]
      public string Id { get; set; }

      public DBMapSettings() { }

      // Map update frequency in minutes
      [MapTo("update_frequency")]
      public int UpdateFrequency { get; set; }

      [MapTo("map_type")]
      public int MapType { get; set; }
   }
}