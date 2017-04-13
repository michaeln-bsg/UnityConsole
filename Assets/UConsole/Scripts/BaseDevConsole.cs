using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeardPhantom.UConsole
{
    public abstract class BaseDevConsole : MonoBehaviour
    {
        public static BaseDevConsole instance { get; protected set; }

        [SerializeField]
        protected InputField input;
        [SerializeField]
        protected Text outputTemplate;
        protected KeyCode inputHistoryUp = KeyCode.UpArrow;
        protected KeyCode inputHistoryDown = KeyCode.DownArrow;
        protected Color errColor = Color.red;
        protected Color warningColor = new Color(1f, 0.5f, 0f);
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
        public void ClearOutput()
        {
            var parent = outputTemplate.transform.parent;
            outputTemplate.transform.SetAsFirstSibling();
            for (int i = parent.childCount - 1; i > 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        public void ClearInput()
        {
            input.text = string.Empty;
        }
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
            if (output != null)
            {
                PrintInternal(output.ToString(), errColor);
            }
        }
        public void PrintErr(string output)
        {
            if (!string.IsNullOrEmpty(output))
            {
                PrintInternal(output, errColor);
            }
        }
        public void PrintWarn(object output)
        {
            if (output != null)
            {
                PrintInternal(output.ToString(), warningColor);
            }
        }
        public void PrintWarn(string output)
        {
            if (!string.IsNullOrEmpty(output))
            {
                PrintInternal(output, warningColor);
            }
        }
        public void Print(object output)
        {
            if (output != null)
            {
                PrintInternal(output.ToString(), normalColor);
            }
        }
        public void Print(string output)
        {
            if (!string.IsNullOrEmpty(output))
            {
                PrintInternal(output, normalColor);
            }
        }
        protected void PrintInternal(string text, Color color)
        {
            var instance = Instantiate(outputTemplate, outputTemplate.transform.parent);
            instance.text = text.Trim();
            instance.color = color;
            instance.gameObject.SetActive(true);
        }
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
            Print("> " + text);
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
                    passedValues[i] = Convert.ChangeType(value, p.ParameterType);
                }
            }
            Print(cmd.method.Invoke(null, passedValues));
        }
        protected virtual void OnInputEditEnded(string text)
        {
            text = text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            ExecuteCommandString(text);
            input.ActivateInputField();
            ClearInput();
        }
        protected virtual void Awake()
        {
            instance = this;
            isOpen = true;
            outputTemplate.gameObject.SetActive(false);
            commandMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            commands = new List<DevCommandInfo>();
            root = transform.GetChild(0) as RectTransform;
            input.onEndEdit.AddListener(OnInputEditEnded);
        }
        protected void UpdateInputHistory()
        {
            var didOffsetInputHistory = false;
            if (Input.GetKeyDown(inputHistoryUp))
            {
                didOffsetInputHistory = true;
                inputHistoryIndex = Mathf.Clamp(inputHistoryIndex - 1, 0, Mathf.Max(0, inputHistory.Count - 1));
            }
            else if (Input.GetKeyDown(inputHistoryDown))
            {
                didOffsetInputHistory = true;
                inputHistoryIndex = Mathf.Clamp(inputHistoryIndex + 1, 0, Mathf.Max(0, inputHistory.Count - 1));
            }
            if (didOffsetInputHistory)
            {
                if (inputHistory.Count > 0)
                {
                    input.text = inputHistory[inputHistoryIndex];
                    input.caretPosition = input.text.Length;
                }
            }
        }
        protected virtual void OnEnable()
        {
            SetOpen(true);
            input.ActivateInputField();
        }
        protected virtual void OnDisable()
        {
            SetOpen(false);
            input.DeactivateInputField();
        }
    }
}