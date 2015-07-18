using System;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using GoHunting.Core.Data;
using GoHunting.Core.Utilities;

namespace GoHunting.Core.Services
{
   public class LoginService : ILoginService
   {
      private async Task<HttpClient> GetClient ()
      {
         HttpClient client = new HttpClient (new NativeMessageHandler ());
         return client;
      }

      //http://gollars.letsmake.ru/gofind2/register.php?dev_id=111222333444555666&name=Royber&message=qwerty
      public async Task<RegisterStatus> Register (string name, string comment, string deviceId)
      {
         StopWatch.Start (string.Format ("LoginService.Register for name: {0} comment: {1}", name, comment));

         HttpClient client = await GetClient ();
         string parameters = string.Format ("dev_id={0}&name={1}&message={2}", deviceId, name, comment);
         string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/register.php?{0}", parameters));
         var deserializedResult = JsonConvert.DeserializeObject<RegisterStatus> (result);

         StopWatch.Stop (string.Format ("LoginService.Register for name: {0} comment: {1}", name, comment));

         return deserializedResult;
      }

      public async Task<RegisterStatus> CheckUserExists(string deviceId)
      {
         StopWatch.Start (string.Format ("LoginService.CheckUserExists for deviceId: {0}", deviceId));

         HttpClient client = await GetClient ();
         string parameters = string.Format ("dev_id={0}", deviceId);
         string result = await client.GetStringAsync (string.Format ("http://gollars.letsmake.ru/gofind2/register.php?{0}", parameters));
         var deserializedResult = JsonConvert.DeserializeObject<RegisterStatus> (result);

         StopWatch.Stop (string.Format ("LoginService.CheckUserExists for deviceId: {0}", deviceId));

         return deserializedResult;
      }
   }
}

