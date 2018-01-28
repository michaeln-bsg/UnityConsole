using UnityEngine;

namespace BeardPhantom.UConsole
{
    public abstract class AbstractConsoleOutputLine : MonoBehaviour
    {
        public abstract string Text { get; set; }

        public abstract Color Color { get; set; }
    }
}