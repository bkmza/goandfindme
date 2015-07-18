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
		public void Debug (string message)
		{
		}
		public void Warn (string message)
		{
		}
		public void Error (string message)
		{
		}
	}
}

