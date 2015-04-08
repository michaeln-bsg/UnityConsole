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
                    controller.ActivateInputField(false);
                }
            }
            if(Input.GetKeyDown(KeyCode.Escape)) {
                UConsole.selectedObj = null;
            }
        }

        private void CheckForPassthrough() {
            controller.AllowPassthrough(Input.GetKey(KeyCode.LeftControl));
        }

        protected override void OnModuleDeactivate() {
        }

        protected override void OnModuleRegistered() {
            UConsole.controller.OnToggleConsole += controller_OnToggleConsole;
        }

        void controller_OnToggleConsole(bool activated) {
            if(activated) {
                ActivateModule();
            }
            else {
                DeactivateModule();
            }
        }
    }
}