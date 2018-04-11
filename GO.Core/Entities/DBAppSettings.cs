using Realms;

namespace GO.Core.Entities
{
   public class DBAppSettings : RealmObject
   {
      [PrimaryKey, MapTo("id")]
      public string Id { get; set; }

      public DBAppSettings() { }

      [MapTo("app_id")]
      public string AppId { get; set; }
   }
}