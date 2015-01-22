using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BSGTools.Console {
	/// <summary>
	/// The behavior of the Console.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ConsoleUI))]
	public class ConsoleController : MonoBehaviour {
		public KeyCode toggleKey = KeyCode.BackQuote;
		private List<string> cache = new List<string>();
		private int cachePointer = 0;
		public GameObject selectedObj { get; set; }
		Component _selectedComponent;
		public Component selectedComponent {
			get {
				if(selectedObj == null && _selectedComponent != null)
					_selectedComponent = null;
				return _selectedComponent;
			}
			set {
				_selectedComponent = value;
			}
		}

		ConsoleUI ui;

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

		void Awake() {
			DontDestroyOnLoad(gameObject);
			ui = GetComponent<ConsoleUI>();
			DefaultCommands.LoadIntoDatabase(this, ui);
			Hook();
		}

		void OnEnable() {
			Console.OnConsoleLog += ui.AddNewOutputLine;
			ui.onSubmitCommand += ExecuteCommand;
		}

		void OnDisable() {
			Console.OnConsoleLog -= ui.AddNewOutputLine;
			ui.onSubmitCommand -= ExecuteCommand;
		}

		void Unhook() {
			Application.RegisterLogCallback(null);
		}

		void Hook() {
			Application.RegisterLogCallback(ConsoleHook);
		}

		private void ConsoleHook(string condition, string stackTrace, LogType type) {
			switch(type) {
				case LogType.Assert:
					if(hookMode >= 3)
						Console.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace), "green");
					break;
				case LogType.Error:
					if(hookMode >= 1)
						Console.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace), "red");
					break;
				case LogType.Exception:
					if(hookMode >= 1)
						Console.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace), "red");
					break;
				case LogType.Log:
					if(hookMode >= 3)
						Console.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace));
					break;
				case LogType.Warning:
					if(hookMode >= 2)
						Console.Log(string.Format("[{0}] {1}: {2}", type.ToString(), condition, stackTrace), "orange");
					break;
			}
		}

		void Update() {
			if(Input.GetMouseButton(0))
				DoRaycast();

			if(Input.GetKeyDown(KeyCode.Escape))
				selectedObj = null;
			else if(Input.GetKeyDown(toggleKey))
				ui.ToggleConsole();
			else if(Input.GetKeyDown(KeyCode.UpArrow))
				SetFromCache(true);
			else if(Input.GetKeyDown(KeyCode.DownArrow))
				SetFromCache(false);

			if(selectedObj != null)
				ui.selected.text = string.Format("{0} (IID:{1})", selectedObj.name, selectedObj.GetInstanceID());
			else
				ui.selected.text = "";
		}

		private void DoRaycast() {
			var maincam = Camera.main;
			if(maincam.isOrthoGraphic) {
				var hit = Physics2D.Raycast(maincam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
				if(hit.collider != null) {
					selectedObj = hit.collider.gameObject;
					ui.ActivateInputField(false);
					return;
				}
			}

			RaycastHit hitInfo;
			if(Physics.Raycast(maincam.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
				selectedObj = hitInfo.collider.gameObject;
				ui.ActivateInputField(false);
			}
		}

		private void SetFromCache(bool upPressed) {
			var a = new AnimationCurve();
			if(cache.Count == 0)
				return;
			ui.SetInput(cache[cachePointer]);
			if(upPressed)
				cachePointer++;
			else
				cachePointer--;

			cachePointer = Mathf.Clamp(cachePointer, 0, cache.Count - 1);
		}

		private void ExecuteCommand(string header, string input) {
			cache.Insert(0, input);

			string[] parts = input.Split(' ');
			string command = parts[0];
			string[] args = parts.Skip(1).ToArray();

			Console.Log(header + input);
			var output = CommandDatabase.ExecuteCommand(command, args);
			if(string.IsNullOrEmpty(output) == false)
				Console.Log(output);
		}
	}
}