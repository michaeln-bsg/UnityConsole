namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Defines the help documentation for a command
    /// </summary>
    public sealed class ConsoleCommandDescriptionAttribute : CommandAttribute
    {
        public readonly string Description;

        public ConsoleCommandDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}