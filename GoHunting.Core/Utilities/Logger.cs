using System;

namespace GoHunting.Core.Utilities
{
	public static class Logger
	{
		private static ILogger _instance;
		public static ILogger Instance
		{
			get
			{
				if (_instance == null) {
					_instance = new DefaultLogger ();
				}
				return _instance;
			}
			set {
				_instance = value;
			}
		}
	}

   public class DefaultLogger : ILogger
	{
      #region ILogger implementation

      public void Debug (string message)
      {
         throw new NotImplementedException ();
      }

      public void Warn (string message)
      {
         throw new NotImplementedException ();
      }

      public void Error (string message)
      {
         throw new NotImplementedException ();
      }

      #endregion


	}
}

