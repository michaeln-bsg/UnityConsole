using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class Selector2DModule : UConsoleModule {
        public Camera eventCamera;

        protected override void OnModuleActivate() {
            throw new System.NotImplementedException();
        }

        protected override void OnModuleUpdate() {
            var hit = Physics2D.Raycast(eventCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null) {
                UConsole.selectedObj = hit.collider.gameObject;
                controller.ActivateInputField(false);
                return;
            }
        }

        protected override void OnModuleDeactivate() {
            throw new System.NotImplementedException();
        }
    }
}