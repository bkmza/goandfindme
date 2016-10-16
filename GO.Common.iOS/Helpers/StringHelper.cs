using System;
using Foundation;
using UIKit;

namespace GO.Common.iOS.Helpers
{
   [Flags]
   public enum StringFormatTag
   {
      Blue = 0,
      Link = 1,

      /// <summary>
      /// Reference type
      /// </summary>
      a = 2
   }

   public class StringFormatPosition
   {
      public string VisibleString { get; set; }

      public int Start { get; set; }

      public int End { get; set; }

      public int BlockLength { get { return End - Start; } }

      public StringFormatPosition(string str)
      {
         Start = -1;
         End = -1;
         VisibleString = str;
      }
   }

   public class StringHelper
   {
      public static StringFormatPosition GetFormatPosition(string str, StringFormatTag tag)
      {
         StringFormatPosition result = new StringFormatPosition(str);
         var sTag = String.Format("<{0}>", tag.ToString().ToLower());
         var eTag = String.Format("</{0}>", tag.ToString().ToLower());
         var startIndex = str.IndexOf(sTag);
         var endIndex = str.IndexOf(eTag);
         if (startIndex != -1 && endIndex != -1)
         {
            result = new StringFormatPosition(str.Replace(sTag, "").Replace(eTag, ""))
            {
               Start = startIndex,
               End = endIndex - sTag.Length
            };
         }
         return result;
      }

      public static NSMutableAttributedString GetAttributedString(string text, float lineSpacing = 1, UITextAlignment alignment = UITextAlignment.Left, NSUnderlineStyle underlineStyle = NSUnderlineStyle.None,
         nfloat? minLineHeight = null, nfloat? maxLineHeight = null, nfloat? letterSpacing = null, NSUnderlineStyle strikethroughStyle = NSUnderlineStyle.None)
      {
         if (!string.IsNullOrEmpty(text))
         {
            var paragraphStyle = new NSMutableParagraphStyle
            {
               LineSpacing = lineSpacing,
               Alignment = alignment
            };
            if (minLineHeight.HasValue)
            {
               paragraphStyle.MinimumLineHeight = minLineHeight.Value;
            }
            if (maxLineHeight.HasValue)
            {
               paragraphStyle.MaximumLineHeight = maxLineHeight.Value;
            }
            var textAttributes = new UIStringAttributes
            {
               ParagraphStyle = paragraphStyle,
               UnderlineStyle = underlineStyle,
               StrikethroughStyle = strikethroughStyle
            };

            var attributedString = new NSMutableAttributedString(text);
            attributedString.SetAttributes(textAttributes.Dictionary, new NSRange(0, text.Length));
            if (letterSpacing.HasValue)
            {
               attributedString.AddAttribute(UIStringAttributeKey.KerningAdjustment, NSObject.FromObject(letterSpacing.Value), new NSRange(0, text.Length));
            }

            return attributedString;
         }
         return null;
      }

      public static string SetLimitCharactersInString(string str, int limit)
      {
         var result = str;
         if (result.Length > limit)
            result = result.Substring(0, limit) + "...";
         else
            result = str;
         return result;
      }
   }
}

