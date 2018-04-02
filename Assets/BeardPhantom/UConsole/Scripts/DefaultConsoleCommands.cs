using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Provides game agnostic commands
    /// </summary>
    public class DefaultConsoleCommands : AbstractConsoleCommandRegistry
    {
        /// <summary>
        /// Error for when a PlayerPrefs key cannot be found
        /// </summary>
        private const string PREFS_KEY_NOT_FOUND = "PREFSKEY NOT FOUND";

        /// <summary>
        /// Used for outputting text and creating less garbage
        /// </summary>
        private readonly StringBuilder output = new StringBuilder();

        /// <inheritdoc />
        public DefaultConsoleCommands(Console instance)
            : base(instance) { }

        [CommandAliases("?", "cmds")]
        [CommandDescription(
            "Shows help info about a command, or prints all commands")]
        private string help(string cmdString = "")
        {
            output.Length = 0;

            if(string.IsNullOrEmpty(cmdString))
            {
                var cmds = Console.Commands.CommandList;

                for(var i = 0; i < cmds.Count; i++)
                {
                    var cmd = cmds[i];
                    output.AppendLine(string.Join(",", cmd.Aliases.ToArray()));
                }
            }
            else
            {
                var cmd = Console.Commands.GetCommand(cmdString);

                if(cmd != null)
                {
                    output.AppendFormat(
                        "{0}: {1}",
                        string.Join("/", cmd.Aliases.ToArray()),
                        cmd.Description);
                }
                else
                {
                    output.AppendFormat("COMMAND NOT FOUND: {0}", cmdString);
                }
            }

            return output.ToString();
        }

        [CommandAliases("set_console_alpha", "console_alpha")]
        [CommandDescription("Adjusts the console's transparancy")]
        private void set_console_transparancy(float alpha)
        {
            Console.CanvasGroup.alpha = Mathf.Clamp(alpha, 0.3f, 1f);
        }

        [CommandAliases("quit", "qqq")]
        [CommandDescription("Quits the game.")]
        private void quit_game()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
		    Application.Quit();
                        #endif
        }

        [CommandAliases("cls")]
        [CommandDescription("Clears the console output window.")]
        private void clear()
        {
            Console.InputOutput.ClearOutput();
        }

        [CommandDescription("Performs a console print test.")]
        private string console_test(int length = 1000)
        {
            const string RNDCHARS =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var stringChars = new char[length];

            for(var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = RNDCHARS[Random.Range(0, RNDCHARS.Length - 1)];
            }

            return new string(stringChars);
        }

        [CommandDescription("Returns a string from PlayerPrefs.")]
        private string prefs_get(string key)
        {
            return PlayerPrefs.GetString(key, PREFS_KEY_NOT_FOUND);
        }

        [CommandDescription("Returns an integer from PlayerPrefs.")]
        private string prefs_get_i(string key)
        {
            if(PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key).ToString();
            }

            return PREFS_KEY_NOT_FOUND;
        }

        [CommandDescription("Returns a float from PlayerPrefs.")]
        private string prefs_get_f(string key)
        {
            if(PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key).ToString();
            }

            return PREFS_KEY_NOT_FOUND;
        }

        [CommandDescription("Sets a string in PlayerPrefs.")]
        private void prefs_set(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        [CommandDescription("Sets an integer in PlayerPrefs.")]
        private void prefs_set_i(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        [CommandDescription("Sets a float in PlayerPrefs.")]
        private void prefs_set_f(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        [CommandDescription("Removes a value from PlayerPrefs.")]
        private void prefs_del(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        [CommandDescription("Removes a value from PlayerPrefs.")]
        private void prefs_del_all()
        {
            PlayerPrefs.DeleteAll();

            Console.InputOutput.Print(
                "PlayerPrefs.DeleteAll Success",
                Color.green);
        }
    }
}