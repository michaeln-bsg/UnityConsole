using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class Freecam2DModule : UConsoleModule {
        Camera freecam2D;
        GameObject freecam2DObj;

        public float moveSpeed = 0.1f;

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
            freecam2D.orthographic = true;

            freecam2DObj.SetActive(false);

            if(GetComponent<Selector2DModule>() != null) {
                GetComponent<Selector2DModule>().eventCamera = freecam2D;
            }
        }

        //Control camera
        protected override void OnModuleUpdate() {
            if(!controller.inputHasFocus) {
                ApplyDirectPositionalInput();
                ApplyZooming();
            }
        }

        private void ApplyZooming() {
            var zoom = freecam2D.orthographicSize;
            zoom += Input.GetAxis("Mouse ScrollWheel") * Time.unscaledDeltaTime;
            if(Input.GetKey(KeyCode.Minus)) {
                zoom += 1f * Time.unscaledDeltaTime;
            }
            if(Input.GetKey(KeyCode.Equals)) {
                zoom -= 1f * Time.unscaledDeltaTime;
            }
            zoom = zoom <= 0f ? 0.001f : zoom;
            freecam2D.orthographicSize = zoom;
        }

        private void ApplyDirectPositionalInput() {
            var mx = 0f;
            var my = 0f;
            var boostMultiplier = Input.GetKey(KeyCode.LeftShift) ? 3f : 1f;
            if(Input.GetMouseButton(1)) {
                mx = Input.GetAxis("Mouse X");
                my = Input.GetAxis("Mouse Y");
            }
            var kpl = 50f;
            if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
                mx += kpl;
            }
            if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
                mx -= kpl;
            }
            if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
                my += kpl;
            }
            if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
                my -= kpl;
            }

            var position = freecam2DObj.transform.position;
            var dtMoveSpeed = moveSpeed * Time.unscaledDeltaTime;

            position.x += mx * boostMultiplier * dtMoveSpeed;
            position.y += my * boostMultiplier * dtMoveSpeed;

            freecam2DObj.transform.position = position;
        }
    }
}