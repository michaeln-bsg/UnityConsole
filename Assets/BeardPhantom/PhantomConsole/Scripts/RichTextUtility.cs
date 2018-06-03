using UnityEngine;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Various utilities for formatting strings for rich text
    /// </summary>
    public static class RichTextUtility
    {
        /// <summary>
        /// Formats input string to be colored
        /// </summary>
        public static string MakeColored(this string s, Color c)
        {
            return string.Format(
                "<color=#{0}>{1}</color>",
                ColorUtility.ToHtmlStringRGBA(c),
                s);
        }

        /// <summary>
        /// Formats input string to be bold
        /// </summary>
        public static string MakeBold(this string s)
        {
            return string.Format("<b>{0}</b>", s);
        }

        /// <summary>
        /// Formats input string to be italicized
        /// </summary>
        public static string MakeItalic(this string s)
        {
            return string.Format("<i>{0}</i>", s);
        }
    }
}