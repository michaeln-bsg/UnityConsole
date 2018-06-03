namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Defines a function as being a command
    /// </summary>
    public sealed class ConsoleCommandAttribute : CommandAttribute
    {
        public readonly string[] Aliases;

        public ConsoleCommandAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}