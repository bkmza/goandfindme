using System;

namespace GoHunting.Core.Services
{
   public interface IMapSettingsService
   {
      int GetUpdateFrequency();

      void SetUpdateFrequency(int value);

      int GetMapType();

      void SetMapType(int mapType);
   }
}

