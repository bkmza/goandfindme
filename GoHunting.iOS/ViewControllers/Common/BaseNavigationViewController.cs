using System;
using UIKit;

namespace GoHunting.iOS.ViewControllers
{
   public class BaseNavigationViewController : UINavigationController
   {
      public BaseNavigationViewController()
      {
      }

      public override void ViewDidLoad()
      {
         base.ViewDidLoad();

         Initialize();

         Build();
      }

      public virtual void Initialize()
      {
      }

      public virtual void Build()
      {
      }
   }
}

