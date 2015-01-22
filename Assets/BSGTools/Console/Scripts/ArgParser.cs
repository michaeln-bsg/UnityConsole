
using System.Collections.Generic;
namespace BSGTools.Console {
	public static class ArgParser {

		public static ParseResults Parse(ParseRules rules) {
			var results = new ParseResults();
			for(int i = 0;i < rules.argString.Length;i++) {
				var c = rules.argString[i];

				if(c == rules.flagChar) {
					var nextWhiteSpace = rules.argString.IndexOf(' ', i);
					if(nextWhiteSpace == -1)
						nextWhiteSpace = rules.argString.Length;
					var flagName = rules.argString.Substring(i, nextWhiteSpace - i).Trim().Replace("/", "");
					var nextFlag = rules.argString.IndexOf(rules.flagChar, nextWhiteSpace);
					if(nextFlag == -1)
						nextFlag = rules.argString.Length;
					var arg = rules.argString.Substring(nextWhiteSpace, nextFlag - nextWhiteSpace).Trim();
					if(arg.Length > 0) {
						if(arg[0] == '"')
							arg = arg.Remove(0, 1);
						if(arg[arg.Length - 1] == '"')
							arg = arg.Remove(arg.Length - 1);
					}
					else
						arg = null;
					results.flagsArgs.Add(flagName, arg);
				}
			}
			return results;
		}

	}

	public struct ParseRules {
		public string argString;
		public char flagChar;

		public ParseRules(string argString) : this(argString, '/') { }
		public ParseRules(string[] argparts) : this(string.Join(" ", argparts)) { }
		public ParseRules(string[] argparts, char flagChar) : this(string.Join(" ", argparts), flagChar) { }

		public ParseRules(string argString, char flagChar) {
			this.argString = argString;
			this.flagChar = flagChar;
		}
	}

	public class ParseResults {
		public Dictionary<string, string> flagsArgs;

		public ParseResults() {
			flagsArgs = new Dictionary<string, string>();
		}
	}
}