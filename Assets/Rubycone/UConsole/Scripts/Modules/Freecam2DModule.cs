using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class Freecam2DModule : UConsoleModule {
        Camera freecam2D;
        GameObject freecam2DObj;

        public Vector3 targetPosition;
        public float moveSpeed       = 0.1f;

        protected override void OnModuleActivate() {
            freecam2DObj.SetActive(true);
        }
        protected override void OnModuleDeactivate() {
            freecam2DObj.SetActive(false);
        }
        protected override void OnModuleRegistered() {
            SetupCamera();
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

        private void SetupCamera() {
            freecam2DObj = new GameObject("Freecam2D");
            freecam2DObj.transform.position = Vector3.zero;
            freecam2D = freecam2DObj.AddComponent<Camera>();
            freecam2D.depth = 100f;
            freecam2DObj.SetActive(false);
        }

        //Control camera
        protected override void OnModuleUpdate() {
            if(Input.GetMouseButton(1)) {
                ApplyDirectPositionalInput();
            }
        }

        private void ApplyDirectPositionalInput() {
            var mx = Input.GetAxis("Mouse X");
            var my = Input.GetAxis("Mouse Y");
            var position = freecam2DObj.transform.position;
            var dtMoveSpeed = moveSpeed * Time.unscaledDeltaTime;

            position.x += mx * dtMoveSpeed;
            position.y += my * dtMoveSpeed;

            freecam2DObj.transform.position = position;
        }
    }
}