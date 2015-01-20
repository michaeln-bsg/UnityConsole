
public delegate void OnConsoleLog(string line);

namespace Wenzil.Console {
	/// <summary>
	/// Use Console.Log() anywhere in your code. The Console prefab will display the output.
	/// </summary>
	public static class Console {
		public static OnConsoleLog OnConsoleLog;

		public static void Log(string line) {
			if(OnConsoleLog != null)
				OnConsoleLog(line);
		}

		public static void Log(string line, string color) {
			if(OnConsoleLog != null)
				OnConsoleLog(Colorize(line, color));
		}

		public static string ExecuteCommand(string command, params string[] args) {
			return CommandDatabase.ExecuteCommand(command, args);
		}

		public static string Colorize(string str, string color) {
			return string.Format("<color={0}>{1}</color>", color, str);
		}
	}
}