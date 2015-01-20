using System;
using System.Collections.Generic;
using System.Linq;

namespace Wenzil.Console {
	public delegate string CommandCallback(params string[] args);

	/// <summary>
	/// Use RegisterCommand() to register your own commands.
	/// </summary>
	public static class CommandDatabase {
		private static HashSet<Command> _database = new HashSet<Command>();
		public static HashSet<Command> database { get { return _database; } }

		public static void RegisterCommand(CommandCallback callback, params string[] aliases) {
			RegisterCommand(null, null, callback, aliases);
		}

		public static void RegisterCommand(string description, string usage, CommandCallback callback, params string[] aliases) {
			database.Add(new Command(description, usage, callback, aliases));
		}

		public static string ExecuteCommand(string alias, params string[] args) {
			var command = GetCommand(alias);
			return (command != null) ? command.callback(args) : @"<color=red>Command """ + alias + @""" not found.</color>"; ;
		}

		public static Command GetCommand(string alias) {
			return database.FirstOrDefault(c => c.aliases.Contains(alias, StringComparer.CurrentCultureIgnoreCase));
		}

		public class Command {
			public string[] aliases { get; private set; }
			public string description { get; private set; }
			public string usage { get; private set; }
			public CommandCallback callback { get; private set; }

			public Command(string description, string usage, CommandCallback callback, string[] aliases) {
				this.description = description;
				this.usage = usage;
				this.callback = callback;
				this.aliases = aliases;
			}
		}
	}
}