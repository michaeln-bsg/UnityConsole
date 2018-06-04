using System;
using System.Collections.Generic;

namespace BeardPhantom.PhantomConsole.Modules
{
    public class AutoCompleteConsoleModule : ConsoleModule
    {
        private readonly List<string> _foundAliases = new List<string>();

        private int _selectionIndex = -1;

        /// <inheritdoc />
        public AutoCompleteConsoleModule(Console console) : base(console) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            Console.InputField.AddOnValueChangedListener(OnInputFieldValueChanged);
            Enabled = false;
        }

        /// <inheritdoc />
        public override void Destroy() { }

        /// <inheritdoc />
        public override void Update()
        {
            var oldSelection = _selectionIndex;
            if(ConsoleUtility.GetAnyInputDown(Console.Settings.AutoCompleteBackwards))
            {
                _selectionIndex--;
                if(_selectionIndex < 0)
                {
                    _selectionIndex = _foundAliases.Count - 1;
                }
            }
            else if (ConsoleUtility.GetAnyInputDown(Console.Settings.AutoCompleteForwards))
            {
                _selectionIndex++;
                if (_selectionIndex >= _foundAliases.Count)
                {
                    _selectionIndex = 0;
                }
            }
            if(oldSelection != _selectionIndex)
            {
                Console.InputField.RemoveOnValueChangedListener(OnInputFieldValueChanged);
                Console.InputField.Text = _foundAliases[_selectionIndex];
                Console.InputField.CaretPosition = _foundAliases[_selectionIndex].Length;
                Console.InputField.AddOnValueChangedListener(OnInputFieldValueChanged);
            }
        }

        private void OnInputFieldValueChanged(string value)
        {
            _foundAliases.Clear();
            _selectionIndex = -1;
            if (ConsoleUtility.IsNullOrWhitespace(value))
            {
                Enabled = false;
                Console.AutoCompleteWindow.gameObject.SetActive(false);
                Console.InputHistory.Enabled = true;
                return;
            }

            foreach(var cmdAlias in Console.Commands.CommandMap.Keys)
            {
                if (cmdAlias.StartsWith(value, StringComparison.OrdinalIgnoreCase))
                {
                    _foundAliases.Add(cmdAlias);
                }
            }

            _foundAliases.Sort(StringComparer.OrdinalIgnoreCase);
            Enabled = _foundAliases.Count > 0;
            Console.AutoCompleteWindow.gameObject.SetActive(Enabled);
            Console.AutoCompleteWindow.SetRowValues(_foundAliases);
            Console.InputHistory.Enabled = !Enabled;
        }
    }
}