using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Rubycone.UConsole;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rubycone.UConsole {

    public static class DefaultCommands {

        static object @lock = new object();
        static bool loaded;

        public static void Load() {
            if(loaded)
                return;

            lock(@lock) {
                MetaCommands();
                UnityCommands();
                RBActions();

#if UNITY_EDITOR
                RegisterEditorCommands();
#endif
                loaded = true;
            }
        }

        static void UnityCommands() {
            new CCommand("exit", "Quits the application.")
                .CommandExecuted += (args) => {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
				    Application.Quit();
#endif
                    UConsole.LogWarn("!!! QUITTING APPLICATION !!!");
                    return true;
                };

            new CCommand("destroy", "Destroys the selected GameObject.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    GameObject.Destroy(UConsole.selectedObj);
                    UConsole.Log("Object destroyed.");
                    return true;
                };

            new CCommand("destroyc", "Destroys the selected Component.", CCommandFlags.RequireSelectedComponent)
                .CommandExecuted += (args) => {
                    Component.Destroy(UConsole.selectedComponent);
                    UConsole.Log("Component destroyed.");
                    return true;
                };

            new CCommand("toggle", "Toggles the selected GameObject.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    UConsole.selectedObj.SetActive(!UConsole.selectedObj.activeSelf);
                    UConsole.Log("GameObject active == " + UConsole.selectedObj.activeSelf + ".");
                    return true;
                };

            new CCommand("listc", "Lists all components on the selected GameObject", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    var components = UConsole.selectedObj.GetComponents<Component>();
                    for(int i = 0; i < components.Length; i++) {
                        var c = components[i];
                        UConsole.Log(string.Format("{0}) {1}", i + 1, c.GetType().Name));
                    }
                    return true;
                };

            new CCommand("sendmsg", "Sends a message using Unity's standard message system.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    UConsole.selectedObj.SendMessage(args[0], SendMessageOptions.DontRequireReceiver);
                    UConsole.Log("Message sent");
                    return true;
                };
            new CCommand("sendmsgr", "Sends a message using Unity's standard message system (requires receiver).", CCommandFlags.RequireSelectedObj).CommandExecuted += (args) => {
                UConsole.selectedObj.SendMessage(args[0], SendMessageOptions.RequireReceiver);
                UConsole.Log("Message sent");
                return true;
            };

            new CCommand("sendmsgup", "Sends a message upwards using Unity's standard message system.", CCommandFlags.RequireSelectedObj).CommandExecuted += (args) => {
                UConsole.selectedObj.SendMessageUpwards(args[0], SendMessageOptions.DontRequireReceiver);
                UConsole.Log("Message sent");
                return true;
            };

            new CCommand("sendmsgrup", "Sends a message upwards using Unity's standard message system (requires receiver).", CCommandFlags.RequireSelectedObj).CommandExecuted += (args) => {
                UConsole.selectedObj.SendMessageUpwards(args[0], SendMessageOptions.RequireReceiver);
                UConsole.Log("Message sent");
                return true;
            };

            new CCommand("bmsg", "Broadcasts a message using Unity's standard message system.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    UConsole.selectedObj.BroadcastMessage(args[0], SendMessageOptions.DontRequireReceiver);
                    UConsole.Log("Message sent");
                    return true;
                };

            new CCommand("bmsgr", "Broadcasts a message using Unity's standard message system (requires receiver).", CCommandFlags.RequireSelectedObj).CommandExecuted += (args) => {
                UConsole.selectedObj.BroadcastMessage(args[0], SendMessageOptions.RequireReceiver);
                UConsole.Log("Message sent");
                return true;
            };

            new CCommand("selectc", "Selects a component on the selected object.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    var component = UConsole.selectedObj.GetComponent(args[0]);
                    UConsole.selectedComponent = (component == null) ? UConsole.selectedComponent : component;

                    if(component == null) {
                        UConsole.LogErr("NO COMPONENT FOUND.");
                    }
                    else {
                        UConsole.Log("Selected component (IID:" + component.GetInstanceID() + ")");
                    }
                    return component != null;
                };

            new CCommand("selectedc", "Returns the selected component on the selected object.", CCommandFlags.RequireSelectedComponent).CommandExecuted += (args) => {
                UConsole.Log("Selected component (IID:" + UConsole.selectedComponent.GetInstanceID() + ")");
                return true;
            };
        }

        static void MetaCommands() {
            new CCommand("revert", "Reverts a cvar to its default value.", CCommandFlags.RequireArgs)
                .CommandExecuted += (args) => {
                    var cvar = UConsoleDB.GetCFunc<CVar>(args[0]);
                    if(cvar == null) {
                        UConsole.LogErr("CVAR NOT FOUND: " + args[0]);
                        return false;
                    }
                    else {
                        var isReadonly = (cvar.flags & CVarFlags.ReadOnly) != 0;
                        if(!isReadonly) {
                            cvar.Revert();
                            UConsole.Log("CVar reverted to default: " + cvar.sVal);
                        }
                        else {
                            UConsole.LogWarn(string.Format("CVar {0} IS READ-ONLY.", cvar.alias));
                        }
                        return true;
                    }
                };
            new CCommand("revert_all", "Reverts ALL cvars to their default values.")
               .CommandExecuted += (args) => {
                   var verbose = args.Length > 0 && args[0] == "v";
                   foreach(var c in UConsoleDB.cvars) {
                       var isReadonly = (c.flags & CVarFlags.ReadOnly) != 0;
                       if(!isReadonly) {
                           c.Revert();
                       }
                       if(verbose) {
                           if(isReadonly) {
                               UConsole.LogWarn(string.Format("CVar {0} IS READ-ONLY.", c.alias));
                           }
                           else {
                               UConsole.Log(string.Format("CVar {0} reverted to default: {1}", c.alias, c.sVal));
                           }
                       }
                   }
                   UConsole.Log("Reverted all cvars to default values.");
                   return true;
               };
            new CCommand("help", "Returns help information.")
                .CommandExecuted += (args) => {
                    if(args.Length != 0) {
                        var command = UConsoleDB.GetCFunc<CCommand>(args[0]);
                        if(command == null) {
                            UConsole.LogErr("COMMAND NOT FOUND: " + args[0]);
                            return false;
                        }
                        else {
                            UConsole.Log(command.alias);
                            UConsole.Log("\t" + command.description);
                            UConsole.Log("\t\tusage: " + command.usage);
                            return true;
                        }
                    }
                    else {
                        var color = false;
                        foreach(var c in UConsoleDB.ccmds.OrderBy(c => c.alias)) {
                            var sb = new StringBuilder();
                            sb.AppendLine(c.alias);
                            sb.AppendLine("\t" + c.description);
                            sb.AppendLine("\tusage: " + c.usage);
                            if(color) {
                                UConsole.Log(UConsole.Colorize(sb.ToString(), Color.cyan));
                            }
                            else {
                                UConsole.Log(sb.ToString());
                            }
                            color = !color;
                        }
                        return true;
                    }
                };
            new CCommand("cvars", "Lists all registered cvars.")
                .CommandExecuted += (args) => {
                    foreach(var cvar in UConsoleDB.cvars.OrderBy(c => c.alias)) {
                        UConsole.Log(cvar.alias);
                    }
                    return true;
                };
            new CCommand("physball", "Spawns a physics sphere in front of the active or main camera.")
                .CommandExecuted += (args) => {
                    var unitsAhead = 10f;
                    if(args.Length != 0) {
                        float.TryParse(args[0], out unitsAhead);
                    }

                    var currentCam = Camera.main;
                    if(currentCam == null) {
                        currentCam = Camera.allCameras[0];
                    }
                    if(currentCam == null) {
                        UConsole.LogWarn("NO CAMERA FOUND");
                    }

                    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.AddComponent<Rigidbody>();
                    var origin = (currentCam == null) ? Vector3.zero : currentCam.transform.forward;
                    var position = origin * unitsAhead;
                    sphere.transform.position = position;
                    UConsole.Log("Physball spawned at " + position.ToString());
                    return true;
                };
        }


#if UNITY_EDITOR
        static void RegisterEditorCommands() {
            new CCommand("ue.view", "EDITOR ONLY: Brings the selected object into view in the scene view.", CCommandFlags.EditorOnly)
                .CommandExecuted += (args) => {
                    Selection.activeGameObject = UConsole.selectedObj;
                    var scene = GetSceneView();
                    scene.Show(true);
                    scene.Focus();
                    scene.FrameSelected();
                    UConsole.Log("Showing in Editor...");
                    return true;
                };

            new CCommand("ue.select", "EDITOR ONLY: Sets the selected object as the Editor selected object.", CCommandFlags.EditorOnly | CCommandFlags.RequireSelectedObj).CommandExecuted += (args) => {
                Selection.activeGameObject = UConsole.selectedObj;
                EditorGUIUtility.PingObject(Selection.activeGameObject);
                UConsole.Log("Selected in Editor...");
                return true;
            };
        }

        static SceneView GetSceneView() {
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
        static void RBActions() {
            new CCommand("rb.wakeup", "Wakes up the selected object's rigidbody, if it exists.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    var rb = UConsole.selectedObj.GetComponent<Rigidbody>();
                    if(rb == null) {
                        UConsole.LogErr(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));
                        return false;
                    }
                    rb.WakeUp();
                    UConsole.Log("Waking up rigidbody...");
                    return true;
                };

            new CCommand("rb.kmon", "Makes the selected object's rigidbody kinematic, if it exists.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    var rb = UConsole.selectedObj.GetComponent<Rigidbody>();
                    if(rb == null) {
                        UConsole.LogErr(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));
                        return false;
                    }

                    rb.isKinematic = true;
                    UConsole.Log("Kinematic now on...");
                    return true;
                };

            new CCommand("rb.kmoff", "Makes the selected object's rigidbody NOT kinematic, if it exists.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    var rb = UConsole.selectedObj.GetComponent<Rigidbody>();
                    if(rb == null) {
                        UConsole.LogErr(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));
                        return false;
                    }

                    rb.isKinematic = false;
                    UConsole.Log("Kinematic now off...");
                    return true;
                };

            new CCommand("rb.kmtoggle", "Toggles the kinematic state for the selected object's rigidbody, if it exists.", CCommandFlags.RequireSelectedObj)
                .CommandExecuted += (args) => {
                    var rb = UConsole.selectedObj.GetComponent<Rigidbody>();
                    if(rb == null) {
                        UConsole.LogErr(UConsole.ColorizeErr("NO RIGIDBODY ON SELECTED OBJECT"));
                        return false;
                    }

                    rb.isKinematic = !rb.isKinematic;
                    UConsole.Log("Kinematic now is " + rb.isKinematic);
                    return true;
                };
        }
    }
}