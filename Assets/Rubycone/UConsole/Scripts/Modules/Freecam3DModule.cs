using UnityEngine;
using System.Collections;

namespace Rubycone.UConsole.Modules {
    public class Freecam3DModule : UConsoleModule {
        Camera freecam;
        GameObject freecamObj;

        public Vector3 targetPosition;
        public Quaternion targetRotation;
        public float moveSpeed       = 0.1f;
        public float rotateSmoothing = 5f;
        public bool keypadLook       = true;

        protected override void OnModuleActivate() {
            freecamObj.SetActive(true);
        }
        protected override void OnModuleDeactivate() {
            freecamObj.SetActive(false);
        }
        protected override void OnModuleRegistered() {
            SetupCamera();
            controller.OnToggleConsole += controller_onToggleConsole;
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
            freecamObj = new GameObject("Freecam");
            freecamObj.transform.position = Vector3.zero;
            freecam = freecamObj.AddComponent<Camera>();
            freecam.depth = 100f;
            freecamObj.SetActive(false);
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

            freecamObj.transform.Rotate(Vector3.right, -my);
            freecamObj.transform.Rotate(Vector3.up, mx);
        }

        private void ApplyDirectPositionalInput() {
            var position = freecamObj.transform.position;
            var dtMoveSpeed = moveSpeed * Time.unscaledDeltaTime;

            if(Input.GetKey(KeyCode.D)) {
                position += freecamObj.transform.right * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.A)) {
                position -= freecamObj.transform.right * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.W)) {
                position += freecamObj.transform.forward * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.S)) {
                position -= freecamObj.transform.forward * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.E)) {
                position += freecamObj.transform.up * dtMoveSpeed;
            }
            if(Input.GetKey(KeyCode.Q)) {
                position -= freecamObj.transform.up * dtMoveSpeed;
            }

            freecamObj.transform.position = position;
        }
    }
}