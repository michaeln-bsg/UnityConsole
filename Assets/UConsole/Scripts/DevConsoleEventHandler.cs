using System;
using UnityEngine;

namespace BeardPhantom.UConsole
{
    public class DevConsoleEventHandler : IDevConsoleEventHandler
    {
        private event OnDevConsoleToggled ConsoleToggled;
        private event OnDevConsoleCommand ConsoleCommand;

        public void OnConsoleLogReceived(string condition, string stackTrace, LogType type)
        {
            BaseDevConsole.instance.Print(string.Format("[{0}] {1}\n{2}", type, condition, stackTrace));
        }
        public void FireConsoleToggled(bool isOpen)
        {
            if (ConsoleToggled != null)
            {
                ConsoleToggled(isOpen);
            }
        }
        public void FireConsoleCommand(DevCommandInfo info)
        {
            if (ConsoleCommand != null)
            {
                ConsoleCommand(info);
            }
        }
        public void AddRemoveCallback(Delegate del, bool add)
        {
            if (del is OnDevConsoleToggled)
            {
                var callback = del as OnDevConsoleToggled;
                ConsoleToggled -= callback;
                if (add)
                {
                    ConsoleToggled += callback;
                }
            }
            else if (del is OnDevConsoleCommand)
            {
                var callback = del as OnDevConsoleCommand;
                ConsoleCommand -= callback;
                if (add)
                {
                    ConsoleCommand += callback;
                }
            }
        }
    }
}