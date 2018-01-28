using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BeardPhantom.UConsole.Modules
{
    public class CommandConsoleModule : AbstractConsoleModule
    {
        public readonly Dictionary<string, int> CommandMap = new Dictionary<string, int>();

        public readonly List<CommandInfo> CommandList = new List<CommandInfo>();

        private readonly Dictionary<Type, object> _commandRegistryInstances
            = new Dictionary<Type, object>();

        public CommandConsoleModule(Console console)
            : base(console) { }

        public override void Awake()
        {
            Console.InputOutput.InputSubmitted += ExecuteCommandString;
            Console.ConsoleReset += SetCommands;
        }

        public override void Destroy()
        {
            Console.ConsoleReset -= SetCommands;
            Console.InputOutput.InputSubmitted -= ExecuteCommandString;
        }

        public override void Update() { }

        public CommandInfo GetCommand(string alias)
        {
            int infoIndex;
            if (CommandMap.TryGetValue(alias, out infoIndex))
            {
                return CommandList[infoIndex];
            }
            return null;
        }

        public void ExecuteCommandString(string text)
        {
            Console.InputOutput.Print("> " + text, Console.Settings.InputEchoColor);
            var parts = text.Split(' ');
            if (parts.Length == 0)
            {
                Console.InputOutput.PrintErr("COULD NOT PARSE CMD STRING");
                return;
            }
            var cmd = GetCommand(parts[0]);
            if (cmd == null)
            {
                Console.InputOutput.PrintErr(string.Format("CMD NOT FOUND: '{0}'", parts[0]));
                return;
            }
            object[] passedValues = null;
            if (cmd.TotalParameters > 0)
            {
                var paramsPartsLength = parts.Length - 1;
                if (paramsPartsLength < cmd.RequiredParameters)
                {
                    Console.InputOutput.PrintErr("MIN REQUIRED PARAMETERS MISSING");
                    return;
                }
                passedValues = new object[cmd.TotalParameters];
                for (var i = 0; i < cmd.TotalParameters; i++)
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
            Console.InputOutput.Print(cmd.Method.Invoke(commandRegistryInstance, passedValues));
        }

        private string[] ParseCmdString(string input)
        {
            var list = new List<string>();
            var inQuotes = false;
            var builder = new StringBuilder();
            var isEscaping = false;
            var c = '\0';
            var lastC = c;
            for (var i = 0; i < input.Length; i++)
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

        private void SetCommands(ConsoleSetupOptions options)
        {
            CommandMap.Clear();
            CommandList.Clear();
            _commandRegistryInstances.Clear();
            var aliasList = new List<string>();
            foreach (var type in options.CommandRegistryTypes)
            {
                var instance = Activator.CreateInstance(type, (object)Console);
                _commandRegistryInstances.Add(type, instance);
                var methods = new List<MethodInfo>(type.GetMethods(
                    BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Static
                    | BindingFlags.Instance));

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
                    var info = new CommandInfo(method, aliasList.ToArray(), cmd.Description);
                    CommandList.Add(info);
                    for (var i = 0; i < aliasList.Count; i++)
                    {
                        if (!CommandMap.ContainsKey(aliasList[i]))
                        {
                            CommandMap.Add(aliasList[i], CommandList.Count - 1);
                        }
                    }
                }
            }
        }

        private bool IsQuoteCharacter(char c)
        {
            return c == '\'' || c == '"';
        }
    }
}
