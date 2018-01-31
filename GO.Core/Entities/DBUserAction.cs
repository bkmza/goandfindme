using System;
using SQLite.Net.Attributes;

namespace GO.Core.Entities
{
   public class DBUserAction : DBEntityBase
   {
      [Column("type")]
      public int Type { get; set; }

      [Column("title")]
      public string Title { get; set; }

      [Column("description")]
      public string Description { get; set; }

      [Column("date")]
      public DateTime Date { get; set; }

      [Column("number")]
      public string Number { get; set; }
   }
}