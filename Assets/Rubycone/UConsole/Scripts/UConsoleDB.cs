using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Rubycone.UConsole {

    /// <summary>
    /// Use RegisterCommand() to register your own commands.
    /// </summary>
    public static class UConsoleDB {
        public static HashSet<CCommand> ccmds = new HashSet<CCommand>();
        public static HashSet<CVar> cvars     = new HashSet<CVar>();

        public static string CONVAR_LOAD_PATH = string.Empty;

        internal static void RegisterCFunc(CFunc c) {
            if(c is CVar) {
                cvars.Add(c as CVar);
            }
            else if(c is CCommand) {
                ccmds.Add(c as CCommand);
            }
            else {
                throw new InvalidCastException();
            }
        }

        public static string ExecuteFromInput(string input) {
            var alias = input.Split(' ')[0];
            var args = ParseArgs(input, alias);

            var command = GetCFunc<CCommand>(alias);

            //no commands found, check cvars
            if(command == null) {
                var cvar = GetCFunc<CVar>(alias);
                if(cvar == null) {
                    return UConsole.ColorizeErr(string.Format(@"COMMAND/CONVAR ""{0}"" NOT FOUND", alias));
                }
                var isReadonly = (cvar.flags & CVarFlags.ReadOnly) != 0;
                if(args[0] == string.Empty || isReadonly) {
                    var desc = (isReadonly) ? UConsole.ColorizeWarn(cvar.description) : cvar.description;
                    return string.Format("{0}\n\t\tvalue: {1}", desc, UConsole.Colorize(cvar.sVal, Color.green));
                }
                else {
                    cvar.SetValue(args[0]);
                }
            }
            else {
                var result = command.Execute(args);
                switch(result) {
                    case CCommandExecResults.None:
                        break;
                    case CCommandExecResults.Success:
                        break;
                    case CCommandExecResults.GenericFailure:
                        return command.usage;
                    case CCommandExecResults.MissingArgs:
                        return command.usage;
                    case CCommandExecResults.NoSelectedGameObj:
                        return CCommand.NO_OBJ_SELECTED_ERR;
                    case CCommandExecResults.NoSelectedComponent:
                        return CCommand.NO_COMPONENT_SELECTED_ERR;
                }
            }
            return null;
        }

        private static string[] ParseArgs(string input, string alias) {
            input = input.Remove(input.IndexOf(alias), alias.Length); //remove alias
            bool inQuotes = false, inArg = false, shouldAdd = false;

            var sb = new StringBuilder();
            var argList = new List<string>();

            for(int i = 0; i < input.Length; i++) {
                char c = input[i];
                bool isEscaped = input[Mathf.Clamp(i - 1, 0, input.Length)] == '\\';

                //reached beginning quote
                if(!inQuotes && !isEscaped && c == '"') {
                    inQuotes = true;
                    inArg = true;
                }
                //reached end quote
                else if(inQuotes && !isEscaped && c == '"') {
                    inQuotes = false;
                    inArg = false;
                    shouldAdd = true;
                }
                else if(!inQuotes && inArg && c == ' ') {
                    shouldAdd = true;
                }
                else if(!(c == '\\' && !isEscaped)) { //if an unescaped backslash
                    sb.Append(c);
                    inArg = true;
                    if(i == input.Length - 1) {
                        shouldAdd = true;
                    }
                }

                if(shouldAdd) {
                    argList.Add(sb.ToString().Trim());
                    sb.Remove(0, sb.Length);
                    shouldAdd = false;
                }
            }

            if(argList.Count == 0) {
                argList.Add(string.Empty);
            }
            return argList.ToArray();
        }

        public static T GetCFunc<T>(string alias) where T : CFunc {
            var t = typeof(T);
            var tCVar = typeof(CVar);
            var tCCommand = typeof(CCommand);

            if(t == tCVar) {
                return cvars.SingleOrDefault(c => c.alias == alias) as T;
            }
            else if(t == tCCommand) {
                return ccmds.SingleOrDefault(c => c.alias == alias) as T;
            }
            else {
                throw new System.ArgumentException("Invalid type: " + t.FullName);
            }
        }
    }

    public enum CommandType {
        NoArgs,
        SingleArgOptional,
        SingleArgRequired,
        MultiArgs,
    }

    public class CommandArgs {
        public Dictionary<string, string> multiArgs { get; private set; }
        public string singleArg { get; private set; }

        public CommandArgs(string singleArg) {
            this.singleArg = singleArg;
            this.multiArgs = null;
        }

        public CommandArgs(Dictionary<string, string> multiArgs) {
            this.singleArg = null;
            this.multiArgs = multiArgs;
        }
    }
}