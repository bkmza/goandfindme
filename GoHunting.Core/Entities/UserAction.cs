using System;

namespace GoHunting.Core.Entities
{
   public class UserAction
   {
      public UserAction ()
      {
      }

      public int Id { get; set; }

      public int Type { get; set; }
      public string Title { get; set; }
      public string Number { get; set; }
      public string Description { get; set; }
      public DateTime Date { get; set; }
   }
}

