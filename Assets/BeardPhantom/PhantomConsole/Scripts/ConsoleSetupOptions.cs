using System;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Type data for providing functionality to a console instance
    /// </summary>
    public class ConsoleSetupOptions
    {
        /// <summary>
        /// Whether default commands should be registered
        /// </summary>
        public readonly bool AddDefaultCommands;

        /// <summary>
        /// The module types to be created for the console
        /// </summary>
        public readonly Type[] ModuleTypes;

        public ConsoleSetupOptions(bool addDefaultCommands, params Type[] moduleTypes)
        {
            AddDefaultCommands = addDefaultCommands;
            ModuleTypes = moduleTypes ?? new Type[0];
        }
    }
}