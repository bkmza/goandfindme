namespace GO.Core.Services
{
   public interface IToastService
   {
      void ShowMessage(string message);

      void ShowMessageLongPeriod(string message);
   }
}