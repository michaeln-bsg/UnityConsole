namespace BeardPhantom.PhantomConsole.Modules
{
    /// <summary>
    /// Represents a block of console functionality.
    /// </summary>
    public abstract class AbstractConsoleModule
    {
        /// <summary>
        /// Console instance
        /// </summary>
        protected readonly Console Console;

        public AbstractConsoleModule(Console console)
        {
            Console = console;
        }

        /// <summary>
        /// Initialization of this module
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Called before module is removed from the console
        /// </summary>
        public abstract void Destroy();

        /// <summary>
        /// Called every frame by the console if the console is opened
        /// </summary>
        public abstract void Update();
    }
}