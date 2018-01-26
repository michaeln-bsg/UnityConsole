using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BeardPhantom.UConsole
{
    public partial class Console
    {
        public readonly Dictionary<string, int> CommandMap = new Dictionary<string, int>();

        public readonly List<DevCommandInfo> Commands = new List<DevCommandInfo>();

        private readonly Dictionary<Type, object> _commandRegistryInstances
            = new Dictionary<Type, object>();

        public void SetCommands(params Type[] types)
        {
            CommandMap.Clear();
            Commands.Clear();
            _commandRegistryInstances.Clear();
            var aliasList = new List<string>();
            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                _commandRegistryInstances.Add(type, instance);
                var methods = new List<MethodInfo>(type.GetMethods(
                    BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Static));

                foreach (var method in methods)
                {
                    var cmds = method.GetCustomAttributes(typeof(ConsoleCommandAttribute), false);
                    if (cmds.Length == 0)
                    {
                        continue;
                    }
                    var cmd = cmds[0] as ConsoleCommandAttribute;
                    aliasList.Clear();
                    aliasList.Add(method.Name);
                    aliasList.AddRange(cmd.Aliases);
                    //cmd.Aliases = aliasList.ToArray();
                    var info = new DevCommandInfo(method, cmd);
                    Commands.Add(info);
                    for (var i = 0; i < cmd.Aliases.Length; i++)
                    {
                        if (!CommandMap.ContainsKey(cmd.Aliases[i]))
                        {
                            CommandMap.Add(cmd.Aliases[i], Commands.Count - 1);
                        }
                    }
                }
            }
        }

        protected void ExecuteCommandString(string text)
        {
            PrintInternal("> " + text, _settings.InputEchoColor);
            _inputHistory.Add(text);
            _inputHistoryIndex = _inputHistory.Count;
            var parts = text.Split(' ');
            if (parts.Length == 0)
            {
                PrintErr("COULD NOT PARSE CMD STRING");
                return;
            }
            var cmd = GetCommand(parts[0]);
            if (cmd == null)
            {
                PrintErr(string.Format("CMD NOT FOUND: '{0}'", parts[0]));
                return;
            }
            object[] passedValues = null;
            if (cmd.TotalParameters > 0)
            {
                var paramsPartsLength = parts.Length - 1;
                if (paramsPartsLength < cmd.RequiredParameters)
                {
                    PrintErr("MIN REQUIRED PARAMETERS MISSING");
                    return;
                }
                passedValues = new object[cmd.TotalParameters];
                for (int i = 0; i < cmd.TotalParameters; i++)
                {
                    var p = cmd.Parameters[i];
                    /*
                     * Use provided value or default if
                     * optional parameter hasn't been specified
                     */
                    var value = i >= paramsPartsLength ? p.DefaultValue : parts[i + 1];
                    if (cmd.TotalParameters == 1 && p.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length == 0)
                    {

                    }
                    passedValues[i] = Convert.ChangeType(value, p.ParameterType);
                }
            }
            var commandRegistryInstance = cmd.Method.IsStatic
                ? null
                : _commandRegistryInstances[cmd.Method.DeclaringType];
            Print(cmd.Method.Invoke(commandRegistryInstance, passedValues));
        }

        private string[] ParseCmdString(string input)
        {
            var list = new List<string>();
            var inQuotes = false;
            var builder = new StringBuilder();
            var isEscaping = false;
            var c = '\0';
            var lastC = c;
            for (int i = 0; i < input.Length; i++)
            {
                lastC = c;
                c = input[i];
                isEscaping = lastC == '\\' && !char.IsWhiteSpace(c);
                if (inQuotes)
                {
                    if (IsQuoteCharacter(c) && !isEscaping)
                    {
                        // End quotes
                        inQuotes = false;
                        list.Add(builder.ToString().Trim());
                        builder.Length = 0;
                        continue;
                    }
                }
                else if (!inQuotes && IsQuoteCharacter(c))
                {
                    inQuotes = true;
                    continue;
                }
                else if (char.IsWhiteSpace(c) && builder.Length > 0)
                {
                    list.Add(builder.ToString().Trim());
                    builder.Length = 0;
                    continue;
                }
                builder.Append(c);
            }
            list.Add(builder.ToString().Trim());
            return list.ToArray();
        }

        private bool IsQuoteCharacter(char c)
        {
            return c == '\'' || c == '"';
        }
    }
}
