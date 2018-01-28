using UnityEngine;
using UnityEngine.UI;

namespace BeardPhantom.UConsole
{
    public class UGUIOutputLineWrapper : AbstractConsoleOutputLine
    {
        [SerializeField]
        private Text _text;

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
    }
}