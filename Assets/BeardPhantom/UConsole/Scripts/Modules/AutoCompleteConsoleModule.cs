/*
using System;

namespace BeardPhantom.UConsole.Modules
{
    /// <summary>
    /// Work in progress. Used to help auto-complete partially typed commands
    /// </summary>
    public class AutoCompleteConsoleModule : AbstractConsoleModule
    {
        private readonly StringComparer _cmdComparison = StringComparer.OrdinalIgnoreCase;

        public AutoCompleteConsoleModule(Console console)
            : base(console) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            Console.InputField.AddOnValueChangedListener(OnInputValueChanged);
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            Console.InputField.RemoveOnValueChangedListener(OnInputValueChanged);
        }

        /// <inheritdoc />
        public override void Update() { }

        private void OnInputValueChanged(string value)
        {
            if (value.Contains("\t"))
            {
                Console.InputField.Text = Console.InputField.Text.Replace("\t", "");
            }
        }
    }
}
*/