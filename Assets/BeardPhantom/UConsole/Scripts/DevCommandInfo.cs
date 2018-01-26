using System.Reflection;

namespace BeardPhantom.UConsole
{
    public class DevCommandInfo
    {
        public readonly MethodInfo Method;

        public readonly ConsoleCommandAttribute Metadata;

        public readonly ParameterInfo[] Parameters;

        public readonly int OptionalParameters;

        public readonly int RequiredParameters;

        public readonly int TotalParameters;

        public DevCommandInfo(MethodInfo method, ConsoleCommandAttribute metadata)
        {
            this.Method = method;
            this.Metadata = metadata;
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