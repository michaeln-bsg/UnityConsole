
using System.Collections.Generic;
namespace Rubycone.UConsole {
	public static class ArgParser {
		public static string flagChars = "/-";

		public static Dictionary<string, string> Parse(string argStr) {
			var results = new Dictionary<string, string>();
			for(int i = 0;i < argStr.Length;i++) {
				var c = argStr[i];

				if(IsFlagChar(c)) {
					var nextWhiteSpace = argStr.IndexOf(' ', i);
					if(nextWhiteSpace == -1)
						nextWhiteSpace = argStr.Length;
					var flagName = argStr.Substring(i, nextWhiteSpace - i).Trim();
					foreach(var fc in flagChars)
						flagName = flagName.Replace(fc.ToString(), "");
					var nextFlag = argStr.IndexOf(flagChars, nextWhiteSpace);
					if(nextFlag == -1)
						nextFlag = argStr.Length;
					var arg = argStr.Substring(nextWhiteSpace, nextFlag - nextWhiteSpace).Trim();
					if(arg.Length > 0) {
						if(arg[0] == '"')
							arg = arg.Remove(0, 1);
						if(arg[arg.Length - 1] == '"')
							arg = arg.Remove(arg.Length - 1);
					}
					else
						arg = null;
					results.Add(flagName, arg);
				}
			}
			return results;
		}

		public static bool IsFlagChar(char c) {
			return flagChars.IndexOf('c') != -1;
		}

	}
}