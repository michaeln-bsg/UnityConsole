using System;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Defines the help documentation for a command
    /// </summary>
    public sealed class CommandDescriptionAttribute : CommandAttribute
    {
        public readonly string Description;

        public CommandDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}