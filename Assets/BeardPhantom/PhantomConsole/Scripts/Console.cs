using System;
using System.Collections.Generic;
using BeardPhantom.PhantomConsole.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace BeardPhantom.PhantomConsole
{
    /// <summary>
    /// Core class for the console instance
    /// </summary>
    public class Console : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// Type-to-instance mapping of custom loaded modules
        /// </summary>
        private readonly Dictionary<Type, ConsoleModule> _customModules
            = new Dictionary<Type, ConsoleModule>();

        /// <summary>
        /// Event invoked when console is toggled open or closed
        /// </summary>
        public event Action<bool> ConsoleToggled;

        /// <summary>
        /// Event invoked when console is reset
        /// </summary>
        public event Action<ConsoleSetupOptions> ConsoleReset;

        /// <summary>
        /// Behavior and visual settings used by this console instance
        /// </summary>
        [SerializeField]
        private ConsoleSettings _settings;

        /// <summary>
        /// Input field
        /// </summary>
        [SerializeField]
        private ConsoleInputField _inputField;

        /// <summary>
        /// Output window scroll rect
        /// </summary>
        [SerializeField]
        private ScrollRect _scrollRect;

        /// <summary>
        /// Output line prefab for showing output
        /// </summary>
        [SerializeField]
        private ConsoleOutputLine _outputLinePrefab;

        /// <summary>
        /// Window for displaying auto complete results
        /// </summary>
        [SerializeField]
        private AutoCompleteWindow _autoCompleteWindow;

        /// <summary>
        /// The main container RectTransform for the console
        /// </summary>
        [SerializeField]
        private RectTransform _consoleRect;

        /// <summary>
        /// Whether the console is currently opened
        /// </summary>
        private bool _isOpen;

        #endregion

        #region Properties

        /// <summary>
        /// Default input-output module
        /// </summary>
        public InputOutputConsoleModule InputOutput { get; private set; }

        /// <summary>
        /// Default command module
        /// </summary>
        public CommandConsoleModule Commands { get; private set; }

        /// <summary>
        /// Default input history module
        /// </summary>
        public InputHistoryConsoleModule InputHistory { get; private set; }

        /// <summary>
        /// Default auto complete module
        /// </summary>
        public AutoCompleteConsoleModule AutoComplete { get; private set; }

        /// <summary>
        /// CanvasGroup attached to the console
        /// </summary>
        public CanvasGroup CanvasGroup { get; private set; }

        /// <summary>
        /// Module accessor for settings
        /// </summary>
        public ConsoleSettings Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// Module accessor for input field
        /// </summary>
        public ConsoleInputField InputField
        {
            get { return _inputField; }
        }

        /// <summary>
        /// Module accessor for output window scroll rect
        /// </summary>
        public ScrollRect ScrollRect
        {
            get { return _scrollRect; }
        }

        /// <summary>
        /// Results for auto complete window
        /// </summary>
        public AutoCompleteWindow AutoCompleteWindow
        {
            get { return _autoCompleteWindow; }
            set { _autoCompleteWindow = value; }
        }

        /// <summary>
        /// Module accessor for output prefab
        /// </summary>
        public ConsoleOutputLine OutputLinePrefab
        {
            get { return _outputLinePrefab; }
        }

        /// <summary>
        /// Whether the console is currently open
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                CanvasGroup.alpha = value ? 1f : 0f;
                CanvasGroup.blocksRaycasts = value;
                InputOutput.ClearInput();
                InputField.IsSelected = true;

                if(ConsoleToggled != null)
                {
                    ConsoleToggled(_isOpen);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a module by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModule<T>() where T : ConsoleModule
        {
            ConsoleModule value;
            _customModules.TryGetValue(typeof(T), out value);

            return (T) value;
        }

        /// <summary>
        /// Completely clears console configuration
        /// </summary>
        /// <param name="options"></param>
        public void ResetConsole(ConsoleSetupOptions options)
        {
            // Setup modules
            foreach(var m in _customModules)
            {
                m.Value.Destroy();
            }

            _customModules.Clear();

            // Add in default modules
            InputOutput = new InputOutputConsoleModule(this);
            InputHistory = new InputHistoryConsoleModule(this);
            Commands = new CommandConsoleModule(this);
            AutoComplete = new AutoCompleteConsoleModule(this);

            AddModules(InputOutput, InputHistory, Commands, AutoComplete);

            foreach(var t in options.ModuleTypes)
            {
                var instance = (ConsoleModule) Activator.CreateInstance(t, (object) this);
                AddModule(instance);
            }

            foreach(var m in _customModules)
            {
                m.Value.Initialize();
            }

            if(ConsoleReset != null)
            {
                ConsoleReset(options);
            }
        }

        /// <summary>
        /// Internal function for adding modules
        /// </summary>
        /// <param name="modules"></param>
        private void AddModules(params ConsoleModule[] modules)
        {
            foreach(var module in modules)
            {
                AddModule(module);
            }
        }

        /// <summary>
        /// Adds a module to modules list
        /// </summary>
        /// <param name="module"></param>
        private void AddModule(ConsoleModule module)
        {
            _customModules.Add(module.GetType(), module);
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            CanvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        /// <summary>
        /// Try to open the console if the command line string is set
        /// </summary>
        private void Start()
        {
            IsOpen = _settings.StartOpen
                || !string.IsNullOrEmpty(_settings.CommandLineOpenArg)
                && Environment.CommandLine.EndsWith(_settings.CommandLineOpenArg);
        }

        /// <summary>
        /// Update modules
        /// </summary>
        private void Update()
        {
            if(ConsoleUtility.GetAnyInputDown(_settings.ToggleConsole))
            {
                IsOpen = !IsOpen;
            }

            if(IsOpen)
            {
                foreach(var m in _customModules.Values)
                {
                    if(m.Enabled)
                    {
                        m.Update();
                    }
                }
            }
        }

        #endregion
    }
}