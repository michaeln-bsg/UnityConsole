using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.PhantomConsole.Modules
{
    /// <summary>
    /// Console module for browsing previously executed inputs
    /// </summary>
    public class InputHistoryConsoleModule : AbstractConsoleModule
    {
        /// <summary>
        /// All inputs
        /// </summary>
        private readonly List<string> _inputHistory = new List<string>();

        /// <summary>
        /// Current position in input history
        /// </summary>
        private int _inputHistoryIndex;

        public InputHistoryConsoleModule(Console console)
            : base(console) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            Console.InputOutput.InputSubmitted += OnInputSubmitted;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            Console.InputOutput.InputSubmitted -= OnInputSubmitted;
        }

        /// <inheritdoc />
        public override void Update()
        {
            var direction = 0;

            if(ConsoleUtility.GetAnyInputDown(
                Console.Settings.InputHistoryBackwards))
            {
                // Go backwards
                direction = -1;
            }
            else if(ConsoleUtility.GetAnyInputDown(
                Console.Settings.InputHistoryForwards))
            {
                // Go forwards
                direction = 1;
            }

            if(direction != 0)
            {
                _inputHistoryIndex = Mathf.Clamp(
                    _inputHistoryIndex + direction,
                    0,
                    Mathf.Max(0, _inputHistory.Count - 1));

                if(_inputHistory.Count > 0)
                {
                    Console.InputOutput.SetInput(
                        _inputHistory[_inputHistoryIndex]);
                }
            }
        }

        /// <summary>
        /// Add input to history when input field editing ceases
        /// </summary>
        /// <param name="text"></param>
        private void OnInputSubmitted(string text)
        {
            _inputHistory.Add(text);
            _inputHistoryIndex = _inputHistory.Count;
        }
    }
}