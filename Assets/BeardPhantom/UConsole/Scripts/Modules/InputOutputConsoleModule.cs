using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BeardPhantom.UConsole.Modules
{
    /// <summary>
    /// Handles text input/output
    /// </summary>
    public class InputOutputConsoleModule : AbstractConsoleModule
    {
        #region Fields

        private const string EMPTY_INPUT_REGEX_PATTERN = @"^\s*\n+$";

        public event Action<string> InputSubmitted;

        private readonly Regex _emptyInputRegex = new Regex(EMPTY_INPUT_REGEX_PATTERN);

        private byte _scrollToEndCounter;

        private SimplePrefabPool<AbstractConsoleOutputLine> _linePool;

        #endregion

        public InputOutputConsoleModule(Console console)
            : base(console) { }

        #region Methods

        public override void Awake()
        {
            _linePool = new SimplePrefabPool<AbstractConsoleOutputLine>(Console.OutputLinePrefab);
            _linePool.Allocate(100, Console.ScrollRect.content);
            Console.InputField.AddEndEditListener(SubmitInput);
        }

        public override void Destroy()
        {
            _linePool.Clear();
            InputSubmitted = null;
            Console.InputField.RemoveEndEditListener(SubmitInput);
        }

        public override void Update()
        {
            if (_scrollToEndCounter > 0)
            {
                _scrollToEndCounter--;
                if (_scrollToEndCounter == 0)
                {
                    Console.ScrollRect.verticalNormalizedPosition = 0f;
                }
            }
        }

        public void PrintErr(object output)
        {
            Print(output, Console.Settings.ErrorColor);
        }

        public void PrintErr(string output)
        {
            Print(output, Console.Settings.ErrorColor);
        }

        public void PrintWarn(object output)
        {
            Print(output, Console.Settings.WarningColor);
        }

        public void PrintWarn(string output)
        {
            Print(output, Console.Settings.WarningColor);
        }

        public void Print(object output)
        {
            Print(output, Console.Settings.DefaultColor);
        }

        public void Print(string output)
        {
            Print(output, Console.Settings.DefaultColor);
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

        public void SetReceiveDebugLogging(bool isRegistered)
        {
            Application.logMessageReceived -= OnApplicationLogMessageReceived;
            if (isRegistered)
            {
                Application.logMessageReceived += OnApplicationLogMessageReceived;
            }
        }

        public void SubmitInput(string input)
        {
            if (input != null && !_emptyInputRegex.IsMatch(input))
            {
                Console.StartCoroutine(DelayTrySubmit(input));
            }
        }

        private IEnumerator DelayTrySubmit(string input)
        {
            yield return null;
            if (Console.InputField.IsSelected)
            {
                ClearInput();
                if (InputSubmitted != null)
                {
                    InputSubmitted(input);
                }
                Console.InputField.IsSelected = true;
            }
        }

        public void ClearInput()
        {
            SetInput(string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetInput(string value)
        {
            Console.InputField.Text = value;
            Console.InputField.CaretPosition = Console.InputField.Text.Length;
        }

        public void ClearOutput()
        {
            _linePool.ReturnAll();
        }

        private void OnApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Print(string.Format("[{0}] {1}\n{2}", type, condition, stackTrace));
        }

        private void PrintInternal(string text, Color color)
        {
            var instance = _linePool.Retrieve(Console.ScrollRect.content);
            instance.Text = text.Trim();
            instance.Color = color;
            instance.gameObject.SetActive(true);
            _scrollToEndCounter = 2;
        }

        #endregion
    }
}