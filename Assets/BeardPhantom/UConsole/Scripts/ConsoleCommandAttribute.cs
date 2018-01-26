using System;

namespace BeardPhantom.UConsole
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        /// <summary>
        /// Description string
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// All other names a command can be invoked by
        /// </summary>
        public readonly string[] Aliases;

        /// <summary>
        /// Defines new metadata for a command
        /// </summary>
        /// <param name="description"></param>
        /// <param name="aliases"></param>
        public ConsoleCommandAttribute(string description, params string[] aliases)
        {
            Description = description;
            Aliases = aliases;
        }
    }
}