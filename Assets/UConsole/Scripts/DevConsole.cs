using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace BeardPhantom.UConsole
{
    public class DevConsole : BaseDevConsole
    {
        [SerializeField]
        private Scrollbar scrollbar;
        [SerializeField]
        private Text outputTemplate;
        [SerializeField]
        private InputField input;

        private CanvasGroup canvasGroup;
        private Regex emptyInputRegex = new Regex(@"^\s*\n+$", RegexOptions.Compiled);
        private byte scrollToEndCounter;
        private readonly DevConsoleEventHandler _eventHandler = new DevConsoleEventHandler();

        public override IDevConsoleEventHandler eventHandler
        {
            get
            {
                return _eventHandler;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {

            var console = FindObjectOfType<BaseDevConsole>();
            if (console == null)
            {
                console = Instantiate(Resources.Load<GameObject>("DevConsole")).GetComponent<BaseDevConsole>();
            }
            DontDestroyOnLoad(console);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            console.SetOpen(true);
#else
            console.SetOpen(Environment.CommandLine.EndsWith("-console"));
#endif
        }

        public override void SetInput(string value)
        {
            input.text = value;
            input.caretPosition = input.text.Length;
        }
        public override void ClearOutput()
        {
            var parent = outputTemplate.transform.parent;
            outputTemplate.transform.SetAsFirstSibling();
            for (int i = parent.childCount - 1; i > 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
        protected override void PrintInternal(string text, Color color)
        {
            var instance = Instantiate(outputTemplate, outputTemplate.transform.parent);
            instance.text = text.Trim();
            instance.color = color;
            instance.gameObject.SetActive(true);
            scrollToEndCounter = 2;
        }
        protected override void Awake()
        {
            base.Awake();
            input.onValueChanged.AddListener(OnInputValueChanged);
            outputTemplate.gameObject.SetActive(false);
            canvasGroup = GetComponent<CanvasGroup>();
            RegisterCommands(
                typeof(DefaultConsoleCommands),
                typeof(ImmediateModeCommands)
            );
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            input.ActivateInputField();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            input.DeactivateInputField();
        }
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
        private void Update()
        {
            UpdateInputHistory();
            if (scrollToEndCounter > 0)
            {
                scrollToEndCounter--;
                if (scrollToEndCounter == 0)
                {
                    scrollbar.value = 0f;
                }
            }
        }
        private void OnInputValueChanged(string value)
        {
            if (emptyInputRegex.IsMatch(value))
            {
                // Remove strings that are just whitespace + newlines
                input.text = string.Empty;
            }
            else if (value.EndsWith("\n"))
            {
                SubmitInput(value.Replace("\n", string.Empty).Trim());
            }
        }
    }
}