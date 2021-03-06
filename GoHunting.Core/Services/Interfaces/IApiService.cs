﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GoHunting.Core.Data;

namespace GoHunting.Core.Services
{
	public interface IApiService
	{
      Task<ErrorInfo> CheckUserAccess (string deviceId);

      Task<IEnumerable<Point>> GetAll (string deviceId);

      Task<PointInfo> GetInfo (string deviceId, string pointId, string type);

		Task<Conquer> Conquer (string deviceId, string lat, string lon);

		Task<Conquer> Quest (string deviceId, string lat, string lon);
	}
}

