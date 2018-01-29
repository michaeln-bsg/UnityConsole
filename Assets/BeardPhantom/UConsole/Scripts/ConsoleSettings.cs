using UnityEngine;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Behavior and visuals settings for a console 
    /// </summary>
    [CreateAssetMenu(menuName = "BeardPhantom/UConsole/UConsole Settings")]
    public class ConsoleSettings : ScriptableObject
    {
        /// <summary>
        /// Does the console start open when created
        /// </summary>
        [Header("General")]
        public bool StartOpen;

        /// <summary>
        /// Checked at console create time to see if the application command line arguments contain
        /// this value. If true, open the console when created.
        /// </summary>
        public string CommandLineOpenArg = "-console";

        /// <summary>
        /// Color used for default print
        /// </summary>
        [Header("Colors")]
        public Color DefaultPrintColor = Color.white;

        /// <summary>
        /// Color used when showing echo of submitted input
        /// </summary>
        public Color InputEchoPrintColor = new Color(0.75f, 0.75f, 0.75f);

        /// <summary>
        /// Color used for warning print
        /// </summary>
        public Color WarningPrintColor = new Color(1f, 0.5f, 0f);

        /// <summary>
        /// Color used for error print
        /// </summary>
        public Color ErrorPrintColor = Color.red;

        /// <summary>
        /// Input checked for opening/closing the console
        /// </summary>
        [Header("Input")]
        public KeyCode[] ToggleConsole = new KeyCode[] { KeyCode.BackQuote };

        /// <summary>
        /// Input checked for navigating backwards in the console input history
        /// </summary>
        public KeyCode[] InputHistoryBackwards = new KeyCode[] { KeyCode.UpArrow };

        /// <summary>
        /// Input checked for navigating forwads in the console input history
        /// </summary>
        public KeyCode[] InputHistoryForwards = new KeyCode[] { KeyCode.DownArrow };

        /// <summary>
        /// Input checked for submitting input to the console
        /// </summary>
        public KeyCode[] SubmitInput = new KeyCode[] { KeyCode.Return, KeyCode.KeypadEnter };
    }
}
