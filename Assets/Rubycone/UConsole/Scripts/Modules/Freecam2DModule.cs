using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    [AddComponentMenu("UConsole/Modules/Freecam2D")]
    [DisallowMultipleComponent]
    public class Freecam2DModule : UConsoleModule {
        [SerializeField]
        Camera freecam2D;

        public float moveSpeed = 0.1f;

        protected override void OnModuleActivate() {
            freecam2D.gameObject.SetActive(true);
        }
        protected override void OnModuleDeactivate() {
            freecam2D.gameObject.SetActive(false);
        }
        protected override void OnModuleRegistered() {
            consoleTogglesModule = true;
        }

        //Control camera
        protected override void OnModuleUpdate() {
            if(!UConsole.controller.inputHasFocus) {
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

            var position = freecam2D.transform.position;
            var dtMoveSpeed = moveSpeed * Time.unscaledDeltaTime;

            position.x += mx * boostMultiplier * dtMoveSpeed;
            position.y += my * boostMultiplier * dtMoveSpeed;

            freecam2D.transform.position = position;
        }
    }
}