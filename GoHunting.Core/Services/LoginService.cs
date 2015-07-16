using System;
using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;

namespace GoHunting.Core.Services
{
	public class LoginService : ILoginService
	{
		private async Task<HttpClient> GetClient ()
		{
			HttpClient client = new HttpClient (new NativeMessageHandler ());
			return client;
		}

		public async Task<bool> Login (string name, string comment, string deviceId)
		{
			HttpClient client = await GetClient ();
			//string parameters = string.Format ("dev_id={0}&id={1}", deviceId, pointId);
			//string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/marker.php?{0}", parameters));

			await Task.Delay (5000);

			return true;
		}
	}
}

