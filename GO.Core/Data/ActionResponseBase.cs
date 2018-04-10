using System.Text;

namespace GO.Core.Data
{
   public class ActionResponseBase
   {
      //{
      //   "status": "true",
      //   "title": "",
      //   "number": "",
      //   "description": "Тестовое действие. Вы совершили действие \"Ловушка\""
      //}
      // or
      //{"status":"fale","description":"\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0437\u0430\u0445\u0432\u0430\u0442\u0430 \u0441\u043e\u044e\u0437\u043d\u043e\u0439 \u0442\u043e\u0447\u043a\u0438"}
      public ActionResponseBase()
      {
      }

      public string status;
      public string title;
      public string number;
      public string description;

      public bool IsSuccess => status == "true";

      public string GetDescription
      {
         get
         {
            if (description == null)
            {
               return description;
            }

            byte[] utf8Bytes = Encoding.UTF8.GetBytes(description);
            return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
         }
      }
   }
}