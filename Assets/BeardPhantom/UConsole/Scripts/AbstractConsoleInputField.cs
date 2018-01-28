using UnityEngine;
using UnityEngine.Events;

namespace BeardPhantom.UConsole
{
    public abstract class AbstractConsoleInputField : MonoBehaviour
    {
        public abstract string Text { get; set; }

        public abstract int CaretPosition { get; set; }

        public abstract bool IsSelected { get; set; }

        public abstract void AddEndEditListener(UnityAction<string> action);

        public abstract void RemoveEndEditListener(UnityAction<string> action);

        public abstract void AddOnValueChangedListener(UnityAction<string> action);

        public abstract void RemoveOnValueChangedListener(UnityAction<string> action);
    }
}
