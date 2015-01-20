
using System.Collections.Generic;
namespace BSGTools.Console {
	public static class ArgParser {

		public static ParseResults Parse(ParseRules rules) {
			var results = new ParseResults();
			for(int i = 0;i < rules.argString.Length;i++) {
				var c = rules.argString[i];

				if(rules.flagChar.HasValue && c == rules.flagChar.Value) {
					var nextWhiteSpace = rules.argString.IndexOf(' ', i);
					if(nextWhiteSpace == -1)
						nextWhiteSpace = rules.argString.Length;
					var flagName = rules.argString.Substring(i, nextWhiteSpace - i).Trim();
					var nextFlag = rules.argString.IndexOf(rules.flagChar.Value, nextWhiteSpace);
					if(nextFlag == -1)
						nextFlag = rules.argString.Length;
					var arg = rules.argString.Substring(nextWhiteSpace, nextFlag - nextWhiteSpace).Trim();
					results.flagsArgs.Add(flagName, arg);

				}
			}
			return results;
		}

	}

	public struct ParseRules {
		public string argString;
		public char? flagChar;
		public bool requireQuotesForArgs;

		public ParseRules(string argString, bool requireQuotesForArgs) {
			this.argString = argString;
			this.flagChar = null;
			this.requireQuotesForArgs = requireQuotesForArgs;
		}

		public ParseRules(string argString, bool requireQuotesForArgs, char? flagChar) {
			this.argString = argString;
			this.flagChar = flagChar;
			this.requireQuotesForArgs = requireQuotesForArgs;
		}
	}

	public class ParseResults {
		public Dictionary<string, string> flagsArgs;

		public ParseResults() {
			flagsArgs = new Dictionary<string, string>();
		}
	}
}