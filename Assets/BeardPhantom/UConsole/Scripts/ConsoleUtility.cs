using UnityEngine;

namespace BeardPhantom.UConsole
{
    public static class ConsoleUtility
    {
        public static Console Create()
        {
            return Create("Console");
        }

        public static Console Create(string resourcesPath)
        {
            return Create(Resources.Load<GameObject>(resourcesPath));
        }

        public static Console Create(GameObject prefab)
        {
            var console = Object.FindObjectOfType<Console>();
            if (console == null)
            {
                console = Object.Instantiate(prefab).GetComponent<Console>();
            }
            return console;
        }

        public static bool GetInputDown(KeyCode[] input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (Input.GetKeyDown(input[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
