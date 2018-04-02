using System.IO;
using UnityEngine;

namespace BeardPhantom.UConsole.Examples
{
    /// <summary>
    /// Example of creating a console, adding commands and running an auto-exec text file
    /// </summary>
    public class ExampleAppStart : MonoBehaviour
    {
        /// <summary>
        /// Console prefab
        /// </summary>
        public GameObject ConsolePrefab;

        /// <summary>
        /// Console instance
        /// </summary>
        private Console _console;

        /*
        /// <summary>
        /// Most likely you want to create your console immediately when your application starts instead
        /// of using Awake.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AppStart()
        {
            _console = ConsoleUtility.Create(ConsolePrefab);
            var commandRegistries = new[]
            {
                typeof(DefaultConsoleCommands),
            };
            _console.ResetConsole(new ConsoleSetupOptions(commandRegistries));
        }
        */

        /// <summary>
        /// Create and setup commands
        /// </summary>
        private void Awake()
        {
            _console = ConsoleUtility.Create(ConsolePrefab);

            var commandRegistries = new[]
            {
                typeof(DefaultConsoleCommands)
            };

            _console.ResetConsole(new ConsoleSetupOptions(commandRegistries));
        }

        /// <summary>
        /// Look for auto-exec script and run it through the console
        /// </summary>
        private void Start()
        {
            var path = Path.Combine(
                Application.streamingAssetsPath,
                "auto_exec.txt");

            if(File.Exists(path))
            {
                var lines = File.ReadAllLines(path);

                for(var i = 0; i < lines.Length; i++)
                {
                    _console.Commands.ExecuteCommandString(lines[i]);
                }
            }
        }
    }
}