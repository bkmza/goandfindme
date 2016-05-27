using System;

namespace GoHunting.Core.Services
{
   public interface IToastService
   {
      void ShowMessage (string message);

      void ShowMessageLongPeriod(string message);
   }
}

