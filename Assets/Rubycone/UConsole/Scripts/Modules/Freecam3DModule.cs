using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    [AddComponentMenu("UConsole/Modules/Freecam3D")]
    [DisallowMultipleComponent]
    public class Freecam3DModule : UConsoleModule {
        [SerializeField]
        Camera freecam;

        public Vector3 targetPosition;
        public Quaternion targetRotation;
        public float moveSpeed       = 0.1f;
        public float rotateSmoothing = 5f;
        public bool keypadLook       = true;

        protected override void OnModuleActivate() {
            freecam.gameObject.SetActive(true);
        }
        protected override void OnModuleDeactivate() {
            freecam.gameObject.SetActive(false);
        }
        protected override void OnModuleRegistered() {
            consoleTogglesModule = true;
        }

        //Control camera
        protected override void OnModuleUpdate() {
            ApplyDirectPositionalInput();
            CheckRotationalInput();
        }

        private void CheckRotationalInput() {
            var mx = 0f;
            var my = 0f;

            if(Input.GetMouseButton(1)) {
                mx = Input.GetAxis("Mouse X");
                my = Input.GetAxis("Mouse Y");
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if(keypadLook) {
                var kpl = 10f;
                var hkpl = kpl / 2f;
                if(Input.GetKey(KeyCode.UpArrow)) {
                    mx -= kpl;
                }
                if(Input.GetKey(KeyCode.DownArrow)) {
                    mx += kpl;
                }
                if(Input.GetKey(KeyCode.LeftArrow)) {
                    my -= kpl;
                }
                if(Input.GetKey(KeyCode.RightArrow)) {
                    my += kpl;
                }
            }

            freecam.transform.Rotate(Vector3.right, -my);
            freecam.transform.Rotate(Vector3.up, mx);
        }

        private void ApplyDirectPositionalInput() {
            var position = freecam.transform.position;
            var dtMoveSpeed = moveSpeed * Time.unscaledDeltaTime;

            if(Input.GetKey(KeyCode.D)) {
                position += freecam.transform.right * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.A)) {
                position -= freecam.transform.right * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.W)) {
                position += freecam.transform.forward * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.S)) {
                position -= freecam.transform.forward * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.E)) {
                position += freecam.transform.up * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.Q)) {
                position -= freecam.transform.up * dtMoveSpeed;
            }

            freecam.transform.position = position;
        }
    }
}