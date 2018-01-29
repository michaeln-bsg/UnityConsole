using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BeardPhantom.UConsole.Modules
{
    /// <summary>
    /// Built-in module for handling parsing and executing commands
    /// </summary>
    public class CommandConsoleModule : AbstractConsoleModule
    {
        /// <summary>
        /// Name-to-index mapping of commands
        /// </summary>
        public readonly Dictionary<string, int> CommandMap = new Dictionary<string, int>();

        /// <summary>
        /// Flat list of all command metadata for execution
        /// </summary>
        public readonly List<CommandMetadata> CommandList = new List<CommandMetadata>();

        /// <summary>
        /// Type-to-instance mapping of all command registries
        /// </summary>
        private readonly Dictionary<Type, object> _commandRegistryInstances
            = new Dictionary<Type, object>();

        public CommandConsoleModule(Console console)
            : base(console) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            Console.InputOutput.InputSubmitted += ExecuteCommandString;
            Console.ConsoleReset += SetCommands;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            Console.ConsoleReset -= SetCommands;
            Console.InputOutput.InputSubmitted -= ExecuteCommandString;
        }

        /// <inheritdoc />
        public override void Update() { }

        /// <summary>
        /// Retrieves a command by name
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public CommandMetadata GetCommand(string alias)
        {
            int infoIndex;
            if (CommandMap.TryGetValue(alias, out infoIndex))
            {
                return CommandList[infoIndex];
            }
            return null;
        }

        /// <summary>
        /// Attempts to parse and execute an input string
        /// </summary>
        /// <param name="text"></param>
        public void ExecuteCommandString(string text)
        {
            Console.InputOutput.Print("> " + text, Console.Settings.InputEchoPrintColor);
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

        /// <summary>
        /// Lexer for input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string[] TokenizeInput(string input)
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

        /// <summary>
        /// Completely clears and reloads new commands from setup options
        /// </summary>
        /// <param name="options"></param>
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
                    var attributes = method.GetCustomAttributes(typeof(CommandAttribute), false);
                    if (attributes.Length == 0)
                    {
                        continue;
                    }
                    var description = "NO DESCRIPTION";
                    aliasList.Clear();
                    aliasList.Add(method.Name);
                    foreach (var a in attributes)
                    {
                        if (a is CommandDescriptionAttribute)
                        {
                            description = ((CommandDescriptionAttribute)a).Description;
                        }
                        else if (a is CommandAliasesAttribute)
                        {
                            aliasList.AddRange(((CommandAliasesAttribute)a).Aliases);
                        }
                    }
                    var info = new CommandMetadata(method, aliasList.ToArray(), description);
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

        /// <summary>
        /// Whether this character is any quote character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsQuoteCharacter(char c)
        {
            return c == '\'' || c == '"';
        }
    }
}
