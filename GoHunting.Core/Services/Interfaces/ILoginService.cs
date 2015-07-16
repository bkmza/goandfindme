using System;
using System.Threading.Tasks;

namespace GoHunting.Core.Services
{
	public interface ILoginService
	{
		Task<bool> Login (string name, string comment, string deviceId);
	}
}

