using System.Collections.Generic;
using System.Reflection;

namespace BeardPhantom.UConsole
{
    public class CommandInfo
    {
        public readonly MethodInfo Method;

        public readonly ParameterInfo[] Parameters;

        public readonly IList<string> Aliases;

        public readonly string Description;

        public readonly int OptionalParameters;

        public readonly int RequiredParameters;

        public readonly int TotalParameters;

        public CommandInfo(MethodInfo method, IList<string> aliases, string description)
        {
            Method = method;
            Aliases = aliases;
            Description = description;
            Parameters = method.GetParameters();
            for (int i = 0; i < Parameters.Length; i++)
            {
                TotalParameters++;
                if (Parameters[i].IsOptional)
                {
                    OptionalParameters++;
                }
                else
                {
                    RequiredParameters++;
                }
            }
        }
    }
}