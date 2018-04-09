using System;
using Realms;

namespace GO.Core.Entities
{
   public class DBUserAction : RealmObject
   {
      [PrimaryKey, MapTo("id")]
      public string Id { get; private set; }

      public DBUserAction()
      {
         Id = Guid.NewGuid().ToString();
      }

      [MapTo("type")]
      public int Type { get; set; }

      [MapTo("title")]
      public string Title { get; set; }

      [MapTo("description")]
      public string Description { get; set; }

      [MapTo("date")]
      public DateTimeOffset Date { get; set; }

      [MapTo("number")]
      public string Number { get; set; }
   }
}