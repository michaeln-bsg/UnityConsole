using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Rubycone.UConsole {
    public delegate bool CommandExecuted(string[] arguments);

    [System.Flags]
    public enum CCommandFlags {
        None = 0,
        RequireArgs = 1,
        RequireSelectedObj = 2,
        RequireSelectedComponent = 4,
        EditorOnly = 8,
        BuildOnly = 16
    }

    [System.Flags]
    public enum CCommandExecResults {
        None = 0,
        Success = 1,
        GenericFailure = 2,
        MissingArgs = 4,
        NoSelectedGameObj = 8,
        NoSelectedComponent = 16,
        NotInEditor = 32,
        NotInBuild = 64
    }

    public sealed class CCommand : CFunc {
        string INVALID_USAGE_ERR;
        public static readonly string NO_OBJ_SELECTED_ERR = UConsole.ColorizeErr("COMMAND REQUIRES A SELECTED GAMEOBJECT");
        public static readonly string NO_COMPONENT_SELECTED_ERR = UConsole.ColorizeErr("COMMAND REQUIRES A SELECTED COMPONENT");

        internal CCommandFlags flags { get; private set; }
        internal string usage { get; private set; }
        public event CommandExecuted CommandExecuted;

        public CCommand(string alias, string description)
            : this(alias, description, "noargs") { }

        public CCommand(string alias, string description, string usage)
            : this(alias, description, usage, CCommandFlags.None) { }

        public CCommand(string alias, string description, CCommandFlags flags)
            : this(alias, description, "noargs", flags) { }

        public CCommand(string alias, string description, string usage, CCommandFlags flags)
            : base(alias, description) {
            this.usage = usage;
            this.flags = flags;
        }

        public string GetUsageErr() {
            //so that we don't keep allocating strings
            if(INVALID_USAGE_ERR == null) {
                INVALID_USAGE_ERR = UConsole.ColorizeErr("USAGE: " + usage);
            }
            return INVALID_USAGE_ERR;
        }

        public CCommandExecResults Execute(string[] arguments) {
            var result = CCommandExecResults.None;

            //init check before execution
            if((flags & CCommandFlags.RequireArgs) != 0 && arguments.Length == 0) {
                result |= CCommandExecResults.MissingArgs;
            }
            if((flags & CCommandFlags.RequireSelectedObj) != 0 && UConsole.selectedObj == null) {
                result |= CCommandExecResults.NoSelectedGameObj;
            }
            if((flags & CCommandFlags.RequireSelectedComponent) != 0 && UConsole.selectedComponent == null) {
                result |= CCommandExecResults.NoSelectedComponent;
            }
            if((flags & CCommandFlags.EditorOnly) != 0 && !Application.isEditor) {
                result |= CCommandExecResults.NotInEditor;
            }
            if((flags & CCommandFlags.BuildOnly) != 0 && Application.isEditor) {
                result |= CCommandExecResults.NotInBuild;
            }

            if(result == CCommandExecResults.None && CommandExecuted != null) {
                bool success;
                try {
                    success = CommandExecuted(arguments);
                }
                catch(System.Exception) {
                    success = false;
                }
                result = (success) ? CCommandExecResults.Success : CCommandExecResults.GenericFailure;
            }

            return result;
        }
    }
}