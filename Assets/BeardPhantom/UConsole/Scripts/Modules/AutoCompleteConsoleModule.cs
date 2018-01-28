using System;

namespace BeardPhantom.UConsole.Modules
{
    public class AutoCompleteConsoleModule : AbstractConsoleModule
    {
        private readonly StringComparer _cmdComparison = StringComparer.OrdinalIgnoreCase;

        public AutoCompleteConsoleModule(Console console)
            : base(console) { }

        public override void Awake()
        {
            Console.InputField.AddOnValueChangedListener(OnInputValueChanged);
        }

        public override void Destroy()
        {
            Console.InputField.RemoveOnValueChangedListener(OnInputValueChanged);
        }

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
