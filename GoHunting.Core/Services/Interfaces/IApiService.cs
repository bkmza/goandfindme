using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoHunting.Core.Data;

namespace GoHunting.Core.Services
{
	public interface IApiService
	{
		Task<IEnumerable<Point>> GetAll (string deviceId);

		Task<PointInfo> GetInfo (string deviceId, string pointId);
	}
}

