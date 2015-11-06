using System;
using SQLite.Net.Attributes;

namespace GoHunting.Core.Entities
{
   public class DBAction : DBEntityBase
   {
      [Column ("type")]
      public int Type { get; set; }

      [Column ("title")]
      public string Title { get; set; }

      [Column ("description")]
      public string Description { get; set; }

      [Column ("date")]
      public DateTime Date { get; set; }
   }
}

