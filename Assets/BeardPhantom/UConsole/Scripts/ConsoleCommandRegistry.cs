namespace BeardPhantom.UConsole
{
    public abstract class ConsoleCommandRegistry
    {
        protected readonly Console ConsoleInstance;

        public ConsoleCommandRegistry(Console instance)
        {
            ConsoleInstance = instance;
        }
    }
}