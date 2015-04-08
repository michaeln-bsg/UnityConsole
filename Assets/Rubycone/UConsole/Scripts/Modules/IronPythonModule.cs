using UnityEngine;
using System.Collections;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;

namespace Rubycone.UConsole.Modules {
    [AddComponentMenu("UConsole/Modules/IronPython")]
    [DisallowMultipleComponent]
    public class IronPythonModule : UConsoleModule {
        ScriptEngine engine;
        ScriptScope scope;

        protected override void OnModuleActivate() {
            if(engine == null || scope == null) {
                engine = Python.CreateEngine();
                scope = engine.CreateScope();
            }
        }
        protected override void OnModuleDeactivate() { }

        protected override void OnModuleUpdate() {
            var source = engine.CreateScriptSourceFromString("");
            source.Execute(scope);
        }

        protected override void OnModuleRegistered() {
            consoleTogglesModule = true;
        }
    }
}