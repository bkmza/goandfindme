using System;
using UIKit;

namespace GO.Common.iOS.ViewControllers
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

