using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    [AddComponentMenu("UConsole/Modules/UConsole Core")]
    [DisallowMultipleComponent]
    public class UConsoleCoreModule : UConsoleModule {

        protected override void OnModuleActivate() {
            throw new System.NotImplementedException();
        }

        protected override void OnModuleDeactivate() {
            throw new System.NotImplementedException();
        }

        protected override void OnModuleRegistered() {
            consoleTogglesModule = true;
        }
    }
}