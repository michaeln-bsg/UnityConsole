using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeardPhantom.UConsole
{
    public abstract class BaseDevConsole : MonoBehaviour
    {
        public static BaseDevConsole instance { get; protected set; }

        protected KeyCode[] inputHistoryUp = new KeyCode[] { KeyCode.UpArrow };
        protected KeyCode[] inputHistoryDown = new KeyCode[] { KeyCode.DownArrow };
        protected KeyCode[] inputSubmit = new KeyCode[] { KeyCode.Return, KeyCode.KeypadEnter };
        protected StringComparer cmdComparison = StringComparer.OrdinalIgnoreCase;
        protected Color errColor = Color.red;
        protected Color warningColor = new Color(1f, 0.5f, 0f);
        protected Color inputEchoColor = new Color(0.75f, 0.75f, 0.75f);
        protected Color normalColor = Color.white;
        protected RectTransform root;
        protected readonly List<string> inputHistory = new List<string>();
        protected int inputHistoryIndex;

        public abstract IDevConsoleEventHandler eventHandler { get; }
        public Dictionary<string, int> commandMap { get; protected set; }
        public List<DevCommandInfo> commands { get; protected set; }
        public bool isOpen { get; protected set; }

        public static string[] ParseCmdString(string input)
        {
            Func<char, bool> isQuote = (char ch) => ch == '\'' || ch == '"';
            var list = new List<string>();
            var inQuotes = false;
            var builder = new System.Text.StringBuilder();
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
                    if (isQuote(c) && !isEscaping)
                    {
                        // End quotes
                        inQuotes = false;
                        list.Add(builder.ToString().Trim());
                        builder.Length = 0;
                        continue;
                    }
                }
                else if (!inQuotes && isQuote(c))
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

        public void RegisterLogging(bool isRegistered)
        {
            Application.logMessageReceived -= eventHandler.OnConsoleLogReceived;
            if (isRegistered)
            {
                Application.logMessageReceived += eventHandler.OnConsoleLogReceived;
            }
        }
        public virtual void Toggle()
        {
            SetOpen(!isOpen);
        }
        public virtual void SetOpen(bool isOpen)
        {
            if (this.isOpen == isOpen)
            {
                return;
            }
            this.isOpen = isOpen;
            ClearInput();
            gameObject.SetActive(isOpen);
            eventHandler.FireConsoleToggled(isOpen);
        }
        public abstract void SetInput(string value);
        public void ClearInput()
        {
            SetInput(string.Empty);
        }
        public abstract void ClearOutput();
        public DevCommandInfo GetCommand(string alias)
        {
            int infoIndex;
            if (commandMap.TryGetValue(alias, out infoIndex))
            {
                return commands[infoIndex];
            }
            return null;
        }
        public void PrintErr(object output)
        {
            Print(output, errColor);
        }
        public void PrintErr(string output)
        {
            Print(output, errColor);
        }
        public void PrintWarn(object output)
        {
            Print(output, warningColor);
        }
        public void PrintWarn(string output)
        {
            Print(output, warningColor);
        }
        public void Print(object output)
        {
            Print(output, normalColor);
        }
        public void Print(string output)
        {
            Print(output, normalColor);
        }
        public void Print(object output, Color color)
        {
            if (output != null)
            {
                PrintInternal(output.ToString(), color);
            }
        }
        public void Print(string output, Color color)
        {
            if (!string.IsNullOrEmpty(output))
            {
                PrintInternal(output, color);
            }
        }
        protected abstract void PrintInternal(string text, Color color);
        protected virtual void RegisterCommands(params Type[] types)
        {
            commandMap.Clear();
            commands.Clear();
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var methods = new List<MethodInfo>(type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));
                var aliasList = new List<string>();
                for (int j = 0; j < methods.Count; j++)
                {
                    var method = methods[j];
                    var cmds = method.GetCustomAttributes(typeof(DevConsoleCommandAttribute), false);
                    if (cmds.Length == 0)
                    {
                        continue;
                    }
                    var cmd = cmds[0] as DevConsoleCommandAttribute;
                    aliasList.Clear();
                    aliasList.Add(method.Name);
                    aliasList.AddRange(cmd.aliases);
                    cmd.aliases = aliasList.ToArray();
                    var info = new DevCommandInfo(method, cmd);
                    commands.Add(info);
                    for (int k = 0; k < cmd.aliases.Length; k++)
                    {
                        if (!commandMap.ContainsKey(cmd.aliases[k]))
                        {
                            commandMap.Add(cmd.aliases[k], commands.Count - 1);
                        }
                    }
                }
            }
        }
        protected virtual void ExecuteCommandString(string text)
        {
            PrintInternal("> " + text, inputEchoColor);
            inputHistory.Add(text);
            inputHistoryIndex = inputHistory.Count;
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
            if (cmd.totalParameters > 0)
            {
                var paramsPartsLength = parts.Length - 1;
                if (paramsPartsLength < cmd.requiredParameters)
                {
                    PrintErr("MIN REQUIRED PARAMETERS MISSING");
                    return;
                }
                passedValues = new object[cmd.totalParameters];
                for (int i = 0; i < cmd.totalParameters; i++)
                {
                    var p = cmd.parameters[i];
                    /*
                     * Use provided value or default if
                     * optional parameter hasn't been specified
                     */
                    var value = i >= paramsPartsLength ? p.DefaultValue : parts[i + 1];
                    if (cmd.totalParameters == 1 && p.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length == 0)
                    {

                    }
                    passedValues[i] = Convert.ChangeType(value, p.ParameterType);
                }
            }
            Print(cmd.method.Invoke(null, passedValues));
        }
        protected void SubmitInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            ClearInput();
            ExecuteCommandString(input);
        }
        protected void UpdateInputHistory()
        {
            var didOffsetInputHistory = false;
            if (GetInputDown(inputHistoryUp))
            {
                didOffsetInputHistory = true;
                inputHistoryIndex = Mathf.Clamp(inputHistoryIndex - 1, 0, Mathf.Max(0, inputHistory.Count - 1));
            }
            else if (GetInputDown(inputHistoryDown))
            {
                didOffsetInputHistory = true;
                inputHistoryIndex = Mathf.Clamp(inputHistoryIndex + 1, 0, Mathf.Max(0, inputHistory.Count - 1));
            }
            if (didOffsetInputHistory)
            {
                if (inputHistory.Count > 0)
                {
                    SetInput(inputHistory[inputHistoryIndex]);
                }
            }
        }
        protected bool GetInputDown(KeyCode[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Input.GetKeyDown(input[i]))
                {
                    return true;
                }
            }
            return false;
        }
        protected virtual void Awake()
        {
            instance = this;
            isOpen = true;
            commandMap = new Dictionary<string, int>(cmdComparison);
            commands = new List<DevCommandInfo>();
            root = transform.GetChild(0) as RectTransform;
        }
        protected virtual void OnEnable()
        {
            SetOpen(true);
        }
        protected virtual void OnDisable()
        {
            SetOpen(false);
        }
    }
}