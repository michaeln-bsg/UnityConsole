using UnityEngine;

namespace BeardPhantom.UConsole.Modules
{
    /// <summary>
    /// Handles console activation events
    /// </summary>
    public class ActivationConsoleModule : AbstractConsoleModule
    {
        /// <summary>
        /// Previously used timescale
        /// </summary>
        private float? _cachedTimescale;

        public ActivationConsoleModule(Console console)
            : base(console) { }

        /// <inheritdoc />
        public override void Initialize()
        {
            Console.ConsoleToggled += OnConsoleToggled;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            Console.ConsoleToggled -= OnConsoleToggled;
        }

        /// <inheritdoc />
        public override void Update() { }

        /// <summary>
        /// Handle console being opened by adjusting timescale
        /// </summary>
        /// <param name="isOpen"></param>
        private void OnConsoleToggled(bool isOpen)
        {
            if (isOpen)
            {
                _cachedTimescale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = _cachedTimescale ?? Time.timeScale;
            }
        }
    }
}