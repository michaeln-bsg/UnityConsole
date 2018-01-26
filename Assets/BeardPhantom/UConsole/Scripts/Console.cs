using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;

namespace BeardPhantom.UConsole
{
    public partial class Console : MonoBehaviour
    {
        private readonly StringComparer _cmdComparison = StringComparer.OrdinalIgnoreCase;

        [SerializeField]
        private ConsoleSettings _settings;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private Text _outputTemplate;

        [SerializeField]
        private InputField _input;

        [SerializeField]
        private RectTransform _contentRoot;

        /// <summary>
        /// 
        /// </summary>
        private byte _scrollToEndCounter;

        public CanvasGroup CanvasGroup { get; private set; }

        public bool IsOpen { get; protected set; }

        public static Console Create()
        {
            return Create("Console");
        }

        public static Console Create(string resourcesPath)
        {
            return Create(Resources.Load<GameObject>(resourcesPath));
        }

        public static Console Create(GameObject prefab)
        {
            var console = FindObjectOfType<Console>();
            if (console == null)
            {
                console = Instantiate(prefab).GetComponent<Console>();
            }
            return console;
        }

        public void SetReceiveDebugLogging(bool isRegistered)
        {
            Application.logMessageReceived -= OnApplicationLogMessageReceived;
            if (isRegistered)
            {
                Application.logMessageReceived += OnApplicationLogMessageReceived;
            }
        }

        private void OnApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Print(string.Format("[{0}] {1}\n{2}", type, condition, stackTrace));
        }

        public void Toggle()
        {
            SetOpen(!IsOpen);
        }

        public void SetOpen(bool isOpen)
        {
            if (IsOpen == isOpen)
            {
                return;
            }
            IsOpen = isOpen;
            ClearInput();
            gameObject.SetActive(isOpen);
        }

        public void ClearInput()
        {
            SetInput(string.Empty);
        }

        public CommandInfo GetCommand(string alias)
        {
            int infoIndex;
            if (CommandMap.TryGetValue(alias, out infoIndex))
            {
                return Commands[infoIndex];
            }
            return null;
        }

        protected void SubmitInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            ClearInput();
            ExecuteCommandString(input);
        }

        private bool GetInputDown(KeyCode[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Input.GetKeyDown(input[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            CanvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            IsOpen = false;
            SetOpen(Environment.CommandLine.EndsWith(_settings.CommandLineOpenArg));
            _input.onValueChanged.AddListener(OnInputValueChanged);
            _outputTemplate.gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            UpdateInputHistory();
            if (_scrollToEndCounter > 0)
            {
                _scrollToEndCounter--;
                if (_scrollToEndCounter == 0)
                {
                    _scrollRect.verticalNormalizedPosition = 0f;
                }
            }
        }

        protected void OnEnable()
        {
            SetOpen(true);
            _input.ActivateInputField();
        }

        protected void OnDisable()
        {
            SetOpen(false);
            _input.DeactivateInputField();
        }
    }
}