using System;
using UIKit;

namespace GoHunting.iOS.ViewControllers
{
   public class BaseViewController : UIViewController
   {
      public BaseViewController ()
      {
      }

      public override void ViewDidLoad ()
      {
         base.ViewDidLoad ();

         Initialize ();

         Build ();
      }

      public virtual void Initialize ()
      {
      }

      public virtual void Build ()
      {
      }
   }
}

