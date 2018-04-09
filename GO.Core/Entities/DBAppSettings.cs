using System;
using Realms;

namespace GO.Core.Entities
{
   public class DBAppSettings : RealmObject
   {
      [PrimaryKey, MapTo("id")]
      public string Id { get; private set; }

      public DBAppSettings()
      {
         Id = Guid.NewGuid().ToString();
      }

      [MapTo("app_id")]
      public string AppId { get; set; }
   }
}