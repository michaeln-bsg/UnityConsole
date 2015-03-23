using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class Selector2DModule : UConsoleModule {
        public Camera eventCamera;

        protected override void OnModuleActivate() {
            if(eventCamera == null) {
                eventCamera = Camera.main;
            }
        }

        protected override void OnModuleRegistered() {
            controller.onToggleConsole += controller_onToggleConsole;
        }

        void controller_onToggleConsole(bool obj) {
            if(obj) {
                ActivateModule();
            }
            else {
                DeactivateModule();
            }
        }

        protected override void OnModuleUpdate() {
            CheckForPassthrough();
            CheckForSelection();
        }

        private void CheckForPassthrough() {
            controller.AllowPassthrough(Input.GetKey(KeyCode.LeftControl));
        }

        private void CheckForSelection() {
            if(Input.GetMouseButtonDown(0)) {
                var hit = Physics2D.Raycast(eventCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if(hit.collider != null) {
                    UConsole.selectedObj = hit.collider.gameObject;
                    controller.selectedObjLabel.text = UConsole.selectedObj.name + " " + UConsole.selectedObj.GetInstanceID();
                    controller.ActivateInputField(false);
                    return;
                }
            }

            //Do deselection
            else if(Input.GetKeyDown(KeyCode.Escape)) {
                UConsole.selectedObj = null;
            }
        }

        protected override void OnModuleDeactivate() { }
    }
}