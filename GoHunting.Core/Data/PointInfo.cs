using System;

namespace GoHunting.Core.Data
{
	public class PointInfo : PointBase
	{
		//{"point":[{"latitude":"53.905987","longitude":"27.432782","name":"\u041c\u0430\u0442\u0435\u0440\u0438\u043a","id":"5","owner":"bengi","alliance":"GO","fraction":"INQ"}]}
		public PointInfo ()
		{
		}

		public string name;
		public string owner;
		public string alliance;
		public string fraction;
	}
}

