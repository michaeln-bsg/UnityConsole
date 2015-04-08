using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    /// <summary>
    /// This module sets Unity's timescale to a specific value when the console is toggled.
    /// </summary>
    [AddComponentMenu("UConsole/Modules/Timescale")]
    [DisallowMultipleComponent]
    public class TimescaleModule : UConsoleModule {
        [SerializeField]
        float consoleTimescale = 0f;
        float previousTimescale = 1f;

        protected override void OnModuleActivate() {
            if(consoleTimescale < 0f) {
                consoleTimescale = 0f;
            }
            previousTimescale = DefaultCVars.timescale.fVal;
            DefaultCVars.timescale.SetValue(consoleTimescale);
        }

        protected override void OnModuleDeactivate() {
            DefaultCVars.timescale.SetValue(previousTimescale);
        }

        protected override void OnModuleRegistered() {
            consoleTogglesModule = true;
        }
    }
}