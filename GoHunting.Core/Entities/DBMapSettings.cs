using System;
using SQLite.Net.Attributes;

namespace GoHunting.Core.Entities
{
   public class DBMapSettings : DBEntityBase
   {
      // Map update frequency in minutes
      [Column ("update_frequency")]
      public int UpdateFrequency { get; set; }
   }
}

