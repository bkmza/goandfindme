using System;
using SQLite.Net;

namespace GoHunting.Core.Services
{
   public interface ISQLite
   {
      SQLiteConnection GetConnection ();
   }
}

