using System;

namespace BeardPhantom.UConsole
{
    public class ConsoleSetupOptions
    {
        public readonly Type[] CommandRegistryTypes = new Type[0];

        public readonly Type[] ModuleTypes = new Type[0];

        public ConsoleSetupOptions(Type[] commandRegistryTypes = null, Type[] moduleTypes = null)
        {
            CommandRegistryTypes = commandRegistryTypes ?? new Type[0];
            ModuleTypes = moduleTypes ?? new Type[0];
        }
    }
}
