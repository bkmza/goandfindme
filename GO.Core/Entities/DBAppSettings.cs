using SQLite.Net.Attributes;

namespace GO.Core.Entities
{
   public class DBAppSettings : DBEntityBase
   {
      [Column("app_id")]
      public string AppId { get; set; }
   }
}