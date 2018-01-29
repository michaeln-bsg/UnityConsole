using System.Collections.Generic;
using System.Reflection;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Defines invocation metadata for a command
    /// </summary>
    public class CommandMetadata
    {
        /// <summary>
        /// Reflection data for the function
        /// </summary>
        public readonly MethodInfo Method;

        /// <summary>
        /// Cached parameter data
        /// </summary>
        public readonly ParameterInfo[] Parameters;

        /// <summary>
        /// All names this command can be invoked by
        /// </summary>
        public readonly IList<string> Aliases;

        /// <summary>
        /// Help documentation for this command
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Count of optional parameters
        /// </summary>
        public readonly int OptionalParameters;

        /// <summary>
        /// Count of required parameters
        /// </summary>
        public readonly int RequiredParameters;

        /// <summary>
        /// Total parameter count
        /// </summary>
        public readonly int TotalParameters;

        public CommandMetadata(MethodInfo method, IList<string> aliases, string description)
        {
            Method = method;
            Aliases = aliases;
            Description = description;
            Parameters = method.GetParameters();
            for (var i = 0; i < Parameters.Length; i++)
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