﻿using System.Text;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    public class DefaultConsoleCommands : ConsoleCommandRegistry
    {
        private const string PREFS_KEY_NOT_FOUND = "PREFSKEY NOT FOUND";

        private readonly StringBuilder output = new StringBuilder();

        public DefaultConsoleCommands(Console instance)
            : base(instance) { }

        [ConsoleCommand("Shows help info about a command.", "?", "Cmds")]
        private string Help(string cmdString = "")
        {
            output.Length = 0;
            if (string.IsNullOrEmpty(cmdString))
            {
                var cmds = ConsoleInstance.Commands;
                for (int i = 0; i < cmds.Count; i++)
                {
                    var cmd = cmds[i];
                    output.AppendLine(string.Join(",", cmd.Metadata.Aliases));
                }
            }
            else
            {
                var cmd = ConsoleInstance.GetCommand(cmdString);
                if (cmd != null)
                {
                    output.AppendFormat("{0}: {1}", string.Join("/", cmd.Metadata.Aliases), cmd.Metadata.Description);
                }
                else
                {
                    output.AppendFormat("COMMAND NOT FOUND: {0}", cmdString);
                }
            }
            return output.ToString();
        }

        [ConsoleCommand("Quits the game.", "Quit", "QQQ")]
        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }

        [ConsoleCommand("Clears the console.", "CLS")]
        private void Clear()
        {
            ConsoleInstance.ClearOutput();
        }

        [ConsoleCommand("Does a console print test.")]
        private string ConsoleTest(int length = 1000)
        {
            const string RNDCHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = RNDCHARS[Random.Range(0, RNDCHARS.Length - 1)];
            }
            return new string(stringChars);
        }

        [ConsoleCommand("Returns a string from PlayerPrefs.")]
        private string PrefsGet(string key)
        {
            return PlayerPrefs.GetString(key, PREFS_KEY_NOT_FOUND);
        }

        [ConsoleCommand("Returns an integer from PlayerPrefs.")]
        private string PrefsGetI(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key).ToString();
            }
            return PREFS_KEY_NOT_FOUND;
        }

        [ConsoleCommand("Returns a float from PlayerPrefs.")]
        private string PrefsGetF(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key).ToString();
            }
            return PREFS_KEY_NOT_FOUND;
        }

        [ConsoleCommand("Sets a string in PlayerPrefs.")]
        private void PrefsSet(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        [ConsoleCommand("Sets an integer in PlayerPrefs.")]
        private void PrefsSetI(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        [ConsoleCommand("Sets a float in PlayerPrefs.")]
        private void PrefsSetF(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        [ConsoleCommand("Removes a value from PlayerPrefs.")]
        private void PrefsDel(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        [ConsoleCommand("Removes a value from PlayerPrefs.")]
        private void PrefsDelAll()
        {
            PlayerPrefs.DeleteAll();
            ConsoleInstance.Print("PlayerPrefs.DeleteAll Success", Color.green);
        }
    }
}