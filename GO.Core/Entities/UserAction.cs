using System;

namespace GO.Core.Entities
{
   public class UserAction
   {
      public UserAction() { }

      public UserAction(DBUserAction item)
      {
         Id = item.Id;
         Type = item.Type;
         Number = item.Number;
         Description = item.Description;
         Date = item.Date;
      }

      public string Id { get; set; }

      public int Type { get; set; }
      public string Title { get; set; }
      public string Number { get; set; }
      public string Description { get; set; }
      public DateTime Date { get; set; }
   }
}