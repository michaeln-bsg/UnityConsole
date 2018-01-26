using System.IO;
using UnityEngine;

namespace BeardPhantom.UConsole.Examples
{
    public class ExampleAppStart : MonoBehaviour
    {
        public GameObject ConsolePrefab;

        private Console _consoleInstance;

        private void Awake()
        {
            _consoleInstance = Console.Create(ConsolePrefab);
            _consoleInstance.SetCommands(typeof(DefaultConsoleCommands));

            // Probably the safer place to execute commands via script
            //var path = Path.Combine(Application.streamingAssetsPath, "auto_exec.txt");
            //if (File.Exists(path))
            //{
            //    var lines = File.ReadAllLines(path);
            //    for (int i = 0; i < lines.Length; i++)
            //    {
            //        _consoleInstance.ExecuteCommandString(lines[i]);
            //    }
            //}
        }
    }
}