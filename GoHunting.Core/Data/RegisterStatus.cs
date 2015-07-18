using System;
using System.Text;
using GoHunting.Core.Enums;

namespace GoHunting.Core.Data
{
   public class RegisterStatus
   {
      public string status { get; set; }

      public string description { get; set; }

      public int GetStatus {
         get {
            int statusValue = (int)UserStatus.Error;
            int.TryParse (status, out statusValue);
            return statusValue;
         }
      }

      public string GetDescription {
         get {
            byte[] utf8Bytes = Encoding.UTF8.GetBytes (description);
            return Encoding.UTF8.GetString (utf8Bytes, 0, utf8Bytes.Length);
         }
      }
   }
}

