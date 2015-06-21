using System;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using ModernHttpClient;
using GoHunting.Core.Data;
using Newtonsoft.Json.Linq;

namespace GoHunting.Core.Services
{
	public class ApiService : IApiService
	{
		private async Task<HttpClient> GetClient ()
		{
			HttpClient client = new HttpClient (new NativeMessageHandler ());
			return client;
		}

		public async Task<IEnumerable<Point>> GetAll (string deviceId)
		{
			HttpClient client = await GetClient ();
			string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/markers.php?", string.Format ("dev_id={0}", deviceId)));

			JObject parent = JObject.Parse (result);
			var points = parent.GetValue ("points").First.First;
			return JsonConvert.DeserializeObject<IEnumerable<Point>> (points.ToString ());
		}

		public async Task<PointInfo> GetInfo (string deviceId, string pointId)
		{
			HttpClient client = await GetClient ();
			string parameters = string.Format ("dev_id={0}&id={1}", deviceId, pointId);
			string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/marker.php?{0}", parameters));

			JObject parent = JObject.Parse (result);
			var point = parent.GetValue ("point").First;
			return JsonConvert.DeserializeObject<PointInfo> (point.ToString ());
		}
	}
}

