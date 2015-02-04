using System;
using System.Linq;
using System.Text;
using UnityEngine;
using BSGTools.Console;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BSGTools.Console {
	[RequireComponent(typeof(UConsoleController))]
	public class DefaultCommands : MonoBehaviour {
		private static readonly string NO_OBJ_SELECTED_ERR = UConsole.ColorizeErr("NO SELECTED OBJECT");

		void Start() {
			var cc = GetComponent<UConsoleController>();
			MetaCommands(cc);
			UnityCommands(cc);
			RBActions(cc);

#if UNITY_EDITOR
			RegisterEditorCommands(cc);
#endif
		}

		void UnityCommands(UConsoleController cc) {
			new CCMD("quit", "exit", "qqq")
				.SetCallback((sb, t) => {
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#else
					Application.Quit();
#endif
					return sb.Append("!!! QUITTING APPLICATION !!!");
				})
				.SetDescription("Quits the game")
				.SetUsage("noargs");

			new CCMD("destroy")
				.SetCallback((sb, t) => {
					GameObject.Destroy(cc.selectedObj);
					return sb.Append("Object destroyed.");
				})
				.SetRequireSelectedGameObj()
				.SetDescription("Destroys the selected GameObject");

			new CCMD("toggle")
				.SetCallback((sb, t) => {
					cc.selectedObj.SetActive(!cc.selectedObj.activeSelf);
					return sb.Append("Object active == " + cc.selectedObj.activeSelf);
				})
				.SetDescription("Toggles the selected GameObject")
				.SetRequireSelectedGameObj();

			new CCMD("listc")
				.SetCallback((sb, t) => {
					if(cc.selectedObj == null)
						return sb.Append(NO_OBJ_SELECTED_ERR);

					var components = cc.selectedObj.GetComponents<Component>();
					for(int i = 0;i < components.Length;i++) {
						var c = components[i];
						sb.AppendLine(string.Format("{0}) {1}", i + 1, c.GetType().Name));
					}
					return sb;
				})
				.SetRequireSelectedGameObj()
				.SetDescription("Lists all components on the selected GameObject");

			new CCMD("addc", "addcomponent")
				.SetCommandType(CommandType.MultiArgsRequired)
				.SetCallback((sb, t) => {
					if(cc.selectedObj == null)
						return sb.Append(NO_OBJ_SELECTED_ERR);
					var name = string.Empty;
					if(t.multiArgs.TryGetValue("n", out name) == false)
						sb.Append(UConsole.ColorizeErr("COMPONENT NAME REQUIRED"));
					var select = t.multiArgs.ContainsKey("s");

					var component = cc.selectedObj.AddComponent(name);
					if(select)
						cc.selectedComponent = component;
					return sb.Append("Component creation success.");
				})
				.SetRequireSelectedGameObj()
				.SetDescription("Adds the component type to the selected object.")
				.SetUsage("addc (/n Component Type Name) [/s select after creation]");

			new CCMD("sendmsg")
				.SetDescription("Sends a message using Unity's standard message system.")
				.SetUsage("sendmsg (message)")
				.SetCommandType(CommandType.SingleArgRequired)
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					if(cc.selectedObj == null)
						return sb.Append(NO_OBJ_SELECTED_ERR);
					cc.selectedObj.SendMessage(t.singleArg, SendMessageOptions.DontRequireReceiver);
					return sb.Append("Message sent");
				});

			new CCMD("sendmsgup")
				.SetDescription("Sends a message upwards using Unity's standard message system.")
				.SetUsage("sendmsgup (message)")
				.SetCommandType(CommandType.SingleArgRequired)
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					cc.selectedObj.SendMessageUpwards(t.singleArg, SendMessageOptions.DontRequireReceiver);
					return sb.Append("Message sent");
				});

			new CCMD("bmsg")
				.SetDescription("Broadcasts a message using Unity's standard message system.")
				.SetUsage("bmsg (message)")
				.SetCommandType(CommandType.SingleArgRequired)
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					cc.selectedObj.BroadcastMessage(t.singleArg, SendMessageOptions.DontRequireReceiver);
					return sb.Append("Message sent");
				});

			new CCMD("selectc")
				.SetDescription("Selects a component on the selected object.")
					.SetCommandType(CommandType.SingleArgRequired)
				.SetUsage("selectc (typestr)")
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					var component = cc.selectedObj.GetComponent(t.singleArg);
					cc.selectedComponent = (component == null) ? cc.selectedComponent : component;
					return sb.Append((component == null) ? UConsole.ColorizeErr("NO COMPONENT FOUND.") : "Selected component (IID:" + component.GetInstanceID() + ")");
				});

			new CCMD("selectedc")
				.SetDescription("Returns the selected component on the selected object.")
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					return sb.Append((cc.selectedComponent == null) ? UConsole.ColorizeWarn("NO COMPONENT SELECTED.") : "Selected component (IID:" + cc.selectedComponent.GetInstanceID() + ")");
				});
		}

		void MetaCommands(UConsoleController cc) {
			new CCMD("cls", "clear")
				.SetCallback((sb, t) => {
					cc.ClearOutput();
					return null;
				})
				.SetDescription("Clears the screen")
				.SetUsage("noargs");

			new CCMD("help", "?")
				.SetCommandType(CommandType.SingleArgOptional)
				.SetCallback((sb, t) => {
					if(t != null) {
						var command = UConsoleDB.GetCommand(t.singleArg);
						sb.AppendLine(string.Join(", ", command.aliases).Trim());
						sb.AppendLine("\t" + command.description);
						sb.AppendLine("\t\tusage: " + command.usage);
					}
					else {
						var color = false;
						foreach(var c in UConsoleDB.ccmds) {
							var innerSB = new StringBuilder();
							innerSB.AppendLine(string.Join(", ", c.aliases).Trim());
							innerSB.AppendLine("\t" + c.description);
							innerSB.AppendLine("\tusage: " + c.usage);
							if(color)
								sb.Append(UConsole.Colorize(innerSB.ToString(), Color.cyan));
							else
								sb.Append(innerSB.ToString());
							color = !color;
						}
					}
					return sb;
				});
			new CCMD("convars", "cvars")
				.SetCallback((sb, t) => {
					foreach(var cvar in UConsoleDB.cvars.OrderBy(c => c.aliases[0]))
						sb.AppendLine(string.Join(",", cvar.aliases));
					return sb;
				});
			new CCMD("physball")
				.SetDescription("Spawns a physics sphere in front of the given or active camera.")
				.SetUsage("physball [/c cameraname] [/u units_ahead def:5]")
				.SetCommandType(CommandType.MultiArgsOptional)
				.SetCallback((sb, t) => {
					var camSpecified = t.multiArgs.ContainsKey("c");
					var unitsahead = (t.multiArgs.ContainsKey("u")) ? float.Parse(t.multiArgs["u"]) : 5f;

					Camera currentCam;
					if(Camera.allCameras.Length == 0)
						currentCam = null;
					else if(camSpecified)
						currentCam = Camera.allCameras.FirstOrDefault(c => c.name == t.multiArgs["c"]);
					else
						currentCam = Camera.allCameras[0];

					if(currentCam == null)
						sb.AppendLine(UConsole.ColorizeWarn("NO CAMERA FOUND"));

					var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					sphere.AddComponent<Rigidbody>();
					var origin = (currentCam == null) ? Vector3.zero : currentCam.transform.forward;
					var position = origin * unitsahead;
					sphere.transform.position = position;
					return sb.AppendLine("Physball spawned at " + position.ToString());
				});
		}


