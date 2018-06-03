using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// A wrapper class to provide UConsole UGUI InputField objects
    /// </summary>
    public class UGUIInputFieldConsoleWrapper : AbstractConsoleInputField
    {
        [SerializeField] private InputField _inputField;

        /// <inheritdoc />
        public override string Text
        {
            get { return _inputField.text; }
            set { _inputField.text = value; }
        }

        /// <inheritdoc />
        public override int CaretPosition
        {
            get { return _inputField.caretPosition; }
            set { _inputField.caretPosition = value; }
        }

        /// <inheritdoc />
        public override bool IsSelected
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject
                       == _inputField.gameObject;
            }
            set
            {
                if(value)
                {
                    _inputField.Select();
                    _inputField.ActivateInputField();
                }
                else if(!value
                        && EventSystem.current.currentSelectedGameObject
                        == _inputField.gameObject)
                {
                    _inputField.DeactivateInputField();
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }

        /// <inheritdoc />
        public override void AddEndEditListener(UnityAction<string> action)
        {
            _inputField.onEndEdit.AddListener(action);
        }

        /// <inheritdoc />
        public override void RemoveEndEditListener(UnityAction<string> action)
        {
            _inputField.onEndEdit.RemoveListener(action);
        }

        /// <inheritdoc />
        public override void AddOnValueChangedListener(
            UnityAction<string> action)
        {
            _inputField.onValueChanged.AddListener(action);
        }

        /// <inheritdoc />
        public override void RemoveOnValueChangedListener(
            UnityAction<string> action)
        {
            _inputField.onValueChanged.RemoveListener(action);
        }

        /// <summary>
        /// Look for text component on object when script is added
        /// </summary>
        private void Reset()
        {
            _inputField = GetComponent<InputField>();
        }
    }
}