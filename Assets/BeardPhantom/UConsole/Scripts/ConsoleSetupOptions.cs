using System;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Type data for providing functionality to a console instance
    /// </summary>
    public class ConsoleSetupOptions
    {
        /// <summary>
        /// The command registries to be registered to the console
        /// </summary>
        public readonly Type[] CommandRegistryTypes = new Type[0];

        /// <summary>
        /// The module types to be created for the console
        /// </summary>
        public readonly Type[] ModuleTypes = new Type[0];

        public ConsoleSetupOptions(
            Type[] commandRegistryTypes = null,
            Type[] moduleTypes = null)
        {
            CommandRegistryTypes = commandRegistryTypes ?? new Type[0];
            ModuleTypes = moduleTypes ?? new Type[0];
        }
    }
}