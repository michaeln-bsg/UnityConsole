
using UnityEngine;

namespace Rubycone.UConsole {
    public delegate void OnConsoleLog(string line);
    public static class UConsole {
        public static event OnConsoleLog OnConsoleLog;

        public static UConsoleController controller { get; private set; }

        public static GameObject selectedObj { get; set; }

        static Component _selectedComponent;
        public static Component selectedComponent {
            get {
                if(selectedObj == null && _selectedComponent != null) {
                    _selectedComponent = null;
                }
                return _selectedComponent;
            }
            set {
                _selectedComponent = value;
            }
        }

        public static readonly Color ORANGE = new Color(255, 127, 0);

        public static void Create(GameObject consolePrefab) {
            if(controller == null) {
                var console = GameObject.Instantiate<GameObject>(consolePrefab);
                controller = console.GetComponentInChildren<UConsoleController>();
            }
        }

        public static void Create(string prefabResource) {
            if(controller == null) {
                Create(Resources.Load<GameObject>(prefabResource));
            }
        }

        public static void Log(string line) {
            if(OnConsoleLog != null) {
                OnConsoleLog(line);
            }
        }

        public static void Log(string line, Color32 color) {
            if(OnConsoleLog != null) {
                OnConsoleLog(Colorize(line, color));
            }
        }

        public static void LogErr(string line) {
            if(OnConsoleLog != null) {
                OnConsoleLog(ColorizeErr(line));
            }
        }

        public static void LogSuccess(string line) {
            if(OnConsoleLog != null) {
                OnConsoleLog(ColorizeSuccess(line));
            }
        }

        public static void LogWarn(string line) {
            if(OnConsoleLog != null) {
                OnConsoleLog(ColorizeWarn(line));
            }
        }

        public static string Colorize(string str, Color32 color) {
            return string.Format("<color={0}>{1}</color>", ColorToHex(color), str);
        }

        public static string ColorizeErr(string str) {
            return Colorize(str, Color.red);
        }

        public static string ColorizeWarn(string str) {
            return Colorize(str, ORANGE);
        }

        public static string ColorizeSuccess(string str) {
            return Colorize(str, Color.green);
        }

        static string ColorToHex(Color32 color) {
            return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        }
    }
}