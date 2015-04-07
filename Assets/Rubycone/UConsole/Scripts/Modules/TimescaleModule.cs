using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class TimescaleModule : UConsoleModule {
        [SerializeField]
        float consoleTimescale = 0f;
        float previousTimescale = 1f;

        protected override void OnModuleActivate() {
            if(consoleTimescale < 0f) {
                consoleTimescale = 0f;
            }
            previousTimescale = Time.timeScale;
            Time.timeScale = consoleTimescale;
        }

        protected override void OnModuleDeactivate() {
            Time.timeScale = previousTimescale;
        }

    }
}