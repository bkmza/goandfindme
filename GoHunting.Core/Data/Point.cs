using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace GoHunting.Core.Data
{
	public class Point : PointBase
	{
		public Point ()
		{
		}

		public string content;
		public string color;

		public float GetColorHue {
			get {
				float hue;
				switch (color) {
				case "azure":
					hue = 210; //HueAzure
					break;
				case "blue":
					hue = 240; //HueBlue
					break;
				case "cyan":
					hue = 180; //HueCyan
					break;
				case "green":
					hue = 120; //HueGreen
					break;
				case "magenta":
					hue = 300; //HueMagenta
					break;
				case "orange":
					hue = 30; //HueOrange
					break;
				case "red":
					hue = 0; //HueRed
					break;
				case "rose":
					hue = 330; //HueRose
					break;
				case "violet":
					hue = 270; //HueViolet
					break;
				case "yellow":
					hue = 60; //HueYellow
					break;
				default:
					hue = 210;
					break;
				}
				return hue;
			}
		}

		public string GetContent {
			get {
				byte[] utf8Bytes = Encoding.UTF8.GetBytes (content);
				return Encoding.UTF8.GetString (utf8Bytes, 0, utf8Bytes.Length);
			}
		}
	}
}

