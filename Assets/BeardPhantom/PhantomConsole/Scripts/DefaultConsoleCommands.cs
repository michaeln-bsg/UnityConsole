using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Provides game agnostic commands
    /// </summary>
    public static class DefaultConsoleCommands
    {
        /// <summary>
        /// Error for when a PlayerPrefs key cannot be found
        /// </summary>
        private const string PREFS_KEY_NOT_FOUND = "PREFSKEY NOT FOUND";

        /// <summary>
        /// Used for outputting text and creating less garbage
        /// </summary>
        private static readonly StringBuilder _output = new StringBuilder();

        [ConsoleCommand("help", "?", "cmds")]
        [ConsoleCommandDescription("Shows help info about a command, or prints all commands")]
        private static string Help(Console console, string cmdString = "")
        {
            _output.Length = 0;

            if(string.IsNullOrEmpty(cmdString))
            {
                var cmds = console.Commands.CommandList;

                for(var i = 0; i < cmds.Count; i++)
                {
                    var cmd = cmds[i];
                    _output.AppendLine(string.Join(",", cmd.Aliases.ToArray()));
                }
            }
            else
            {
                var cmd = console.Commands.GetCommand(cmdString);

                if(cmd != null)
                {
                    _output.AppendFormat("{0}: {1}", string.Join("/", cmd.Aliases.ToArray()), cmd.Description);
                    if(cmd.TotalProvidableParameters > 0)
                    {
                        _output.AppendLine();
                        _output.Append('(');
                        for(var i = 0; i < cmd.Parameters.Length; i++)
                        {
                            var parameter = cmd.Parameters[i];

                            if(parameter.IsSpecialParameter())
                            {
                                continue;
                            }

                            if(parameter.IsParamsParameter())
                            {
                                _output.Append("params ");
                            }

                            _output.AppendFormat("{0} ", parameter.ParameterType.Name);
                            _output.Append(parameter.Name);

                            if(parameter.IsOptional)
                            {
                                _output.AppendFormat(" = {0}", parameter.DefaultValue);
                            }

                            if(i < cmd.Parameters.Length - 1)
                            {
                                _output.Append(", ");
                            }
                        }
                        _output.Append(')');
                    }
                }
                else
                {
                    _output.AppendFormat("COMMAND NOT FOUND: {0}", cmdString);
                }
            }

            return _output.ToString();
        }

        [ConsoleCommand("set_console_transparancy", "set_console_alpha", "console_alpha")]
        [ConsoleCommandDescription("Adjusts the console's transparancy")]
        private static void SetConsoleTransparancy(Console console, float alpha)
        {
            console.CanvasGroup.alpha = Mathf.Clamp(alpha, 0.3f, 1f);
        }

        [ConsoleCommand("quit_game", "quit", "qqq")]
        [ConsoleCommandDescription("Quits the game.")]
        private static void QuitGame()
        {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
		    Application.Quit();
                        #endif
        }

        [ConsoleCommand("clear", "cls")]
        [ConsoleCommandDescription("Clears the console output window.")]
        private static void Clear(Console console)
        {
            console.InputOutput.ClearOutput();
        }

        [ConsoleCommand("console_test")]
        [ConsoleCommandDescription("Performs a console print test.")]
        private static string ConsoleTest(int length = 1000)
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

        [ConsoleCommand("console_test2")]
        [ConsoleCommandDescription("Performs a console print test.")]
        private static string ConsoleTest2(params string[] echos)
        {
            return string.Join(",", echos);
        }

        [ConsoleCommand("prefs_get")]
        [ConsoleCommandDescription("Returns a string from PlayerPrefs.")]
        private static string PrefsGet(string key)
        {
            return PlayerPrefs.GetString(key, PREFS_KEY_NOT_FOUND);
        }

        [ConsoleCommand("prefs_get_int")]
        [ConsoleCommandDescription("Returns an integer from PlayerPrefs.")]
        private static string PrefsGetInt(string key)
        {
            if(PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetInt(key).ToString();
            }

            return PREFS_KEY_NOT_FOUND;
        }

        [ConsoleCommand("prefs_get_float")]
        [ConsoleCommandDescription("Returns a float from PlayerPrefs.")]
        private static string PrefsGetFloat(string key)
        {
            if(PlayerPrefs.HasKey(key))
            {
                return PlayerPrefs.GetFloat(key).ToString();
            }

            return PREFS_KEY_NOT_FOUND;
        }

        [ConsoleCommand("prefs_set")]
        [ConsoleCommandDescription("Sets a string in PlayerPrefs.")]
        private static void PrefsSet(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        [ConsoleCommand("prefs_set_int")]
        [ConsoleCommandDescription("Sets an integer in PlayerPrefs.")]
        private static void PrefsSetIn(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        [ConsoleCommand("prefs_set_float")]
        [ConsoleCommandDescription("Sets a float in PlayerPrefs.")]
        private static void PrefsSetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        [ConsoleCommand("prefs_del")]
        [ConsoleCommandDescription("Removes a value from PlayerPrefs.")]
        private static void PrefsDelete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        [ConsoleCommand("prefs_del_all")]
        [ConsoleCommandDescription("Removes a value from PlayerPrefs.")]
        private static void PrefsDeleteAll(Console console)
        {
            PlayerPrefs.DeleteAll();

            console.InputOutput.Print(
                "PlayerPrefs.DeleteAll Success",
                Color.green);
        }
    }
}