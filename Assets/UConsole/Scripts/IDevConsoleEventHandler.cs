using UnityEngine;
using System.Collections;
using System;

namespace BeardPhantom.UConsole
{
    public delegate void OnDevConsoleToggled(bool isOpen);
    public delegate void OnDevConsoleCommand(DevCommandInfo info);

    public interface IDevConsoleEventHandler
    {
        void OnConsoleLogReceived(string condition, string stackTrace, LogType type);
        void AddRemoveCallback(Delegate del, bool add);
        void FireConsoleToggled(bool isOpen);
        void FireConsoleCommand(DevCommandInfo info);
    }
}