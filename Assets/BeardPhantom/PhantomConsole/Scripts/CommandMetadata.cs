using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BeardPhantom.PhantomConsole
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
        public readonly string[] Aliases;

        /// <summary>
        /// Help documentation for this command
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// Total parameter count
        /// </summary>
        public readonly int TotalParameters;

        /// <summary>
        /// Count of required parameters
        /// </summary>
        public readonly int RequiredParameters;

        /// <summary>
        /// Count of optional parameters
        /// </summary>
        public readonly int OptionalParameters;

        /// <summary>
        /// Total parameter count
        /// </summary>
        public readonly int TotalProvidableParameters;

        /// <summary>
        /// Number of parameters that can be provided (ie not including special parameters)
        /// </summary>
        public readonly int ProvidableRequiredParameters;

        /// <summary>
        /// Number of parameters that can be provided (ie not including special parameters)
        /// </summary>
        public readonly int ProvidableOptionalParameters;

        public CommandMetadata(MethodInfo method, IEnumerable<string> aliases, string description)
        {
            Method = method;
            Aliases = aliases.ToArray();
            Description = description;
            Parameters = method.GetParameters();

            for(var i = 0; i < Parameters.Length; i++)
            {
                var isSpecial = Parameters[i].IsSpecialParameter();

                TotalParameters++;
                if(!isSpecial)
                {
                    TotalProvidableParameters++;
                }

                if (Parameters[i].IsOptional || Parameters[i].IsParamsParameter())
                {
                    OptionalParameters++;
                    if (!isSpecial)
                    {
                        ProvidableOptionalParameters++;
                    }
                }
                else
                {
                    RequiredParameters++;
                    if (!isSpecial)
                    {
                        ProvidableRequiredParameters++;
                    }
                }
            }
        }
    }
}