using System;
using System.Linq;
using System.Text;
using UnityEngine;
using BSGTools.Console;

namespace BSGTools.Console {
	public static class DefaultCommands {
		private static readonly string NO_OBJ_SELECTED = Console.Colorize("NO SELECTED OBJECT", "red");

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
					var results = ArgParser.Parse(new ParseRules(t));

					var camNameFlag = Array.IndexOf(t, "c");
					var unitFlag = Array.IndexOf(t, "u");
					var camname = "";
					var unitsahead = 5f;

					if(camNameFlag >= 0)
						camname = t[camNameFlag + 1];
					else if(unitFlag >= 0)
						unitsahead = float.Parse(t[unitFlag + 1]);

					var currentCam = (camNameFlag >= 0) ? Camera.allCameras.FirstOrDefault(c => c.name == camname) : Camera.allCameras[0];
					if(currentCam == null) {
						if(camNameFlag >= 0)
							return Console.Colorize("CAM NAME NOT FOUND", "red");
						else
							return Console.Colorize("NO CAMERAS IN SCENE", "red");
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
			CommandDatabase.RegisterCommand("Destroys the selected GameObject",
				"no args",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;

					GameObject.Destroy(cc.selectedObj);
					return "";
				}, "destroy");
			CommandDatabase.RegisterCommand("Toggles the selected GameObject",
				"no args",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;

					cc.selectedObj.SetActive(!cc.selectedObj.activeSelf);
					return "";
				}, "toggle");
			CommandDatabase.RegisterCommand("Lists all components on the selected GameObject",
				"no args",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;

					var components = cc.selectedObj.GetComponents<Component>();
					var sb = new StringBuilder();
					foreach(var c in components)
						sb.AppendLine(c.GetType().Name);
					return sb.ToString().Trim();
				}, "listc");

			CommandDatabase.RegisterCommand("Adds the component type to the selected object.",
				"addc (/n Component Type Name) [/s select after creation]",
			(t) => {
				if(cc.selectedObj == null)
					return NO_OBJ_SELECTED;

				var results = ArgParser.Parse(new ParseRules(t));

				var name = results.flagsArgs["n"];
				var select = results.flagsArgs.ContainsKey("s");

				var component = cc.selectedObj.AddComponent(name);
				if(select)
					cc.selectedComponent = component;
				return "";
			}, "addc");
			CommandDatabase.RegisterCommand("Performs actions on the selected object's rigidbody, if it exists.",
				"rb.(Component Type Name)",
			(t) => {
				if(cc.selectedObj == null)
					return NO_OBJ_SELECTED;
				var rb = cc.selectedObj.GetComponent<Rigidbody>();

				if(rb == null)
					return Console.Colorize("NO RIGIDBODY ON SELECTED OBJECT", "red");

				if(t[0].Equals("wakeup", StringComparison.CurrentCultureIgnoreCase))
					rb.WakeUp();
				else if(t[0].Equals("togglekm", StringComparison.CurrentCultureIgnoreCase))
					rb.isKinematic = !rb.isKinematic;
				else
					return Console.Colorize("INVALID ACTION: " + t[0], "red");

				return "Action " + t[0] + " executed";
			}, "rb");
			CommandDatabase.RegisterCommand("Sends a message using Unity's standard message system.",
				"sendmsg (/m message)",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;
					cc.selectedObj.SendMessage(t[0], SendMessageOptions.DontRequireReceiver);
					return "Message sent";
				}, "sendmsg");
			CommandDatabase.RegisterCommand("Sends a message upwards using Unity's standard message system.",
				"sendmsgup (/m message)",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;
					cc.selectedObj.SendMessageUpwards(t[0], SendMessageOptions.DontRequireReceiver);
					return "Message sent";
				}, "sendmsgup");
			CommandDatabase.RegisterCommand("Broadcasts a message using Unity's standard message system.",
				"bmsg (/m message)",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;
					cc.selectedObj.BroadcastMessage(t[0], SendMessageOptions.DontRequireReceiver);
					return "Message sent";
				}, "bmsg");
			CommandDatabase.RegisterCommand("Selects a component on the selected object.",
				"selectc (/t type)",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;
					var component = cc.selectedObj.GetComponent(t[0]); ;
					cc.selectedComponent = (component == null) ? cc.selectedComponent : component;
					return (component == null) ? Console.Colorize("NO COMPONENT FOUND.", "red") : "Selected component (IID:" + component.GetInstanceID() + ")";
				}, "selectc");
			CommandDatabase.RegisterCommand("Returns the selected component on the selected object.",
				"selectc (/t type)",
				(t) => {
					if(cc.selectedObj == null)
						return NO_OBJ_SELECTED;
					return (cc.selectedComponent == null) ? Console.Colorize("NO COMPONENT SELECTED.", "orange") : "Selected component (IID:" + cc.selectedComponent.GetInstanceID() + ")";
				}, "selectedc");


#if UNITY_EDITOR
			RegisterEditorCommands(cc, ui);
#endif
		}

#if UNITY_EDITOR
		private static void RegisterEditorCommands(ConsoleController cc, ConsoleUI ui) {
			CommandDatabase.RegisterCommand("EDITOR ONLY: Brings the selected object into view in the scene view.",
					"no args",
					(t) => {
						if(cc.selectedObj == null)
							return NO_OBJ_SELECTED;
						UnityEditor.Selection.activeGameObject = cc.selectedObj;
						var scene = FindSceneView();
						scene.Show(true);
						scene.Focus();
						scene.FrameSelected();
						return null;

					}, "ue.view");
			CommandDatabase.RegisterCommand("EDITOR ONLY: Sets the selected object as the Editor selected object.",
					"no args",
					(t) => {
						if(cc.selectedObj == null)
							return NO_OBJ_SELECTED;

						UnityEditor.Selection.activeGameObject = cc.selectedObj;
						return null;

					}, "ue.select");
		}

		private static UnityEditor.SceneView FindSceneView() {
			var scene = UnityEditor.SceneView.currentDrawingSceneView;
			if(scene == null)
				scene = UnityEditor.SceneView.lastActiveSceneView;
			if(scene == null && UnityEditor.SceneView.sceneViews.Count > 0)
				scene = UnityEditor.SceneView.sceneViews[0] as UnityEditor.SceneView;
			if(scene == null)
				scene = UnityEditor.SceneView.CreateInstance<UnityEditor.SceneView>();
			return scene;
		}
#endif
	}
}