using System;
using System.Globalization;

namespace GoHunting.Core.Data
{
	public abstract class PointBase
	{
		public string id;
		public string latitude;
		public string longitude;

		public PointBase()
		{
			id = "0";
			Created = DateTime.Now;
		}

		public int GetId {
			get {
				return int.Parse (id);
			}
		}

		public double GetLatitude {
			get {
				return double.Parse (latitude, CultureInfo.InvariantCulture);
			}
		}

		public double GetLongitude {
			get {
				return double.Parse (longitude, CultureInfo.InvariantCulture);
			}
		}

		public bool IsValid ()
		{
			bool isValid = true;
			double coord;
			if (string.IsNullOrEmpty (latitude) && double.TryParse (latitude, out coord))
				isValid = false;

			if (string.IsNullOrEmpty (longitude) && double.TryParse (longitude, out coord))
				isValid = false;

			if (string.IsNullOrEmpty (id))
				isValid = false;

			return isValid;
		}

		public DateTime? Created { get; set; }
		public DateTime? Updated { get; set; }
	}
}

