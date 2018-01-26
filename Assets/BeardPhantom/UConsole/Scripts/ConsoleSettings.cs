using UnityEngine;

namespace BeardPhantom.UConsole
{
    [CreateAssetMenu(menuName = "BeardPhantom/UConsole/UConsole Settings")]
    public class ConsoleSettings : ScriptableObject
    {
        [Header("Invocation")]
        public bool StartOpen;

        public string CommandLineOpenArg = "-console";

        [Header("Colors")]
        public Color DefaultColor = Color.white;

        public Color InputEchoColor = new Color(0.75f, 0.75f, 0.75f);

        public Color ErrorColor = Color.red;

        public Color WarningColor = new Color(1f, 0.5f, 0f);

        [Header("Input")]
        public KeyCode[] InputToggle = new KeyCode[] { KeyCode.BackQuote };

        public KeyCode[] InputHistoryUp = new KeyCode[] { KeyCode.UpArrow };

        public KeyCode[] InputHistoryDown = new KeyCode[] { KeyCode.DownArrow };

        public KeyCode[] InputSubmit = new KeyCode[] { KeyCode.Return, KeyCode.KeypadEnter };
    }
}
