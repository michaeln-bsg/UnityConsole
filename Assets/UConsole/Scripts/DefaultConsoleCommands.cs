using System.Text;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    public static class DefaultConsoleCommands
    {
        private const string PREFS_KEY_NOT_FOUND = "PREFSKEY NOT FOUND";

        private static readonly StringBuilder output = new StringBuilder();

        [DevConsoleCommand("Shows help info about a command", "?", "cmds")]
        private static string Help(string cmdString = "")
        {
            output.Length = 0;
            if (string.IsNullOrEmpty(cmdString))
            {
                var cmds = BaseDevConsole.instance.commands;
                for (int i = 0; i < cmds.Count; i++)
                {
                    var cmd = cmds[i];
                    output.AppendLine(string.Join(",", cmd.metadata.aliases));
                }
            }
            else
            {
                var cmd = BaseDevConsole.instance.GetCommand(cmdString);
                if (cmd != null)
                {
                    output.AppendFormat("{0}: {1}", string.Join("/", cmd.metadata.aliases), cmd.metadata.description);
                }
                else
                {
                    output.AppendFormat("COMMAND NOT FOUND: {0}", cmdString);
                }
            }
            return output.ToString();
        }
        [DevConsoleCommand("Quits the game", "quit", "qqq")]
        private static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }
        [DevConsoleCommand("Clears the console", "cls")]
        private static void Clear()
        {
            BaseDevConsole.instance.ClearOutput();
        }
        [DevConsoleCommand("Does a console print test")]
        private static string Console_Test(int length = 1000)
        {
            const string RNDCHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = RNDCHARS[Random.Range(0, RNDCHARS.Length - 1)];
            }
            return new string(stringChars);
        }
        private static string PrefsGet(string key)
        {
            return PlayerPrefs.GetString(key, PREFS_KEY_NOT_FOUND);
        }
        private static string PrefsGetI(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key).ToString();
            }
            return PREFS_KEY_NOT_FOUND;
        }
        private static string PrefsGetF(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key).ToString();
            }
            return PREFS_KEY_NOT_FOUND;
        }
        private static void PrefsSet(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        private static void PrefsSetI(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        private static void PrefsSetF(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
    }
}