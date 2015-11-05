using System;
using SQLite.Net.Attributes;

namespace GoHunting.Core.Entities
{
   public class DBEntityBase
   {
      [PrimaryKey, AutoIncrement, Column ("id")]
      public int Id { get; set; }
   }
}

