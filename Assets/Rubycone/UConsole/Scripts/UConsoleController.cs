using Rubycone.UConsole.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rubycone.UConsole {
    public delegate void OnToggleConsole(bool isOpen);
    public delegate void OnSubmitCommand(string alias, string argString);

    /// <summary>
    /// The behavior of the Console.
    /// </summary>
    [DisallowMultipleComponent]
    public class UConsoleController : MonoBehaviour, IScrollHandler {
        public event OnToggleConsole OnToggleConsole;
        public event OnSubmitCommand OnSubmitCommand;

        public bool isConsoleOpen { get; private set; }

        public Scrollbar scrollbar;
        public Text output, selectedObjLabel;
        public UConsoleInputField input;
        public CanvasGroup group;

        CVar scrollSpeedCVar;

        [SerializeField, Range(0f, 1f)]
        float onAlpha = 0.75f, passthroughAlpha = 0.25f;
        [SerializeField, Range(0f, 1f)]
        float scrollSpeed = 0.25f;

        public KeyCode toggleKey = KeyCode.BackQuote;

        List<string> cache = new List<string>();
        UConsoleModule[] modules;
        int cachePointer = 0;

        public bool inputHasFocus {
            get {
                return EventSystem.current.currentSelectedGameObject == input.gameObject;
            }
        }

        byte _hookMode = 0;
        public byte hookMode {
            get { return _hookMode; }
            set {
                if(_hookMode == 0 && value != 0)
                    Hook();
                _hookMode = value;
                if(value == 0)
                    Unhook();
            }
        }

        void inputField_OnTabEntered() {
            var text = input.userText;
            if(text.Length == 0)
                return;

            var matchingCmds = UConsoleDB.ccmds.Select(c => c.alias)
                .Where(a => a.StartsWith(text, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            var matchingCvars = UConsoleDB.cvars.Select(c => c.alias)
                .Where(a => a.StartsWith(text, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            if(matchingCmds.Length == 0 && matchingCvars.Length == 0)
                return;

            var totalMatching = matchingCmds.Concat(matchingCvars).OrderBy(a => a).ToArray();
            if(totalMatching.Length == 1)
                SetInput(totalMatching[0]);
            else {
                for(int i = 0; i < matchingCmds.Length; i++)
                    matchingCmds[i] = "cmd::" + matchingCmds[i];
                for(int i = 0; i < matchingCvars.Length; i++)
                    matchingCvars[i] = "cvar::" + matchingCvars[i];
                totalMatching = matchingCmds.Concat(matchingCvars).OrderBy(a => a).ToArray();
                UConsole.Log(string.Join("\n", totalMatching) + "\n");
            }
        }

        void OnEndEdit() {
            if(OnSubmitCommand != null)
                OnSubmitCommand(input.fieldHeader, input.userText);

            ActivateInputField(true);
        }

        public void OnScroll(PointerEventData eventData) {
            scrollbar.value += eventData.scrollDelta.y * scrollSpeed;
            scrollbar.value = Mathf.Clamp01(scrollbar.value);
        }

        public void OpenConsole() {
            ToggleConsole(true);
        }

        public void CloseConsole() {
            ToggleConsole(false);
        }

        public void ToggleConsole() {
            ToggleConsole(!isConsoleOpen);
        }

        public void ToggleConsole(bool open) {
            isConsoleOpen = open;
            group.interactable = open;
            group.blocksRaycasts = open;
            group.alpha = open ? onAlpha : 0f;

            ClearInput();
            if(open) {
                ActivateInputField(true);
            }
            else {
                DeselectInputField();
            }
            if(OnToggleConsole != null) {
                OnToggleConsole(open);
            }
        }

        public void AddNewOutputLine(string line) {
            output.text += Environment.NewLine + line;
            StartCoroutine(DelayedScroll(0f, 0.05f));
        }

        private IEnumerator DelayedScroll(float scrollValue, float delay) {
            scrollValue = Mathf.Clamp01(scrollValue);
            var start = Time.realtimeSinceStartup;
            while(Time.realtimeSinceStartup - start < delay) {
                yield return null;
            }
            scrollbar.value = scrollValue;
        }

        public void ClearOutput() {
            output.text = "";
        }

        public void ClearInput() {
            SetInput("");
        }

        public void SetInput(string value) {
            input.text = value;
            input.MoveTextEnd(false);
        }

        public void ActivateInputField(bool clear) {
            if(clear) {
                ClearInput();
            }
            EventSystem.current.SetSelectedGameObject(scrollbar.gameObject, null);
            EventSystem.current.SetSelectedGameObject(input.gameObject, null);
        }

        public void DeselectInputField() {
            // necessary when console is being destroyed as a result of app shutdown
            if(EventSystem.current != null) {
                EventSystem.current.SetSelectedGameObject(null, null);
            }
        }

        void Start() {
            RegisterModules();
            new CCommand("cls", "Clears the screen").CommandExecuted += (args) => {
                ClearOutput();
                return true;
            };

            Unhook();
            CloseConsole();
            input.OnTabEntered += inputField_OnTabEntered;
            DefaultCVars.scrollSpeed.CVarValueChanged += (oldValues, cvar) => {
                scrollSpeed = cvar.fVal;
            };
            DontDestroyOnLoad(this);
        }

        void OnEnable() {
            UConsole.OnConsoleLog += AddNewOutputLine;
            OnSubmitCommand += ExecuteFromInput;
            OpenConsole();
        }

        void OnDisable() {
            UConsole.OnConsoleLog -= AddNewOutputLine;
            OnSubmitCommand -= ExecuteFromInput;
            CloseConsole();
        }

        void Unhook() {
            Application.logMessageReceived -= ConsoleHook;
        }

        void Hook() {
            Application.logMessageReceived += ConsoleHook;
        }

        private void ConsoleHook(string condition, string stackTrace, LogType type) {
            switch(type) {
                case LogType.Assert:
                    if(hookMode >= 3) {
                        UConsole.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace), Color.green);
                    }
                    break;
                case LogType.Error:
                case LogType.Exception:
                    if(hookMode >= 1) {
                        UConsole.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace), Color.red);
                    }
                    break;
                case LogType.Log:
                    if(hookMode >= 3) {
                        UConsole.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace));
                    }
                    break;
                case LogType.Warning:
                    if(hookMode >= 2) {
                        UConsole.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace), UConsole.ORANGE);
                    }
                    break;
            }
        }

        void Update() {
            if(Input.GetKeyDown(toggleKey)) {
                ToggleConsole();
            }
            if(isConsoleOpen == false) {
                return;
            }

            hookMode = (byte)UConsoleDB.GetCFunc<CVar>("hookmode").iVal;

            //Begin console-open-only checks
            if(Input.GetKeyDown(KeyCode.UpArrow)) {
                SetFromCache(true);
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow)) {
                SetFromCache(false);
            }

            if(UConsole.selectedObj != null) {
                selectedObjLabel.text = string.Format("{0} (IID:{1})", UConsole.selectedObj.name, UConsole.selectedObj.GetInstanceID());
            }
            else {
                selectedObjLabel.text = "";
            }

            if(input.isFocused && input.userText.Length > 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) {
                OnEndEdit();
            }

            //if(isConsoleOpen &&
            //EventSystem.current.currentSelectedGameObject != inputField.gameObject &&
            //EventSystem.current.currentSelectedGameObject != scrollbar.gameObject &&
            //EventSystem.current.currentSelectedGameObject != outputText.gameObject)
        }

        private void SetFromCache(bool upPressed) {
            if(cache.Count == 0) {
                return;
            }
            SetInput(cache[cachePointer]);
            if(upPressed) {
                cachePointer++;
            }
            else {
                cachePointer--;
            }

            cachePointer = Mathf.Clamp(cachePointer, 0, cache.Count - 1);
        }

        private void ExecuteFromInput(string header, string input) {
            input = input.Trim();
            if(cache.Count == 0 || !cache[0].Equals(input, StringComparison.CurrentCultureIgnoreCase)) {
                cache.Insert(0, input);
            }
            UConsole.Log(header + input);
            var output = UConsoleDB.ExecuteFromInput(input);
            if(string.IsNullOrEmpty(output) == false) {
                UConsole.Log("\t" + output);
            }
        }

        void RegisterModules() {
            modules = transform.root.GetComponentsInChildren<UConsoleModule>().Where(m => m.isActiveAndEnabled).ToArray();
            foreach(var m in modules) {
                m.RegisterModule(this);
            }
        }

        void OnApplicationQuit() {
            OnToggleConsole = null;
            OnSubmitCommand = null;
        }

        internal void AllowPassthrough(bool passthrough) {
            group.blocksRaycasts = !passthrough;
            group.alpha = passthrough ? 0.25f : onAlpha;
            if(passthrough) {
                DeselectInputField();
            }
        }
    }
}