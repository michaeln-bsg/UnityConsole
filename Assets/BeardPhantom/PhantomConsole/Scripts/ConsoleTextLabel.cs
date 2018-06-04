using UnityEngine;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Abstraction for showing label in UI
    /// </summary>
    public abstract class ConsoleTextLabel : MonoBehaviour
    {
        /// <summary>
        /// Label contents
        /// </summary>
        public abstract string Text { get; set; }
    }
}