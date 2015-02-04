
using UnityEngine;
public delegate void OnConsoleLog(string line);

namespace BSGTools.Console {
	public class UConsole {

		public static OnConsoleLog OnConsoleLog;

		public static readonly Color ORANGE = new Color(255, 127, 0);

		public static void Log(string line) {
			if(OnConsoleLog != null)
				OnConsoleLog(line);
		}

		public static void Log(string line, Color32 color) {
			if(OnConsoleLog != null)
				OnConsoleLog(Colorize(line, color));
		}

		public static string Colorize(string str, Color32 color) {
			return string.Format("<color={0}>{1}</color>", ColorToHex(color), str);
		}

		public static string ColorizeErr(string str) {
			return Colorize(str, Color.red);
		}

		public static string ColorizeWarn(string str) {
			return Colorize(str, ORANGE);
		}

		static string ColorToHex(Color32 color) {
			return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		}
	}
}