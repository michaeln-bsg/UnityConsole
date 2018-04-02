using UnityEngine;
using UnityEngine.Events;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Provides access to a GUI based input field
    /// </summary>
    public abstract class AbstractConsoleInputField : MonoBehaviour
    {
        /// <summary>
        /// Get/set access to the current input value
        /// </summary>
        public abstract string Text { get; set; }

        /// <summary>
        /// Get/set access to the input caret position
        /// </summary>
        public abstract int CaretPosition { get; set; }

        /// <summary>
        /// Get/set access for selecting this input field
        /// </summary>
        public abstract bool IsSelected { get; set; }

        /// <summary>
        /// Adds a callback for when editing has ceased.
        /// </summary>
        public abstract void AddEndEditListener(UnityAction<string> action);

        /// <summary>
        /// Removes a callback for when editing has ceased.
        /// </summary>
        public abstract void RemoveEndEditListener(UnityAction<string> action);

        /// <summary>
        /// Adds a callback for when the input field's value changes
        /// </summary>
        public abstract void AddOnValueChangedListener(
            UnityAction<string> action);

        /// <summary>
        /// Removes a callback for when the input field's value changes
        /// </summary>
        public abstract void RemoveOnValueChangedListener(
            UnityAction<string> action);
    }
}