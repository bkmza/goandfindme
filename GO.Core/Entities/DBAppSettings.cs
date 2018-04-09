using Realms;

namespace GO.Core.Entities
{
   public class DBAppSettings : DBEntityBase
   {
      [MapTo("app_id")]
      public string AppId { get; set; }
   }
}