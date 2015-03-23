using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Rubycone.UConsole {

    public delegate StringBuilder CmdCallback(StringBuilder sb, CommandArgs args);
    public delegate void CVarValueChanged(string oldVal, CVar cvar);
    /// <summary>
    /// Use RegisterCommand() to register your own commands.
    /// </summary>
    public static class UConsoleDB {
        private static HashSet<CCMD> _ccmds    = new HashSet<CCMD>();
        private static HashSet<CVar> _cvars    = new HashSet<CVar>();
        private static StringBuilder OUTPUT_SB = new StringBuilder();
        internal static HashSet<CCMD> ccmds { get { return _ccmds; } }
        internal static HashSet<CVar> cvars { get { return _cvars; } }

        static readonly string INVALID_USAGE_ERR = UConsole.ColorizeErr("INVALID USAGE");
        static readonly string NO_OBJ_SELECTED_ERR = UConsole.ColorizeErr("COMMAND REQUIRES A SELECTED GAMEOBJECT");
        static readonly string NO_COMPONENT_SELECTED_ERR = UConsole.ColorizeErr("COMMAND REQUIRES A SELECTED COMPONENT");

        public static string CONVAR_LOAD_PATH = string.Empty;

        internal static void RegisterCommand(CCMD c) {
            ccmds.Add(c);
        }

        internal static void RegisterConVar(CVar c) {
            cvars.Add(c);
        }

        public static string ExecuteFromInput(string input) {
            OUTPUT_SB.Remove(0, OUTPUT_SB.Length);
            var parts = input.Split(' ');
            var alias = parts[0];
            var command = GetCommand(alias);
            var args = string.Join(" ", parts.Skip(1).ToArray());

            //no commands found, check cvars
            if(command == null) {
                var convar = GetCVar(alias);
                if(convar == null)
                    return UConsole.ColorizeErr(string.Format(@"COMMAND/CONVAR ""{0}"" NOT FOUND", alias));

                var isReadonly = (convar.flags & CVarFlags.ReadOnly) != 0;
                if(parts.Length < 2 || isReadonly) {
                    var desc = (isReadonly) ? UConsole.ColorizeWarn(convar.description) : convar.description;
                    return string.Format("{0}\n\t\tvalue: {1}", desc, UConsole.Colorize(convar.Get(), Color.green));
                }
                else {
                    convar.SetVal(args);
                    return null;
                }
            }


            try {
                if(command.requireSelectedGameObj && UConsole.selectedObj == null)
                    return NO_OBJ_SELECTED_ERR;
                if(command.requireSelectedComponent && UConsole.selectedComponent == null)
                    return NO_COMPONENT_SELECTED_ERR;

                switch(command.type) {
                    case CommandType.NoArgs:
                        return command.FireCallback(OUTPUT_SB, null).ToString().Trim();

                    case CommandType.SingleArgOptional:
                        if(args.Length != 0)
                            return command.FireCallback(OUTPUT_SB, new CommandArgs(args)).ToString();
                        return command.FireCallback(OUTPUT_SB, null).ToString();

                    case CommandType.SingleArgRequired:
                        if(args.Length == 0)
                            return UConsole.ColorizeErr("USAGE: " + command.usage);
                        return command.FireCallback(OUTPUT_SB, new CommandArgs(args)).ToString();

                    case CommandType.MultiArgs:
                        var cArgs = new CommandArgs(ArgParser.Parse(args));

                        //Check for missing required flags
                        if(command.requiredFlags.Except(cArgs.multiArgs.Keys).Any())
                            return UConsole.ColorizeErr("USAGE: " + command.usage);

                        return command.FireCallback(OUTPUT_SB, cArgs).ToString();

                    default:
                        return UConsole.ColorizeErr("COMMAND TYPE ERR");
                }
            }
            catch(Exception e) {
                return UConsole.ColorizeErr(e.ToString());
            }
        }

        public static CVar GetCVar(string alias) {
            return cvars.FirstOrDefault(c => c.aliases.Contains(alias, StringComparer.CurrentCultureIgnoreCase));
        }

        public static CCMD GetCommand(string alias) {
            return ccmds.FirstOrDefault(c => c.aliases.Contains(alias, StringComparer.CurrentCultureIgnoreCase));
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

    public class CCMD {
        internal string[] aliases { get; private set; }
        internal event CmdCallback CmdCallback;
        internal bool requireSelectedGameObj { get; private set; }
        internal bool requireSelectedComponent { get; private set; }
        internal string description { get; private set; }
        internal string usage { get; private set; }
        internal CommandType type { get; private set; }
        internal HashSet<string> requiredFlags;

        public CCMD(params string[] aliases) {
            this.aliases = aliases;
            this.description = "nodesc";
            this.usage = "nousage";
            this.type = CommandType.NoArgs;
            requiredFlags = new HashSet<string>();
            this.requireSelectedComponent = this.requireSelectedGameObj = false;
            UConsoleDB.RegisterCommand(this);
        }

        public CCMD SetRequireSelectedComponent() {
            requireSelectedComponent = true;
            return SetRequireSelectedGameObj();
        }

        public CCMD SetRequireSelectedGameObj() {
            requireSelectedGameObj = true;
            return this;
        }

        public CCMD AddRequiredFlags(params string[] flagNames) {
            foreach(var a in flagNames)
                requiredFlags.Add(a);
            return this;
        }

        public CCMD SetCallback(CmdCallback callback) {
            this.CmdCallback += callback;
            return this;
        }

        public CCMD SetDescription(string description) {
            this.description = description;
            return this;
        }

        public CCMD SetCommandType(CommandType type) {
            this.type = type;
            return this;
        }

        public CCMD SetUsage(string usage) {
            this.usage = usage;
            return this;
        }

        internal StringBuilder FireCallback(StringBuilder sb, CommandArgs args) {
            return CmdCallback(sb, args);
        }
    }

    [System.Flags]
    public enum CVarFlags {
        None = 0,
        Archive = 2,
        Track = 4,
        ReadOnly = 8,
        Cheat = 16,
        SP_Only = 32
    }

    public class CVar {
        public string[] aliases { get; private set; }
        public string description { get; private set; }
        string value { get; set; }
        string defValue { get; set; }

        public CVarValueChanged CVarValueChanged;
        public CVarFlags flags { get; private set; }

        public CVar(object defaultVal, string description, params string[] aliases)
            : this(defaultVal, description, CVarFlags.None, aliases) { }

        public CVar(object defaultVal, string description, CVarFlags flags, params string[] aliases) {
            this.aliases = aliases;
            this.defValue = this.value = defaultVal.ToString();
            this.description = description;
            this.flags = flags;
            UConsoleDB.RegisterConVar(this);
        }

        public void Revert() {
            this.value = defValue;
        }

        public int GetInt() {
            return int.Parse(value);
        }

        public float GetFloat() {
            return float.Parse(value);
        }

        public long GetLong() {
            return long.Parse(value);
        }

        public byte GetByte() {
            return byte.Parse(value);
        }

        public sbyte GetSByte() {
            return sbyte.Parse(value);
        }

        public bool GetBool() {
            try {
                return bool.Parse(value);
            }
            catch(Exception) {
                return int.Parse(value) != 0;
            }
        }

        public short GetShort() {
            return short.Parse(value);
        }

        public decimal GetDecimal() {
            return decimal.Parse(value);
        }

        public Vector2 GetVec2() {
            var parts = value.Split(' ');
            return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public Vector2 GetVec3() {
            var parts = value.Split(' ');
            return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
        }

        public string Get() {
            return value;
        }

        public bool TryGetInt(out int result) {
            return int.TryParse(value, out result);
        }

        public bool TryGetFloat(out float result) {
            return float.TryParse(value, out result);
        }

        public bool TryGetLong(out long result) {
            return long.TryParse(value, out result);
        }

        public bool TryGetByte(out byte result) {
            return byte.TryParse(value, out result);
        }

        public bool TryGetSByte(out sbyte result) {
            return sbyte.TryParse(value, out result);
        }

        public bool TryGetBool(out bool result) {
            var success = bool.TryParse(value, out result);
            if(!success) {
                int i;
                success = int.TryParse(value, out i);
                if(success)
                    result = i != 0;
            }
            return success;
        }

        public bool TryGetShort(out short result) {
            return short.TryParse(value, out result);
        }

        public bool TryGetDecimal(out decimal result) {
            return decimal.TryParse(value, out result);
        }

        public bool TryGetVec2(out Vector2 result) {
            var parts = value.Split(' ');
            result = Vector2.zero;
            if(parts.Length < 2)
                return false;
            float x, y;
            bool success = float.TryParse(parts[0], out x);
            if(!success)
                return false;
            success = float.TryParse(parts[1], out y);
            if(!success)
                return false;
            result = new Vector2(x, y);
            return true;
        }

        public bool TryGetVec3(out Vector3 result) {
            var parts = value.Split(' ');
            result = Vector3.zero;
            if(parts.Length < 3)
                return false;
            float x, y, z;
            bool success = float.TryParse(parts[0], out x);
            if(!success)
                return false;
            success = float.TryParse(parts[1], out y);
            if(!success)
                return false;
            success = float.TryParse(parts[2], out z);
            if(!success)
                return false;
            result = new Vector3(x, y, z);
            return true;
        }

        internal void SetVal(object o) {
            SetVal(o, true);
        }

        internal void SetVal(object o, bool alert) {
            if(value != o) {
                var old = value;
                value = o.ToString();
                if(alert && CVarValueChanged != null)
                    CVarValueChanged(old, this);
            }
        }
    }
}