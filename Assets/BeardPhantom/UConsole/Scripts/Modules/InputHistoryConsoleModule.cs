using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.UConsole.Modules
{
    public class InputHistoryConsoleModule : AbstractConsoleModule
    {
        private readonly List<string> _inputHistory = new List<string>();

        private int _inputHistoryIndex;

        public InputHistoryConsoleModule(Console console)
            : base(console) { }

        public override void Awake()
        {
            Console.InputOutput.InputSubmitted += OnInputSubmitted;
        }

        public override void Destroy()
        {
            Console.InputOutput.InputSubmitted -= OnInputSubmitted;
        }

        public override void Update()
        {
            var direction = 0;
            if (ConsoleUtility.GetInputDown(Console.Settings.InputHistoryUp))
            {
                direction = -1;
            }
            else if (ConsoleUtility.GetInputDown(Console.Settings.InputHistoryDown))
            {
                direction = 1;
            }

            if (direction != 0)
            {
                _inputHistoryIndex = Mathf.Clamp(
                    _inputHistoryIndex + direction,
                    0,
                    Mathf.Max(0, _inputHistory.Count - 1));

                if (_inputHistory.Count > 0)
                {
                    Console.InputOutput.SetInput(_inputHistory[_inputHistoryIndex]);
                }
            }
        }

        private void OnInputSubmitted(string text)
        {
            _inputHistory.Add(text);
            _inputHistoryIndex = _inputHistory.Count;
        }
    }
}
