using SQLite.Net;

namespace GO.Core.Services
{
   public interface ISQLite
   {
      SQLiteConnection GetConnection();
   }
}