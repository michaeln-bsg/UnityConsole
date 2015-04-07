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
            if(!controller.inputHasFocus) {
                ApplyDirectPositionalInput();
                CheckRotationalInput();
                freecamObj.transform.localRotation
                    = Quaternion.Lerp(freecamObj.transform.localRotation, targetRotation, rotateSmoothing * Time.unscaledDeltaTime);
            }
        }

        private void CheckRotationalInput() {
            var mx = 0f;
            var my = 0f;

            if(Input.GetMouseButton(1)) {
                mx = Input.GetAxis("Mouse X");
                my = Input.GetAxis("Mouse Y");
            }

            if(keypadLook) {
                var kpl = 10f;
                var hkpl = kpl / 2f;
                if(Input.GetKey(KeyCode.Keypad6)) {
                    mx -= kpl;
                }
                if(Input.GetKey(KeyCode.Keypad4)) {
                    mx += kpl;
                }
                if(Input.GetKey(KeyCode.Keypad8)) {
                    my -= kpl;
                }
                if(Input.GetKey(KeyCode.Keypad2)) {
                    my += kpl;
                }

                if(Input.GetKey(KeyCode.Keypad9)) {
                    mx -= hkpl;
                    my -= hkpl;
                }
                if(Input.GetKey(KeyCode.Keypad3)) {
                    mx -= hkpl;
                    my += hkpl;
                }
                if(Input.GetKey(KeyCode.Keypad1)) {
                    mx += hkpl;
                    my += hkpl;
                }
                if(Input.GetKey(KeyCode.Keypad7)) {
                    mx += hkpl;
                    my -= hkpl;
                }
            }

            var rotation = freecamObj.transform.localRotation;
            var xQ = Quaternion.AngleAxis(mx, -Vector3.up);
            var yQ = Quaternion.AngleAxis(my, Vector3.right);
            targetRotation = rotation * xQ * yQ;
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