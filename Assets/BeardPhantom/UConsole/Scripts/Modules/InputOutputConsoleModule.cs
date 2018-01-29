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

        /// <summary>
        /// Event triggered when input is "submitted"
        /// </summary>
        public event Action<string> InputSubmitted;

        /// <summary>
        /// Tracks how many times we need to scroll to the end of the output window
        /// </summary>
        private byte _scrollToEndCounter;

        /// <summary>
        /// Object pool for getting output line objects
        /// </summary>
        private SimplePrefabPool<AbstractConsoleOutputLine> _linePool;

        #endregion

        public InputOutputConsoleModule(Console console)
            : base(console) { }

        #region Methods

        /// <inheritdoc />
        public override void Initialize()
        {
            _linePool = new SimplePrefabPool<AbstractConsoleOutputLine>(Console.OutputLinePrefab);
            _linePool.Allocate(100, Console.ScrollRect.content);
            Console.InputField.AddEndEditListener(SubmitInput);
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            _linePool.Clear();
            InputSubmitted = null;
            Console.InputField.RemoveEndEditListener(SubmitInput);
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Prints output using error color
        /// </summary>
        /// <param name="output"></param>
        public void PrintErr(object output)
        {
            Print(output, Console.Settings.ErrorPrintColor);
        }

        /// <summary>
        /// Prints output using error color
        /// </summary>
        /// <param name="output"></param>
        public void PrintErr(string output)
        {
            Print(output, Console.Settings.ErrorPrintColor);
        }

        /// <summary>
        /// Prints output using warning color
        /// </summary>
        /// <param name="output"></param>
        public void PrintWarn(object output)
        {
            Print(output, Console.Settings.WarningPrintColor);
        }

        /// <summary>
        /// Prints output using warning color
        /// </summary>
        /// <param name="output"></param>
        public void PrintWarn(string output)
        {
            Print(output, Console.Settings.WarningPrintColor);
        }

        /// <summary>
        /// Prints output using default color
        /// </summary>
        /// <param name="output"></param>
        public void Print(object output)
        {
            Print(output, Console.Settings.DefaultPrintColor);
        }

        /// <summary>
        /// Prints output using default color
        /// </summary>
        /// <param name="output"></param>
        public void Print(string output)
        {
            Print(output, Console.Settings.DefaultPrintColor);
        }

        /// <summary>
        /// Prints output using provided color
        /// </summary>
        /// <param name="output"></param>
        /// <param name="color"></param>
        public void Print(object output, Color color)
        {
            if (output != null)
            {
                PrintInternal(output.ToString(), color);
            }
        }

        /// <summary>
        /// Prints output using provided color
        /// </summary>
        /// <param name="output"></param>
        /// <param name="color"></param>
        public void Print(string output, Color color)
        {
            if (!string.IsNullOrEmpty(output))
            {
                PrintInternal(output, color);
            }
        }

        /// <summary>
        /// If true, listen for Unity debug logging and print it to the console,
        /// if false remove any listeners
        /// </summary>
        /// <param name="isRegistered"></param>
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
            if (input != null && !IsWhitespaceString(input))
            {
                Console.StartCoroutine(DelayTrySubmit(input));
            }
        }

        /// <summary>
        /// Due to limitations in how UGUI is updated, need to wait a frame
        /// before checking if the EndEdit was sent due to enter being pressed or
        /// if the input field was just deselected.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Clears the input field
        /// </summary>
        public void ClearInput()
        {
            SetInput(string.Empty);
        }

        /// <summary>
        /// Sets the input field to value
        /// </summary>
        /// <param name="value"></param>
        public void SetInput(string value)
        {
            Console.InputField.Text = value;
            Console.InputField.CaretPosition = Console.InputField.Text.Length;
        }

        /// <summary>
        /// Clears the output window
        /// </summary>
        public void ClearOutput()
        {
            _linePool.ReturnAll();
        }

        /// <summary>
        /// Print function for unity debug logs
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void OnApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Print(string.Format("[{0}] {1}\n{2}", type, condition, stackTrace));
        }

        /// <summary>
        /// Whether this string is purely whitespace
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsWhitespaceString(string input)
        {
            for (var i =0; i < input.Length; i++)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Actually prints text to console
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        private void PrintInternal(string text, Color color)
        {
            var instance = _linePool.Retrieve(Console.ScrollRect.content);
            instance.Text = text.Trim();
            instance.Color = color;
            _scrollToEndCounter += 2;
        }

        #endregion
    }
}