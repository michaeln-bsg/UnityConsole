using System.Reflection;

namespace BeardPhantom.UConsole
{
    public class DevCommandInfo
    {
        public MethodInfo method;
        public DevConsoleCommandAttribute metadata;
        public ParameterInfo[] parameters;
        public readonly int optionalParameters;
        public readonly int requiredParameters;
        public readonly int totalParameters;

        public DevCommandInfo(MethodInfo method, DevConsoleCommandAttribute metadata)
        {
            this.method = method;
            this.metadata = metadata;
            parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                totalParameters++;
                if (parameters[i].IsOptional)
                {
                    optionalParameters++;
                }
                else
                {
                    requiredParameters++;
                }
            }
        }
    }
}