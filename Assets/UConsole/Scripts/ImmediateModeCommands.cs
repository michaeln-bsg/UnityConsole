#if DEVCONSOLE_IMMEDIATE
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
#endif

namespace BeardPhantom.UConsole
{
    public static class ImmediateModeCommands
    {
#if DEVCONSOLE_IMMEDIATE
        private const string CSHARP_ENV_CLASS = "ProcCSharpDevConsoleEnvironment";
        private const string CSHARP_ENV_METHOD = "Execute";
        private const string CSHARP_ENV_EXEC =
    @"using System;
using UnityEngine;
public class {0}
{{
    public object {1}()
    {{
        return {2};
    }}
}}";
        private const string CSHARP_ENV_EXEC_VOID =
@"using System;
using UnityEngine;
public class {0}
{{
    public void {1}()
    {{
        {2};
    }}
}}";
        private static readonly string[] IMM_ASSEMBLIES = new string[] { "UnityEngine.dll", "System.dll", "Assembly-CSharp-firstpass.dll", "Assembly-CSharp.dll" };

        private static string[] immediateAssemblies;
        private static CSharpCodeProvider csharp;
        private static bool initialized;

        private static void CreateEnvironment()
        {
            initialized = true;
            csharp = new CSharpCodeProvider(
                new Dictionary<string, string>
                {
                    { "CompilerVersion", "v2.0" }
                }
            );
            var immediateAssemblies = new List<string>();
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < allAssemblies.Length; i++)
            {
                var a = allAssemblies[i];
                if (Array.IndexOf(IMM_ASSEMBLIES, a.ManifestModule.Name) >= 0)
                {
                    immediateAssemblies.Add(a.Location);
                }
            }
            ImmediateModeCommands.immediateAssemblies = immediateAssemblies.ToArray();
        }
        private static void ExecMethod(string template, string code)
        {
            if (!initialized)
            {
                CreateEnvironment();
            }
            var source = string.Format(template, CSHARP_ENV_CLASS, CSHARP_ENV_METHOD, code);
            var compilerParams = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            compilerParams.ReferencedAssemblies.AddRange(immediateAssemblies);
            var results = csharp.CompileAssemblyFromSource(compilerParams, source);
            if (results.Errors.HasErrors)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < results.Errors.Count; i++)
                {
                    sb.AppendFormat("{0}", results.Errors[i].ErrorText);
                    sb.AppendLine();
                }
                BaseDevConsole.instance.PrintErr(sb.ToString().Trim());
                return;
            }
            var assembly = results.CompiledAssembly;
            var instance = assembly.CreateInstance(CSHARP_ENV_CLASS);
            var type = assembly.GetType(CSHARP_ENV_CLASS);
            var method = type.GetMethod(CSHARP_ENV_METHOD);
            BaseDevConsole.instance.Print(method.Invoke(instance, null));
        }
        [DevConsoleCommand("Executes code that has a return value.")]
        private static void Exec(string code)
        {
            ExecMethod(CSHARP_ENV_EXEC, code);
        }
        [DevConsoleCommand("Executes code that has no return value.", "exec_void")]
        private static void ExecVoid(string code)
        {
            ExecMethod(CSHARP_ENV_EXEC_VOID, code);
        }
#endif
    }
}