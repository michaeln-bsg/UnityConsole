using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    [AddComponentMenu("UConsole/Modules/Selector3D")]
    [DisallowMultipleComponent]
    public class Selector3DModule : UConsoleModule {
        [SerializeField]
        Camera eventCamera;

        protected override void OnModuleActivate() { }

        protected override void OnModuleUpdate() {
            CheckForPassthrough();
            CheckForSelection();
        }

        private void CheckForSelection() {
            if(Input.GetMouseButtonDown(0)) {
                RaycastHit hitInfo;
                if(Physics.Raycast(eventCamera.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
                    UConsole.selectedObj = hitInfo.collider.gameObject;
                    UConsole.controller.ActivateInputField(false);
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape)) {
                UConsole.selectedObj = null;
            }
        }

        private void CheckForPassthrough() {
            UConsole.controller.AllowPassthrough(Input.GetKey(KeyCode.LeftControl));
        }

        protected override void OnModuleDeactivate() { }

        protected override void OnModuleRegistered() {
            consoleTogglesModule = true;
        }
    }
}