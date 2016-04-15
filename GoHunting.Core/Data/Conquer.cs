using System;
using System.Text;

namespace GoHunting.Core
{
	public class Conquer
	{
		//{"status":"fale","description":"\u041f\u043e\u043f\u044b\u0442\u043a\u0430 \u0437\u0430\u0445\u0432\u0430\u0442\u0430 \u0441\u043e\u044e\u0437\u043d\u043e\u0439 \u0442\u043e\u0447\u043a\u0438"}
		public Conquer ()
		{
		}

		public string status;
		public string description;
      public string title;
      public string number;

		public string GetDescription
		{
			get {
				byte[] utf8Bytes = Encoding.UTF8.GetBytes (description);
				return Encoding.UTF8.GetString (utf8Bytes, 0, utf8Bytes.Length);
			}
		}

		public bool IsSuccess
		{
			get {
				return status == "true";
			}
		}
	}
}

