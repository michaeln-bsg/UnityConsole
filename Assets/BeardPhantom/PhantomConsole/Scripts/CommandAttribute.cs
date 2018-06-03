using System;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Base class for defining a console command metadata attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public abstract class CommandAttribute : Attribute { }
}