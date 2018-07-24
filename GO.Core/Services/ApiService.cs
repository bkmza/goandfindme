using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GO.Core.Data;
using GO.Core.Enums;
using GO.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GO.Core.Services
{
   public class ApiService : IApiService
   {
      private HttpClient GetClientSync()
      {
         HttpClient client = new HttpClient();
         client.Timeout = new TimeSpan(1, 0, 0);
         return client;
      }

      async Task<HttpClient> GetClient()
      {
         HttpClient client = new HttpClient();
         client.Timeout = new TimeSpan(1, 0, 0);
         return client;
      }

      public async Task<ErrorInfo> CheckUserAccess(string deviceId)
      {
         StopWatch.Start("ApiService.CheckUserAccess");

         HttpClient client = await GetClient();
         string result = await client.GetStringAsync(string.Format("{0}gofind2/markers.php?{1}", AppSettings.BaseHost, string.Format("dev_id={0}", deviceId)));

         ErrorInfo deserializedResult = JsonConvert.DeserializeObject<ErrorInfo>(result);
         if (deserializedResult == null)
         {
            deserializedResult = new ErrorInfo { status = "allowed", message = "all right" };
         }

         StopWatch.Stop("ApiService.CheckUserAccess");

         return deserializedResult;
      }

      public async Task<IEnumerable<Point>> GetAll(string deviceId)
      {
         StopWatch.Start("ApiService.GetAll");

         HttpClient client = await GetClient();

         string result = string.Empty;
         try
         {
            result = await client.GetStringAsync(string.Format("{0}gofind2/markers.php?{1}", AppSettings.BaseHost, string.Format("dev_id={0}", deviceId)));
         }
         catch (Exception ex)
         {
            Logger.Instance.Error(string.Format("ApiService.GetAll exception: {0}", ex.Message));
         }

         JObject parent = JObject.Parse(result);
         var points = parent.GetValue("points").First.First;
         IEnumerable<Point> deserializedResult = JsonConvert.DeserializeObject<IEnumerable<Point>>(points.ToString());

         StopWatch.Stop("ApiService.GetAll");

         return deserializedResult;
      }

      public PointInfo GetInfo(string deviceId, string pointId, string type)
      {
         StopWatch.Start(string.Format("ApiService.GetInfo for pointId: {0}", pointId));

         HttpClient client = GetClientSync();
         string parameters = string.Format("dev_id={0}&id={1}&type={2}", deviceId, pointId, type);
         var response = client.GetAsync(string.Format("{0}gofind2/marker.php?{1}", AppSettings.BaseHost, parameters)).Result;

         PointInfo deserializedResult = null;
         if (response.IsSuccessStatusCode)
         {
            var responseContent = response.Content;
            string responseString = responseContent.ReadAsStringAsync().Result;

            JObject parent = JObject.Parse(responseString);
            var point = parent.GetValue("point").First;
            deserializedResult = JsonConvert.DeserializeObject<PointInfo>(point.ToString());
         }

         StopWatch.Stop(string.Format("ApiService.GetInfo for pointId: {0}", pointId));

         return deserializedResult;
      }

      public async Task<PointInfo> GetInfoAsync(string deviceId, string pointId, string type)
      {
         StopWatch.Start(string.Format("ApiService.GetInfo for pointId: {0}", pointId));

         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&id={1}&type={2}", deviceId, pointId, type);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/marker.php?{1}", AppSettings.BaseHost, parameters));

         JObject parent = JObject.Parse(result);
         var point = parent.GetValue("point").First;
         var deserializedResult = JsonConvert.DeserializeObject<PointInfo>(point.ToString());

         StopWatch.Stop(string.Format("ApiService.GetInfo for pointId: {0}", pointId));

         return deserializedResult;
      }

      public async Task<ActionResponseBase> Conquer(string deviceId, string lat, string lon)
      {
         StopWatch.Start(string.Format("ApiService.Conquer for lat: {0} lon: {1}", lat, lon));

         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/takepoint.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<ActionResponseBase>(result);

         StopWatch.Start(string.Format("ApiService.Conquer for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }

      public async Task<ActionResponseBase> Quest(string deviceId, string lat, string lon)
      {
         StopWatch.Start(string.Format("ApiService.Quest for lat: {0} lon: {1}", lat, lon));

         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/takequest.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<ActionResponseBase>(result);

         StopWatch.Stop(string.Format("ApiService.Quest for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }

      public async Task<ActionResponseBase> Trap(string deviceId, string lat, string lon)
      {
         StopWatch.Start(string.Format("ApiService.Trap for lat: {0} lon: {1}", lat, lon));
         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/action_type3.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<ActionResponseBase>(result);
         StopWatch.Stop(string.Format("ApiService.Trap for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }

      public async Task<ActionResponseBase> Place(string deviceId, string lat, string lon)
      {
         StopWatch.Start(string.Format("ApiService.Place for lat: {0} lon: {1}", lat, lon));
         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/action_type4.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<ActionResponseBase>(result);
         StopWatch.Stop(string.Format("ApiService.Place for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }

      public async Task<ActionResponseBase> Raze(string deviceId, string lat, string lon)
      {
         StopWatch.Start(string.Format("ApiService.Raze for lat: {0} lon: {1}", lat, lon));
         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/action_type5.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<ActionResponseBase>(result);
         StopWatch.Stop(string.Format("ApiService.Raze for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }

      public async Task<ActionResponseBase> Attack(string deviceId, string lat, string lon)
      {
         StopWatch.Start(string.Format("ApiService.Attack for lat: {0} lon: {1}", lat, lon));
         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&lat={1}&lon={2}", deviceId, lat, lon);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/action_type6.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<ActionResponseBase>(result);
         StopWatch.Stop(string.Format("ApiService.Attack for lat: {0} lon: {1}", lat, lon));

         return deserializedResult;
      }

      public async Task<ActionResponseBase> Use(string deviceId, string lat, string lon, string objectCode)
      {
         StopWatch.Start(string.Format("ApiService.Use for lat: {0} lon: {1} objectCode: {2}", lat, lon, objectCode));
         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&lat={1}&lon={2}&object_code={3}", deviceId, lat, lon, objectCode);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/action_type7.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<ActionResponseBase>(result);
         StopWatch.Stop(string.Format("ApiService.Use for lat: {0} lon: {1} objectCode: {2}", lat, lon, objectCode));

         return deserializedResult;
      }
   }
}