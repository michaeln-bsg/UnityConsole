namespace BeardPhantom.UConsole.Modules
{
    public abstract class AbstractConsoleModule
    {
        protected readonly Console Console;

        public AbstractConsoleModule(Console console)
        {
            Console = console;
        }

        public abstract void Awake();

        public abstract void Destroy();

        public abstract void Update();
    }
}