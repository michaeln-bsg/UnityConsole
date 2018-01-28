using UnityEngine;

namespace BeardPhantom.UConsole.Examples
{
    public class ExampleAppStart : MonoBehaviour
    {
        public GameObject ConsolePrefab;

        private Console _console;

        private void Awake()
        {
            _console = ConsoleUtility.Create(ConsolePrefab);
            _console.Setup(new ConsoleSetupOptions(new[] { typeof(DefaultConsoleCommands) }));

            // Probably the safer place to execute commands via script
            //var path = Path.Combine(Application.streamingAssetsPath, "auto_exec.txt");
            //if (File.Exists(path))
            //{
            //    var lines = File.ReadAllLines(path);
            //    for (var i = 0; i < lines.Length; i++)
            //    {
            //        _consoleInstance.ExecuteCommandString(lines[i]);
            //    }
            //}
        }
    }
}