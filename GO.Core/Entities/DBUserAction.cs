using System;
using Realms;

namespace GO.Core.Entities
{
   public class DBUserAction : DBEntityBase
   {
      [MapTo("type")]
      public int Type { get; set; }

      [MapTo("title")]
      public string Title { get; set; }

      [MapTo("description")]
      public string Description { get; set; }

      [MapTo("date")]
      public DateTime Date { get; set; }

      [MapTo("number")]
      public string Number { get; set; }
   }
}