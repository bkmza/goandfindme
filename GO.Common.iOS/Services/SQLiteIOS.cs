using System.IO;
using GoHunting.Core.Services;
using MvvmCross.Platform;
using SQLite.Net;
using SQLite.Net.Interop;

namespace GO.Common.iOS.Services
{
   public class SQLiteIOS : ISQLite
   {
      public SQLiteConnection GetConnection()
      {
         var sqliteFilename = "goandfindmedb.db3";

         string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
         var path = Path.Combine(documentsPath, sqliteFilename);
         var platform = Mvx.Resolve<ISQLitePlatform>();
         var conn = new SQLiteConnection(platform, path);

         return conn;
      }
   }
}
