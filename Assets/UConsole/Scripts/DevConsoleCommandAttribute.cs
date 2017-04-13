using System;

namespace BeardPhantom.UConsole
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class DevConsoleCommandAttribute : Attribute
    {
        public string description;
        public string[] aliases;

        public DevConsoleCommandAttribute(string description, params string[] aliases)
        {
            this.description = description;
            this.aliases = aliases;
        }
    }
}