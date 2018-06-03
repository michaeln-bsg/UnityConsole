using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Various utility functions for UConsole
    /// </summary>
    public static class ConsoleUtility
    {
        /// <summary>
        /// Creates a new console, presumed stored in the Resources folder root as
        /// a prefab called "Console"
        /// </summary>
        /// <returns></returns>
        public static Console Create()
        {
            return Create("Console");
        }

        /// <summary>
        /// Creates a new console via a resources path
        /// </summary>
        /// <param name="resourcesPath"></param>
        /// <returns></returns>
        public static Console Create(string resourcesPath)
        {
            return Create(Resources.Load<GameObject>(resourcesPath));
        }

        /// <summary>
        /// Creates a new console via a GameObject
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static Console Create(GameObject prefab)
        {
            var console = Object.FindObjectOfType<Console>();

            if(console == null)
            {
                console = Object.Instantiate(prefab).GetComponent<Console>();
            }

            return console;
        }

        /// <summary>
        /// Checks whether any of the provided inputs were activated this frame
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool GetAnyInputDown(KeyCode[] input)
        {
            for(var i = 0; i < input.Length; i++)
            {
                if(Input.GetKeyDown(input[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Does this parameter have the params keyword?
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsParamsParameter(this ParameterInfo info)
        {
            return info.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
        }

        /// <summary>
        /// Will this parameter have its value passed in by the console system?
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool IsSpecialParameter(this ParameterInfo info)
        {
            return info.ParameterType == typeof(Console);
        }
    }
}