using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class Selector3DModule : UConsoleModule {

        Camera eventCamera;

        protected override void OnModuleActivate() { }

        protected override void OnModuleUpdate() {
            CheckForPassthrough();
            CheckForSelection();
        }

        private void CheckForSelection() {
            if(eventCamera == null) {
                eventCamera = Camera.main;
            }
            if(eventCamera == null && Camera.allCamerasCount > 0) {
                eventCamera = Camera.allCameras[0];
            }
            if(Input.GetMouseButtonDown(1)) {
                RaycastHit hitInfo;
                if(Physics.Raycast(eventCamera.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
                    UConsole.selectedObj = hitInfo.collider.gameObject;
                    controller.ActivateInputField(false);
                }
            }
            else if(Input.GetKeyDown(KeyCode.Escape)) {
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