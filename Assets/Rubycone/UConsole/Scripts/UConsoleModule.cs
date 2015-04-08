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
        bool _consoleTogglesModule = false;
        public bool consoleTogglesModule {
            get {
                return _consoleTogglesModule;
            }
            protected set {
                _consoleTogglesModule = value;
                if(value) {
                    UConsole.controller.OnToggleConsole += controller_OnToggleConsole;
                }
                else {
                    UConsole.controller.OnToggleConsole -= controller_OnToggleConsole;
                }
            }
        }

        void controller_OnToggleConsole(bool isOpen) {
            if(isOpen) {
                ActivateModule();
            }
            else {
                DeactivateModule();
            }
        }

        protected abstract void OnModuleActivate();
        protected abstract void OnModuleDeactivate();

        protected virtual void OnModuleUpdate() { }
        protected virtual void OnModuleRegistered() { }

        bool shuttingDown = false;

        public void ActivateModule() {
            if(state == ModuleState.Deactivated && enabled) {
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

        void OnDisable() {
            if(!shuttingDown) {
                DeactivateModule();
            }
        }

        void OnApplicationQuit() {
            shuttingDown = true;
        }

        void Update() {
            if(state == ModuleState.Activated && enabled) {
                state = ModuleState.Updating;
                OnModuleUpdate();
                state = ModuleState.Activated;
            }
        }

        public void RegisterModule() {
            OnModuleRegistered();
            state = ModuleState.Deactivated;
        }
    }
}