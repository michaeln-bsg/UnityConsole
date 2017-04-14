using System.Text;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    public static class DefaultConsoleCommands
    {
        private const string PREFS_KEY_NOT_FOUND = "PREFSKEY NOT FOUND";

        private static readonly StringBuilder output = new StringBuilder();
        private static string prefsDeleteAllPassword;

        [DevConsoleCommand("Shows help info about a command.", "?", "Cmds")]
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
        [DevConsoleCommand("Quits the game.", "Quit", "QQQ")]
        private static void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }
        [DevConsoleCommand("Clears the console.", "CLS")]
        private static void Clear()
        {
            BaseDevConsole.instance.ClearOutput();
        }
        [DevConsoleCommand("Does a console print test.")]
        private static string ConsoleTest(int length = 1000)
        {
            const string RNDCHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = RNDCHARS[Random.Range(0, RNDCHARS.Length - 1)];
            }
            return new string(stringChars);
        }
        [DevConsoleCommand("Returns a string from PlayerPrefs.")]
        private static string PrefsGet(string key)
        {
            return PlayerPrefs.GetString(key, PREFS_KEY_NOT_FOUND);
        }
        [DevConsoleCommand("Returns an integer from PlayerPrefs.")]
        private static string PrefsGetI(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key).ToString();
            }
            return PREFS_KEY_NOT_FOUND;
        }
        [DevConsoleCommand("Returns a float from PlayerPrefs.")]
        private static string PrefsGetF(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key).ToString();
            }
            return PREFS_KEY_NOT_FOUND;
        }
        [DevConsoleCommand("Sets a string in PlayerPrefs.")]
        private static void PrefsSet(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        [DevConsoleCommand("Sets an integer in PlayerPrefs.")]
        private static void PrefsSetI(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
        [DevConsoleCommand("Sets a float in PlayerPrefs.")]
        private static void PrefsSetF(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        [DevConsoleCommand("Removes a value from PlayerPrefs.")]
        private static void PrefsDel(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
        [DevConsoleCommand("Removes a value from PlayerPrefs.")]
        private static void PrefsDelAll(string password = "")
        {
            var printPass = true;
            output.Length = 0;
            if (password == string.Empty || prefsDeleteAllPassword == string.Empty)
            {
                const string PASSCHARS = "abcdefg123456789";
                for (int i = 0; i < 5; i++)
                {
                    output.Append(PASSCHARS[Random.Range(0, PASSCHARS.Length)]);
                }
                prefsDeleteAllPassword = output.ToString();
            }
            else if (password == prefsDeleteAllPassword)
            {
                PlayerPrefs.DeleteAll();
                BaseDevConsole.instance.Print("PlayerPrefs.DeleteAll Success", Color.green);
                printPass = false;
                prefsDeleteAllPassword = string.Empty;
            }
            else
            {
                BaseDevConsole.instance.PrintErr("INVALID PASSWORD");
            }
            if (printPass)
            {
                BaseDevConsole.instance.PrintWarn(string.Format("Please confirm by resubmitting command with password: {0}", prefsDeleteAllPassword));
            }
        }
    }
}