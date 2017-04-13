using UnityEngine;

namespace BeardPhantom.UConsole
{
    public static class RichTextUtility
    {
        public static string MakeColored(this string s, Color c)
        {
            return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(c), s);
        }

        public static string MakeBold(this string s)
        {
            return string.Format("<b>{0}</b>", s);
        }

        public static string MakeItalic(this string s)
        {
            return string.Format("<i>{0}</i>", s);
        }
    }
}