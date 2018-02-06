using GO.Core.Utilities;

namespace GO.Paranoia.Droid.Utilities
{
	public class AndroidLogger : ILogger
	{
		const string TAG = "GOH";

		public void Debug (string message)
		{
			Android.Util.Log.Debug (TAG, message);
		}

		public void Warn (string message)
		{
			Android.Util.Log.Warn (TAG, message);
		}

		public void Error (string message)
		{
			Android.Util.Log.Error (TAG, message);
		}
	}
}

