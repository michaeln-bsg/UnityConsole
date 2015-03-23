using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public abstract class UConsoleModule : MonoBehaviour {
        public enum ModuleState {
            Unregistered = 0,
            Deactivated = 1,
            Activated = 2,
            Updating = 4
        }

        public ModuleState state { get; private set; }
        protected UConsoleController controller { get; private set; }

        protected abstract void OnModuleActivate();
        protected abstract void OnModuleDeactivate();

        protected virtual void OnModuleUpdate() { }
        protected virtual void OnModuleRegistered() { }

        public void ActivateModule() {
            if(state == ModuleState.Deactivated) {
                OnModuleActivate();
                state = ModuleState.Activated;
            }
        }

        public void DeactivateModule() {
            if(state == ModuleState.Activated || state == ModuleState.Updating) {
                OnModuleDeactivate();
                state = ModuleState.Deactivated;
            }
        }

        void Update() {
            if(state == ModuleState.Activated) {
                state = ModuleState.Updating;
                OnModuleUpdate();
                state = ModuleState.Activated;
            }
        }

        public void RegisterModule(UConsoleController controller) {
            this.controller = controller;
            OnModuleRegistered();
            state = ModuleState.Activated;
        }
    }
}