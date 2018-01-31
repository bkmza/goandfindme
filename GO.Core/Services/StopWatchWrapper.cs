using System.Collections.Generic;
using System.Diagnostics;
using GO.Core.Utilities;

namespace GO.Core.Services
{
   public class StopWatchWrapper : IStopWatchWrapper
   {
      static Dictionary<string, Stopwatch> _watches = new Dictionary<string, Stopwatch>();

      public void Start(string key)
      {
         if (_watches == null)
         {
            _watches = new Dictionary<string, Stopwatch>();
         }
         _watches[key] = new Stopwatch();
         _watches[key].Start();
      }

      public void Stop(string key)
      {
         if (_watches.ContainsKey(key))
         {
            _watches[key].Stop();
            var message = string.Format(key + " {0} ms", _watches[key].ElapsedMilliseconds);
            Logger.Instance.Debug(message);
            _watches.Remove(key);
         }
      }
   }
}