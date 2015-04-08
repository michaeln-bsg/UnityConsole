using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    /// <summary>
    /// This module is used to select GameObjects with 2D colliders.
    /// </summary>
    [AddComponentMenu("UConsole/Modules/Selector2D")]
    [DisallowMultipleComponent]
    public class Selector2DModule : UConsoleModule {
        [SerializeField]
        Camera eventCamera;

        protected override void OnModuleActivate() { }

        protected override void OnModuleRegistered() {
            consoleTogglesModule = true;
        }

        protected override void OnModuleUpdate() {
            CheckForPassthrough();
            CheckForSelection();
        }

        private void CheckForPassthrough() {
            UConsole.controller.AllowPassthrough(Input.GetKey(KeyCode.LeftControl));
        }

        private void CheckForSelection() {
            if(Input.GetMouseButtonDown(0)) {
                var hit = Physics2D.Raycast(eventCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if(hit.collider != null) {
                    UConsole.selectedObj = hit.collider.gameObject;
                    UConsole.controller.selectedObjLabel.text = UConsole.selectedObj.name + " " + UConsole.selectedObj.GetInstanceID();
                    UConsole.controller.ActivateInputField(false);
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