using SQLite.Net.Attributes;

namespace GO.Core.Entities
{
   public class DBMapSettings : DBEntityBase
   {
      // Map update frequency in minutes
      [Column("update_frequency")]
      public int UpdateFrequency { get; set; }

      [Column("map_type")]
      public int MapType { get; set; }
   }
}