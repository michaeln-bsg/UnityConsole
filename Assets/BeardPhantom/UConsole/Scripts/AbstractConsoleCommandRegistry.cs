namespace BeardPhantom.UConsole
{
    /// <summary>
    /// An instance of functions that can be executed via the console
    /// </summary>
    public abstract class AbstractConsoleCommandRegistry
    {
        /// <summary>
        /// The current console instance
        /// </summary>
        protected readonly Console Console;

        public AbstractConsoleCommandRegistry(Console instance)
        {
            Console = instance;
        }
    }
}