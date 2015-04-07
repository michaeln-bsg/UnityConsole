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
            var parts = input.Split(' ');
            var alias = parts[0];
            var command = GetCFunc<CCommand>(alias);
            var args = parts.Skip(1).ToArray();

            //no commands found, check cvars
            if(command == null) {
                var cvar = GetCFunc<CVar>(alias);
                if(cvar == null) {
                    return UConsole.ColorizeErr(string.Format(@"COMMAND/CONVAR ""{0}"" NOT FOUND", alias));
                }
                var isReadonly = (cvar.flags & CVarFlags.ReadOnly) != 0;
                if(parts.Length < 2 || isReadonly) {
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

        public static T GetCFunc<T>(string alias) where T : CFunc {
            if(typeof(T) == typeof(CVar)) {
                return cvars.SingleOrDefault(c => c.alias == alias) as T;
            }
            else if(typeof(T) == typeof(CCommand)) {
                return ccmds.SingleOrDefault(c => c.alias == alias) as T;
            }
            else {
                throw new System.ArgumentException("Invalid type T!");
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