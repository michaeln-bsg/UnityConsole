using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wenzil.Console {

	/// <summary>
	/// The interactive front-end of the Console.
	/// </summary>
	///
	[DisallowMultipleComponent]
	[RequireComponent(typeof(ConsoleController))]
	public class ConsoleUI : MonoBehaviour, IScrollHandler {
		public event Action<bool> onToggleConsole;
		public event Action<string, string> onSubmitCommand;

		public bool isConsoleOpen { get; private set; }

		public Scrollbar scrollbar;
		public Text outputText, selected;
		public ConsoleInputField inputField;
		public CanvasGroup togglableContent;


		[SerializeField, Range(0f, 1f)]
		private float onAlpha = 0.75f;

		void Start() {
			CloseConsole();
			inputField.OnTabEntered += inputField_OnTabEntered;
		}

		void inputField_OnTabEntered() {
			var text = inputField.userText;
			if(text.Length == 0)
				return;

			var matching = CommandDatabase.database.SelectMany(c => c.aliases).Where(a => a.StartsWith(text, StringComparison.CurrentCultureIgnoreCase));

			if(matching.Count() == 0)
				return;
			else if(matching.Count() == 1)
				SetInput(matching.First());
			else
				Console.Log(string.Join("\n", matching.ToArray()));
		}

		void OnEnable() {
			OpenConsole();
		}

		void OnDisable() {
			CloseConsole();
		}

		void Update() {
			if(inputField.isFocused && inputField.userText.Length > 0 && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
				OnEndEdit();

			//if(isConsoleOpen &&
			//EventSystem.current.currentSelectedGameObject != inputField.gameObject &&
			//EventSystem.current.currentSelectedGameObject != scrollbar.gameObject &&
			//EventSystem.current.currentSelectedGameObject != outputText.gameObject)
			//	ActivateInputField();
		}

		void OnEndEdit() {
			if(onSubmitCommand != null)
				onSubmitCommand(inputField.fieldHeader, inputField.userText);
			scrollbar.value = 0;
			ClearInput();

			// have to delay, otherwise the enter key writes a newline into the freshly cleared input field
			Invoke("ActivateInputField", 0.1f);
		}

		public void OnScroll(PointerEventData eventData) {
			scrollbar.value += eventData.scrollDelta.y;
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
			enabled = open;
			togglableContent.interactable = open;
			togglableContent.alpha = open ? onAlpha : 0f;

			ClearInput();
			if(open)
				Invoke("ActivateInputField", 0.1f); // have to delay, otherwise the toggle key is written into the input field
			else
				DeactivateInputField();

			if(onToggleConsole != null)
				onToggleConsole(open);
		}

		public void AddNewOutputLine(string line) {
			outputText.text += Environment.NewLine + line;
		}

		public void ClearOutput() {
			outputText.text = "";
		}

		public void ClearInput() {
			SetInput("");
		}

		public void SetInput(string input) {
			inputField.text = input;
			inputField.MoveTextEnd(false);
		}

		public void ActivateInputField() {
			EventSystem.current.SetSelectedGameObject(scrollbar.gameObject, null);
			EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
		}

		public void DeactivateInputField() {
			if(EventSystem.current != null) // necessary when console is being destroyed as a result of app shutdown
				EventSystem.current.SetSelectedGameObject(null, null);
		}
	}
}