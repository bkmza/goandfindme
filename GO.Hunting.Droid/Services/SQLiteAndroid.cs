using System;
using System.IO;
using GO.Core.Services;
using MvvmCross.Platform;
using SQLite.Net;
using SQLite.Net.Interop;

namespace GO.Hunting.Droid.Services
{
   public class SQLiteAndroid : ISQLite
   {
      public SQLiteConnection GetConnection ()
      {
         var sqliteFilename = "GOandFindMeDB.db3";

         string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
         var path = Path.Combine (documentsPath, sqliteFilename);
         var platform = Mvx.Resolve<ISQLitePlatform> ();
         var conn = new SQLiteConnection (platform, path);

         return conn;
      }
   }
}

