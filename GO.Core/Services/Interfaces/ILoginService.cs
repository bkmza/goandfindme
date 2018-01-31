using System.Threading.Tasks;
using GO.Core.Data;

namespace GO.Core.Services
{
   public interface ILoginService
   {
      Task<RegisterStatus> Register(string name, string comment, string deviceId);

      Task<RegisterStatus> CheckUserExists(string deviceId);
   }
}