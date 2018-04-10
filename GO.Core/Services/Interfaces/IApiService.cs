using System.Collections.Generic;
using System.Threading.Tasks;
using GO.Core.Data;

namespace GO.Core.Services
{
   public interface IApiService
   {
      Task<ErrorInfo> CheckUserAccess(string deviceId);
      Task<IEnumerable<Point>> GetAll(string deviceId);
      PointInfo GetInfo(string deviceId, string pointId, string type);
      Task<PointInfo> GetInfoAsync(string deviceId, string pointId, string type);

      Task<ActionResponseBase> Conquer(string deviceId, string lat, string lon);
      Task<ActionResponseBase> Quest(string deviceId, string lat, string lon);
      Task<ActionResponseBase> Trap(string deviceId, string lat, string lon);
      Task<ActionResponseBase> Place(string deviceId, string lat, string lon);
      Task<ActionResponseBase> Raze(string deviceId, string lat, string lon);
      Task<ActionResponseBase> Attack(string deviceId, string lat, string lon);
   }
}