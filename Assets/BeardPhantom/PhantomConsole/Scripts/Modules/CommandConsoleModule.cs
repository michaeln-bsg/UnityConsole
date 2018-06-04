using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BeardPhantom.PhantomConsole.Modules
{
    /// <summary>
    /// Built-in module for handling parsing and executing commands
    /// </summary>
    public class CommandConsoleModule : ConsoleModule
    {
        /// <summary>
        /// Name-to-index mapping of commands
        /// </summary>
        public readonly Dictionary<string, int> CommandMap =
            new Dictionary<string, int>();

        /// <summary>
        /// Flat list of all command metadata for execution
        /// </summary>
        public readonly List<CommandMetadata> CommandList =
            new List<CommandMetadata>();

        public CommandConsoleModule(Console console) : base(console) { }

        /// <summary>
        /// Whether this character is any quote character
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsQuoteCharacter(char c)
        {
            return c == '\'' || c == '"';
        }

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

            if(CommandMap.TryGetValue(alias, out infoIndex))
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
            Console.InputOutput.Print(
                "> " + text,
                Console.Settings.InputEchoPrintColor);

            var splitInput = Regex.Split(text, @"\s");

            if(splitInput.Length == 0)
            {
                Console.InputOutput.PrintErr("COULD NOT PARSE CMD STRING");
                return;
            }

            var cmd = GetCommand(splitInput[0]);

            if(cmd == null)
            {
                Console.InputOutput.PrintErr(string.Format("CMD NOT FOUND: '{0}'", splitInput[0]));
                return;
            }

            List<object> passedValues = null;

            if(cmd.TotalParameters > 0)
            {
                if(splitInput.Length - 1 < cmd.ProvidableRequiredParameters)
                {
                    Console.InputOutput.PrintErr("MIN REQUIRED PARAMETERS MISSING");
                    return;
                }

                passedValues = new List<object>();

                var specialParamsHit = 0;
                for(var i = 0; i < cmd.TotalParameters; i++)
                {
                    var parameter = cmd.Parameters[i];

                    if(parameter.IsSpecialParameter())
                    {
                        specialParamsHit++;
                        passedValues.Add(Console);
                        continue;
                    }

                    /*
                     * Use provided value or default if
                     * optional parameter hasn't been specified
                     */
                    var currentParamIndex = i + 1 - specialParamsHit;

                    if(parameter.IsParamsParameter())
                    {
                        var elementType = parameter.ParameterType.GetElementType();
                        var arrayLength = splitInput.Length - currentParamIndex;
                        var array = Array.CreateInstance(elementType, arrayLength);
                        for(var j = 0; j < arrayLength; j++)
                        {
                            array.SetValue(Convert.ChangeType(splitInput[j + currentParamIndex], elementType), j);
                        }
                        passedValues.Add(array);
                    }
                    else
                    {
                        var value = currentParamIndex >= splitInput.Length 
                            ? parameter.DefaultValue 
                            : splitInput[currentParamIndex];
                        passedValues.Add(Convert.ChangeType(value, parameter.ParameterType));
                    }
                }
            }

            Console.InputOutput.Print(cmd.Method.Invoke(null, passedValues == null ? null : passedValues.ToArray()));
        }

        /// <summary>
        /// Lexer for input string
        /// </summary>
        private string[] TokenizeInput(string input)
        {
            var list = new List<string>();
            var inQuotes = false;
            var builder = new StringBuilder();
            var chr = '\0';

            for(var i = 0; i < input.Length; i++)
            {
                var lastChr = chr;
                chr = input[i];
                var isEscaping = lastChr == '\\' && !char.IsWhiteSpace(chr);

                if(inQuotes && IsQuoteCharacter(chr) && !isEscaping)
                {
                    // End quotes
                    inQuotes = false;
                    list.Add(builder.ToString().Trim());
                    builder.Length = 0;
                }
                else if(!inQuotes && IsQuoteCharacter(chr))
                {
                    inQuotes = true;
                }
                else if(char.IsWhiteSpace(chr) && builder.Length > 0)
                {
                    list.Add(builder.ToString().Trim());
                    builder.Length = 0;
                }

                builder.Append(chr);
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

            var methodCmdMap =
                AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .SelectMany(
                             t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
                         .Where(m => m.GetCustomAttributes(typeof(ConsoleCommandAttribute), false).Length > 0)
                         .ToDictionary(m => m, m => m.GetCustomAttributes(typeof(CommandAttribute), false));

            var aliasList = new List<string>();

            foreach(var pair in methodCmdMap)
            {
                aliasList.Clear();
                var description = "NO DESCRIPTION";

                foreach(var attribute in pair.Value)
                {
                    var cmd = attribute as ConsoleCommandAttribute;

                    if(cmd != null)
                    {
                        aliasList.AddRange(cmd.Aliases);

                        continue;
                    }

                    var descr = attribute as ConsoleCommandDescriptionAttribute;

                    if(descr != null)
                    {
                        description = descr.Description;
                    }
                }

                var info = new CommandMetadata(pair.Key, aliasList, description);

                CommandList.Add(info);

                for(var i = 0; i < aliasList.Count; i++)
                {
                    if(!CommandMap.ContainsKey(aliasList[i]))
                    {
                        CommandMap.Add(aliasList[i], CommandList.Count - 1);
                    }
                }
            }
        }
    }
}