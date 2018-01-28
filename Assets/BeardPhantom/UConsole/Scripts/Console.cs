using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using BeardPhantom.UConsole.Modules;

namespace BeardPhantom.UConsole
{
    public class Console : MonoBehaviour
    {
        #region Fields

        private readonly Dictionary<Type, AbstractConsoleModule> _customModules
            = new Dictionary<Type, AbstractConsoleModule>();

        public event Action<ConsoleSetupOptions> ConsoleReset;

        [SerializeField]
        private ConsoleSettings _settings;

        [SerializeField]
        private AbstractConsoleInputField _inputField;

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private AbstractConsoleOutputLine _outputLinePrefab;

        [SerializeField]
        private RectTransform _consoleRect;

        private Canvas _canvas;

        private bool _isOpen;

        #endregion

        #region Properties

        public ConsoleSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public AbstractConsoleInputField InputField
        {
            get
            {
                return _inputField;
            }
        }

        public ScrollRect ScrollRect
        {
            get
            {
                return _scrollRect;
            }
        }

        public AbstractConsoleOutputLine OutputLinePrefab
        {
            get
            {
                return _outputLinePrefab;
            }
        }

        public InputOutputConsoleModule InputOutput { get; private set; }

        public CommandConsoleModule Commands { get; private set; }

        public InputHistoryConsoleModule InputHistory { get; private set; }

        public CanvasGroup CanvasGroup { get; private set; }

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                _canvas.enabled = value;
                InputOutput.ClearInput();
                InputField.IsSelected = true;
            }
        }

        #endregion

        #region Methods

        public T GetModule<T>() where T : AbstractConsoleModule
        {
            AbstractConsoleModule value;
            _customModules.TryGetValue(typeof(T), out value);
            return (T)value;
        }

        public void Setup(ConsoleSetupOptions options)
        {
            // Setup modules
            foreach (var m in _customModules)
            {
                m.Value.Destroy();
            }
            _customModules.Clear();

            // Add in default modules
            InputOutput = new InputOutputConsoleModule(this);
            InputHistory = new InputHistoryConsoleModule(this);
            Commands = new CommandConsoleModule(this);

            AddModule(InputOutput);
            AddModule(InputHistory);
            AddModule(Commands);

            foreach (var t in options.ModuleTypes)
            {
                var instance = (AbstractConsoleModule)Activator.CreateInstance(t, (object)this);
                AddModule(instance);
            }

            if (ConsoleReset != null)
            {
                ConsoleReset(options);
            }
        }

        private void AddModule(AbstractConsoleModule module)
        {
            _customModules.Add(module.GetType(), module);
            module.Awake();
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            CanvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            IsOpen = Environment.CommandLine.EndsWith(_settings.CommandLineOpenArg);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            if (ConsoleUtility.GetInputDown(_settings.InputToggle))
            {
                IsOpen = !IsOpen;
            }
            if (IsOpen)
            {
                foreach (var m in _customModules.Values)
                {
                    m.Update();
                }
            }
        }

        #endregion
    }
}