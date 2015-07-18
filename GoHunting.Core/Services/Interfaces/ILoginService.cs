using System;
using System.Threading.Tasks;
using GoHunting.Core.Data;

namespace GoHunting.Core.Services
{
   public interface ILoginService
   {
      Task<RegisterStatus> Register (string name, string comment, string deviceId);

      Task<RegisterStatus> CheckUserExists (string deviceId);
   }
}

