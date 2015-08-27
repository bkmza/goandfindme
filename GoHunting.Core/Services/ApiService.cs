using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GoHunting.Core.Data;
using GoHunting.Core.Utilities;
using ModernHttpClient;
using Newtonsoft.Json;
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

      public async Task<ErrorInfo> CheckUserAccess (string deviceId)
      {
         StopWatch.Start ("ApiService.CheckUserAccess");

         HttpClient client = await GetClient ();
         string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/markers.php?{0}", string.Format ("dev_id={0}", deviceId)));

         ErrorInfo deserializedResult = JsonConvert.DeserializeObject<ErrorInfo> (result);
         if (deserializedResult == null) {
            deserializedResult = new ErrorInfo { status = "allowed", message = "all right" };
         }

         StopWatch.Stop ("ApiService.CheckUserAccess");

         return deserializedResult;
      }

      public async Task<IEnumerable<Point>> GetAll (string deviceId)
      {
         StopWatch.Start ("ApiService.GetAll");

         HttpClient client = await GetClient ();

         string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/markers.php?{0}", string.Format ("dev_id={0}", deviceId)));

         JObject parent = JObject.Parse (result);
         var points = parent.GetValue ("points").First.First;
         IEnumerable<Point> deserializedResult = JsonConvert.DeserializeObject<IEnumerable<Point>> (points.ToString ());

         StopWatch.Stop ("ApiService.GetAll");

         return deserializedResult;
      }

      public async Task<PointInfo> GetInfo (string deviceId, string pointId, string type)
      {
         StopWatch.Start (string.Format ("ApiService.GetInfo for pointId: {0}", pointId));

         HttpClient client = await GetClient ();
         string parameters = string.Format ("dev_id={0}&id={1}&type={2}", deviceId, pointId, type);
         string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/marker.php?{0}", parameters));

         JObject parent = JObject.Parse (result);
         var point = parent.GetValue ("point").First;
         var deserializedResult = JsonConvert.DeserializeObject<PointInfo> (point.ToString ());

         StopWatch.Stop (string.Format ("ApiService.GetInfo for pointId: {0}", pointId));

         return deserializedResult;
      }

      public async Task<Conquer> Conquer (string deviceId, string lat, string lon)
      {
         StopWatch.Start (string.Format ("ApiService.Conquer for lat: {0} lon: {1}", lat, lon));

         HttpClient client = await GetClient ();
         string parameters = string.Format ("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/takepoint.php?{0}", parameters));
         var deserializedResult = JsonConvert.DeserializeObject<Conquer> (result);

         StopWatch.Start (string.Format ("ApiService.Conquer for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }

      public async Task<Conquer> Quest (string deviceId, string lat, string lon)
      {
         StopWatch.Start (string.Format ("ApiService.Quest for lat: {0} lon: {1}", lat, lon));

         HttpClient client = await GetClient ();
         string parameters = string.Format ("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/takequest.php?{0}", parameters));
         var deserializedResult = JsonConvert.DeserializeObject<Conquer> (result);

         StopWatch.Stop (string.Format ("ApiService.Quest for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }
   }
}

