using UnityEngine;
using UnityEngine.UI;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// A wrapper class to provide UConsole UGUI Text objects
    /// </summary>
    public class UGUIOutputLineWrapper : AbstractConsoleOutputLine
    {
        [SerializeField]
        private Text _text;

        /// <inheritdoc />
        public override string Text
        {
            get
            {
                return _text.text;
            }
            set
            {
                _text.text = value;
            }
        }

        /// <inheritdoc />
        public override Color Color
        {
            get
            {
                return _text.color;
            }
            set
            {
                _text.color = value;
            }
        }

        /// <summary>
        /// Look for text component on object when script is added
        /// </summary>
        private void Reset()
        {
            _text = GetComponent<Text>();
        }
    }
}