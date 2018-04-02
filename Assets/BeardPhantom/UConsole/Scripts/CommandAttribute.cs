using System;

namespace BeardPhantom.UConsole
{
    /// <summary>
    /// Base class for defining a console command metadata attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class CommandAttribute : Attribute { }
}