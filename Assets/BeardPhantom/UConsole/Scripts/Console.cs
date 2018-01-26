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
        private Scrollbar _scrollbar;

        [SerializeField]
        private Text _outputTemplate;

        [SerializeField]
        private InputField _input;

        private RectTransform _root;

        /// <summary>
        /// 
        /// </summary>
        private byte _scrollToEndCounter;

        public CanvasGroup CanvasGroup { get; private set; }

        public bool IsOpen { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var console = FindObjectOfType<Console>();
            if (console == null)
            {
                console = Instantiate(Resources.Load<GameObject>("DevConsole")).GetComponent<Console>();
            }
            DontDestroyOnLoad(console);
            console.SetOpen(Environment.CommandLine.EndsWith("-console"));
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            // Probably the safer place to execute commands via script
            var path = Path.Combine(Application.streamingAssetsPath, "auto_exec.txt");
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    ExecuteCommandString(lines[i]);
                }
            }
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
                    _scrollbar.value = 0f;
                }
            }
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

        public virtual void Toggle()
        {
            SetOpen(!IsOpen);
        }

        public virtual void SetOpen(bool isOpen)
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

        public DevCommandInfo GetCommand(string alias)
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
            IsOpen = false;
            _root = transform.GetChild(0) as RectTransform;

            _input.onValueChanged.AddListener(OnInputValueChanged);
            _outputTemplate.gameObject.SetActive(false);
            CanvasGroup = GetComponent<CanvasGroup>();
            SetCommands(typeof(DefaultConsoleCommands));
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