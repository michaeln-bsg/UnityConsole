using UnityEngine;

namespace BeardPhantom.UConsole
{
    public class DevConsole : BaseDevConsole
    {
        private CanvasGroup canvasGroup;
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
            var console = Instantiate(Resources.Load<GameObject>("DevConsole")).GetComponent<DevConsole>();
            DontDestroyOnLoad(console);
            console.SetOpen(true);
        }

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
            RegisterCommands(
                typeof(DefaultConsoleCommands),
                typeof(ImmediateModeCommands)
            );
        }
        private void Update()
        {
            UpdateInputHistory();
        }
    }
}