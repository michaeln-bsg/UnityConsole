using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class Selector3DModule : UConsoleModule {

        Camera eventCamera;

        protected override void OnModuleActivate() {
            throw new System.NotImplementedException();
        }

        protected override void OnModuleUpdate() {
            RaycastHit hitInfo;
            if(Physics.Raycast(eventCamera.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
                UConsole.selectedObj = hitInfo.collider.gameObject;
                controller.ActivateInputField(false);
            }
        }

        protected override void OnModuleDeactivate() {
            throw new System.NotImplementedException();
        }
    }
}