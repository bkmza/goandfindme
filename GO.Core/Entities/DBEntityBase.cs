using SQLite.Net.Attributes;

namespace GO.Core.Entities
{
   public class DBEntityBase
   {
      [PrimaryKey, AutoIncrement, Column("id")]
      public int Id { get; set; }
   }
}