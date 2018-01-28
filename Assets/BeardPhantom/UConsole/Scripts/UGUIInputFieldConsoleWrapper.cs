using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BeardPhantom.UConsole
{
    public class UGUIInputFieldConsoleWrapper : AbstractConsoleInputField
    {
        [SerializeField]
        private InputField _inputField;

        public override string Text
        {
            get
            {
                return _inputField.text;
            }
            set
            {
                _inputField.text = value;
            }
        }

        public override int CaretPosition
        {
            get
            {
                return _inputField.caretPosition;
            }
            set
            {
                _inputField.caretPosition = value;
            }
        }

        public override bool IsSelected
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject == _inputField.gameObject;
            }
            set
            {
                if (value)
                {
                    _inputField.Select();
                    _inputField.ActivateInputField();
                }
                else if (!value && EventSystem.current.currentSelectedGameObject == _inputField.gameObject)
                {
                    _inputField.DeactivateInputField();
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }

        public override void AddEndEditListener(UnityAction<string> action)
        {
            _inputField.onEndEdit.AddListener(action);
        }

        public override void RemoveEndEditListener(UnityAction<string> action)
        {
            _inputField.onEndEdit.RemoveListener(action);
        }

        public override void AddOnValueChangedListener(UnityAction<string> action)
        {
            _inputField.onValueChanged.AddListener(action);
        }

        public override void RemoveOnValueChangedListener(UnityAction<string> action)
        {
            _inputField.onValueChanged.RemoveListener(action);
        }
    }
}
