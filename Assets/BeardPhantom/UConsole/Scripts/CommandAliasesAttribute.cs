namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Stores all names a command can be invoked by
    /// </summary>
    public sealed class CommandAliasesAttribute : CommandAttribute
    {
        public readonly string[] Aliases;

        public CommandAliasesAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}