#if UNITY_EDITOR
		private static void RegisterEditorCommands(UConsoleController cc) {
			new CCMD("ue.view")
				.SetDescription("EDITOR ONLY: Brings the selected object into view in the scene view.")
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					Selection.activeGameObject = cc.selectedObj;
					var scene = GetSceneView();
					scene.Show(true);
					scene.Focus();
					scene.FrameSelected();
					return sb.Append("Showing in Editor...");
				});

			new CCMD("ue.select")
			.SetDescription("EDITOR ONLY: Sets the selected object as the Editor selected object.")
			.SetRequireSelectedGameObj()
			.SetCallback((sb, t) => {
				Selection.activeGameObject = cc.selectedObj;
				return sb.Append("Selected in Editor...");
			});
		}

		private static SceneView GetSceneView() {
			var scene = SceneView.currentDrawingSceneView;
			if(scene == null)
				scene = SceneView.lastActiveSceneView;
			if(scene == null && SceneView.sceneViews.Count > 0)
				scene = SceneView.sceneViews[0] as SceneView;
			if(scene == null)
				scene = SceneView.CreateInstance<SceneView>();
			return scene;
		}
#endif
		private static void RBActions(UConsoleController cc) {
			new CCMD("rb.wakeup")
				.SetDescription("Wakes up the selected object's rigidbody, if it exists.")
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					var rb = cc.selectedObj.GetComponent<Rigidbody>();
					if(rb == null)
						return sb.Append(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));

					rb.WakeUp();
					return sb.Append("Waking up rigidbody...");
				});

			new CCMD("rb.kmon")
				.SetDescription("Makes the selected object's rigidbody kinematic, if it exists.")
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					var rb = cc.selectedObj.GetComponent<Rigidbody>();
					if(rb == null)
						return sb.Append(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));

					rb.isKinematic = true;
					return sb.Append("Kinematic now on...");
				});

			new CCMD("rb.kmoff")
				.SetDescription("Makes the selected object's rigidbody NOT kinematic, if it exists.")
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					var rb = cc.selectedObj.GetComponent<Rigidbody>();
					if(rb == null)
						return sb.Append(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));

					rb.isKinematic = false;
					return sb.Append("Kinematic now off...");
				});

			new CCMD("rb.kmtoggle")
				.SetDescription("Toggles the kinematic state for the selected object's rigidbody, if it exists.")
				.SetRequireSelectedGameObj()
				.SetCallback((sb, t) => {
					var rb = cc.selectedObj.GetComponent<Rigidbody>();
					if(rb == null)
						return sb.Append(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));

					rb.isKinematic = !rb.isKinematic;
					return sb.Append("Kinematic now is " + rb.isKinematic);
				});
		}
	}
}