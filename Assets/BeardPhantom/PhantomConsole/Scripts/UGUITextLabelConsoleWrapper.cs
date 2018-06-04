using UnityEngine;
using UnityEngine.UI;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Wrapper for UGUI Text component for PhantomConsole
    /// </summary>
    public class UGUITextLabelConsoleWrapper : ConsoleTextLabel
    {
        [SerializeField]
        private Text _text;

        public override string Text
        {
            get { return _text.text; }
            set { _text.text = value; }
        }
    }
}