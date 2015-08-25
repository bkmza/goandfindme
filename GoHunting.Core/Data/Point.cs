using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GoHunting.Core.Enums;
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
      public string icon;
      public string type;

      public string GetType {
         get {
            return type;
         }
      }

      public MapItemType GetMapItemType {
         get {
            MapItemType type;
            switch (this.GetType) {
            case "point":
               type = MapItemType.Point;
               break;
            case "quest":
               type = MapItemType.Quest;
               break;
            default:
               type = MapItemType.Point;
               break;
            }
            return type;
         }
      }

      public string GetIconName {
         get {
            return icon.Substring (0, icon.IndexOf (".")).Replace ("-", "_");
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

