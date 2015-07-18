using System;

namespace GoHunting.Core.Services
{
   public interface IStopWatchWrapper
   {
      void Start(string key);

      void Stop(string key);
   }
}

