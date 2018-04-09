using System;
using Realms;

namespace GO.Core.Entities
{
   public class DBEntityBase : RealmObject
   {
      // TODO realm
      // add autoincrement
      // 
      [PrimaryKey, MapTo("id")]
      public string Id { get; private set; }

      public DBEntityBase()
      {
         Id = Guid.NewGuid().ToString();
      }
   }
}