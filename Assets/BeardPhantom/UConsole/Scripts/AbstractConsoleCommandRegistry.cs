namespace BeardPhantom.UConsole
{
    public abstract class AbstractConsoleCommandRegistry
    {
        protected readonly Console ConsoleInstance;

        public AbstractConsoleCommandRegistry(Console instance)
        {
            ConsoleInstance = instance;
        }
    }
}