using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Wenzil.Console;

namespace Wenzil.Console {
	public static class DefaultCommands {

		public static void LoadIntoDatabase(ConsoleController cc, ConsoleUI ui) {
			CommandDatabase.RegisterCommand("Clears the screen", "No args",
				(t) => {
					ui.ClearOutput();
					return null;
				}, "clear", "cls");
			CommandDatabase.RegisterCommand("Quits the game", "No args",
				(t) => {
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
					return "!!! QUITTING APPLICATION !!!";
				}, "quit", "exit", "qqq");
			CommandDatabase.RegisterCommand("Shows this message, or help on a specific command.", "help [command]",
				(t) => {
					var sb = new StringBuilder();
					if(t.Length > 0) {
						var command = CommandDatabase.GetCommand(t[0]);
						sb.AppendLine(string.Join(", ", command.aliases).Trim());
						sb.AppendLine(command.description);
						sb.AppendLine("Usage: " + command.usage);
					}
					else {
						var color = false;
						foreach(var c in CommandDatabase.database) {
							if(color)
								sb.Append("<color=teal>");
							sb.AppendLine(string.Join(", ", c.aliases).Trim());
							sb.AppendLine(c.description);
							if(color)
								sb.AppendLine("Usage: " + c.usage + "</color>");
							else
								sb.AppendLine("Usage: " + c.usage);
							color = !color;
						}
					}
					return sb.ToString().Trim();
				}, "help", "?");
			CommandDatabase.RegisterCommand("Modifies the hook that displays Unity's debug console's output here. " +
				"Acceptable modes are 0 (off), 1 (errors & exceptions), 2 (1 + warnings), 3 (all)", "hookmode [mode]",
				(t) => {
					byte mode = byte.Parse(t[0]);
					cc.hookMode = mode;
					return "Hook mode set to " + t[0];
				}, "hookmode");
			CommandDatabase.RegisterCommand("Spawns a physics sphere in front of the given or active camera.",
				"physball [/c cameraname] [/u units_ahead def:5]",
				(t) => {
					var results = ArgParser.Parse(new ParseRules(string.Join(" ", t), true, '/'));

					var camNameFlag = Array.IndexOf(t, "/c");
					var unitFlag = Array.IndexOf(t, "/u");
					var camname = "";
					var unitsahead = 5f;

					if(camNameFlag >= 0)
						camname = t[camNameFlag + 1];
					else if(unitFlag >= 0)
						unitsahead = float.Parse(t[unitFlag + 1]);

					var currentCam = (camNameFlag >= 0) ? Camera.allCameras.FirstOrDefault(c => c.name == camname) : Camera.allCameras[0];
					if(currentCam == null) {
						if(camNameFlag >= 0)
							return "<color=red>CAM NAME NOT FOUND</color>";
						else
							return "<color=red>NO CAMERAS IN SCENE</color>";
					}

					var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					sphere.AddComponent<Rigidbody>();
					var position = currentCam.transform.forward * unitsahead;
					sphere.transform.position = position;
					return "Physball spawned at " + position.ToString();
				}, "physball");
			CommandDatabase.RegisterCommand("Adjusts timescale",
				"timescale [value]",
				(t) => {
					Time.timeScale = int.Parse(t[0]);
					return "";
				}, "timescale", "ts");
			CommandDatabase.RegisterCommand("Destroy's the selected GameObject",
				"no args",
				(t) => {
					if(cc.selected == null)
						return Console.Colorize("NO SELECTED OBJECT", "red");
					GameObject.Destroy(cc.selected);
					return "";
				}, "destroy");
			CommandDatabase.RegisterCommand("Toggles the selected GameObject",
				"no args",
				(t) => {
					if(cc.selected == null)
						return Console.Colorize("NO SELECTED OBJECT", "red");
					cc.selected.SetActive(!cc.selected.activeSelf);
					return "";
				}, "toggle");
			CommandDatabase.RegisterCommand("Lists all components on the selected GameObject",
				"no args",
				(t) => {
					if(cc.selected == null)
						return Console.Colorize("NO SELECTED OBJECT", "red");
					var components = cc.selected.GetComponents<Component>();
					var sb = new StringBuilder();
					foreach(var c in components)
						sb.AppendLine(c.GetType().Name);
					return sb.ToString().Trim();
				}, "listc");

#if UNITY_EDITOR
			RegisterEditorCommands(cc, ui);
#endif

		}

#if UNITY_EDITOR
		private static void RegisterEditorCommands(ConsoleController cc, ConsoleUI ui) {
			CommandDatabase.RegisterCommand("EDITOR ONLY: Brings the selected object into view in the scene view.",
					"no args",
					(t) => {
						if(cc.selected == null)
							return Console.Colorize("NO SELECTED OBJECT", "red");

						var scene = FindSceneView();
						scene.Focus();
						scene.MoveToView(cc.selected.transform);
						return null;

					}, "ue.view");
			CommandDatabase.RegisterCommand("EDITOR ONLY: Sets the selected object as the Editor selected object.",
					"no args",
					(t) => {
						if(cc.selected == null)
							return Console.Colorize("NO SELECTED OBJECT", "red");

						UnityEditor.Selection.activeGameObject = cc.selected;
						return null;

					}, "ue.select");
		}

		private static UnityEditor.SceneView FindSceneView() {
			var scene = UnityEditor.SceneView.currentDrawingSceneView;
			if(scene == null)
				scene = UnityEditor.SceneView.lastActiveSceneView;
			if(scene == null)
				scene = UnityEditor.SceneView.sceneViews[0] as UnityEditor.SceneView;
			return scene;
		}
#endif
	}
}