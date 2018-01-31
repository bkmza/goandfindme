using System.Net.Http;
using System.Threading.Tasks;
using GO.Core.Data;
using GO.Core.Utilities;
using ModernHttpClient;
using Newtonsoft.Json;

namespace GO.Core.Services
{
   public class LoginService : ILoginService
   {
      private async Task<HttpClient> GetClient()
      {
         HttpClient client = new HttpClient(new NativeMessageHandler());
         return client;
      }

      public async Task<RegisterStatus> Register(string name, string comment, string deviceId)
      {
         StopWatch.Start(string.Format("LoginService.Register for name: {0} comment: {1}", name, comment));

         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}&name={1}&message={2}", deviceId, name, comment);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/register.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<RegisterStatus>(result);

         StopWatch.Stop(string.Format("LoginService.Register for name: {0} comment: {1}", name, comment));

         return deserializedResult;
      }

      public async Task<RegisterStatus> CheckUserExists(string deviceId)
      {
         StopWatch.Start(string.Format("LoginService.CheckUserExists for deviceId: {0}", deviceId));

         HttpClient client = await GetClient();
         string parameters = string.Format("dev_id={0}", deviceId);
         string result = await client.GetStringAsync(string.Format("{0}gofind2/register.php?{1}", AppSettings.BaseHost, parameters));
         var deserializedResult = JsonConvert.DeserializeObject<RegisterStatus>(result);

         StopWatch.Stop(string.Format("LoginService.CheckUserExists for deviceId: {0}", deviceId));

         return deserializedResult;
      }
   }
}