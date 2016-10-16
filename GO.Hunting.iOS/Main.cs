using UIKit;

namespace GO.Hunting.iOS
{
   public class Application
   {
      // This is the main entry point of the application.
      static void Main(string[] args)
      {
         try
         {
            UIApplication.Main(args, null, "AppDelegate");
         }
         catch (Exception exception)
         {
            if (exception != null)
            {
               //Logger.Instance.Error(exception.Message);
               //DataManagerBase dataManager;
               //if (Mvx.TryResolve(out dataManager))
               //{
               //dataManager.TrackException(exception, true);
               //}
            }
            throw exception;
         }
      }
   }
}
