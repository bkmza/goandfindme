﻿using System;

namespace GoHunting.Core.Utilities
{
	public interface ILogger
	{
		void Debug (string message);

		void Warn (string message);

		void Error (string message);
	}
}

