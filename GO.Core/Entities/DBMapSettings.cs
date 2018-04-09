using Realms;

namespace GO.Core.Entities
{
   public class DBMapSettings : DBEntityBase
   {
      // Map update frequency in minutes
      [MapTo("update_frequency")]
      public int UpdateFrequency { get; set; }

      [MapTo("map_type")]
      public int MapType { get; set; }
   }
}