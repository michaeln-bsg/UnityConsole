using UnityEngine;
using System.Collections;
using System;

namespace BSGTools.Console {
	public class DefaultConVars : MonoBehaviour {
		[SerializeField]
		string aboutStr;

		CVar uVersion,
			platform,
			loadedLevel,
			loadedLevelName,
			internetReachability,
			time,
			scaledTime,
			dateTime,
			about;

		// Use this for initialization
		void Start() {
			RegisterUnityConVars();
		}

		// Update is called once per frame
		void RegisterUnityConVars() {
			//UnityConsole settings
			new CVar("0", "The hookmode for Unity's application output.", "hookmode");
			new CVar("0.1", "The console scrollspeed", CVarFlags.Track, "scrollspeed");

			//Unity application
			new CVar(Application.runInBackground, "If the Unity Application should run in the background.", "runinbackground").CVarValueChanged += RunInBackgroundCVarChanged;
			uVersion = new CVar(Application.unityVersion, "Unity's version", CVarFlags.ReadOnly, "uversion");
			platform = new CVar(Application.platform, "The current runtime platform.", CVarFlags.ReadOnly, "platform");
			loadedLevel = new CVar(Application.loadedLevel, "The loaded level index.", CVarFlags.ReadOnly, "levelindex");
			loadedLevelName = new CVar(Application.loadedLevelName, "The loaded level's name", CVarFlags.ReadOnly, "level");
			internetReachability = new CVar(Application.internetReachability, "Current internet reachability status.", CVarFlags.ReadOnly, "webstatus");
			about = new CVar(aboutStr, "Info string regarding this product.", CVarFlags.ReadOnly, "about");

			//Physics
			new CVar(Physics.gravity, "Current physics gravity.", CVarFlags.Track, "gravity").CVarValueChanged += GravityCVarChanged;
			new CVar(Physics2D.gravity, "Current 2D physics gravity.", CVarFlags.Track, "gravity2d").CVarValueChanged += Gravity2DCVarChanged;
			new CVar(Physics.solverIterationCount, "Physics solver iteration count.", CVarFlags.Track, "phys_solver").CVarValueChanged += PhysSolverCVarChanged;
			new CVar(Physics2D.positionIterations, "2D Physics position solver iteration count.", CVarFlags.Track, "phys2d_psolver").CVarValueChanged += Phys2DPosSolverCVarChanged;
			new CVar(Physics2D.velocityIterations, "2D Physics velocity solver iteration count.", CVarFlags.Track, "phys2d_vsolver").CVarValueChanged += Phys2DVelSolverCVarChanged;

			//Time
			new CVar("1.0", "Unity's timescale.", CVarFlags.Track, "timescale", "ts").CVarValueChanged += TimescaleCVarChanged;
			time = new CVar(Time.realtimeSinceStartup, "Unity's real-time since startup.", CVarFlags.ReadOnly, "time");
			scaledTime = new CVar(Time.time, "Unity's scaled time.", CVarFlags.ReadOnly, "scaledtime");
			dateTime = new CVar(DateTime.Now, "Current system datetime.", CVarFlags.ReadOnly, "datetime", "dt");
		}

		private void Phys2DPosSolverCVarChanged(string oldVal, CVar convar) {
			int result;
			if(convar.TryGetInt(out result))
				Physics2D.positionIterations = result;
		}

		private void Phys2DVelSolverCVarChanged(string oldVal, CVar convar) {
			int result;
			if(convar.TryGetInt(out result))
				Physics2D.velocityIterations = result;
		}

		private void PhysSolverCVarChanged(string oldVal, CVar convar) {
			int result;
			if(convar.TryGetInt(out result))
				Physics.solverIterationCount = result;
		}

		private void GravityCVarChanged(string oldVal, CVar convar) {
			Vector3 result;
			if(convar.TryGetVec3(out result))
				Physics.gravity = result;
		}
		private void Gravity2DCVarChanged(string oldVal, CVar convar) {
			Vector2 result;
			if(convar.TryGetVec2(out result))
				Physics2D.gravity = result;
		}

		private void TimescaleCVarChanged(string oldVal, CVar convar) {
			float result;
			if(convar.TryGetFloat(out result))
				Time.timeScale = result;
		}

		void Update() {
			uVersion.SetVal(Application.unityVersion);
			platform.SetVal(Application.platform);
			loadedLevelName.SetVal(Application.loadedLevelName);
			loadedLevel.SetVal(Application.loadedLevel);
			internetReachability.SetVal(Application.internetReachability);
			time.SetVal(Time.realtimeSinceStartup);
			scaledTime.SetVal(Time.time);
			dateTime.SetVal(DateTime.Now);
			about.SetVal(aboutStr);
		}

		private void RunInBackgroundCVarChanged(string oldVal, CVar convar) {
			Application.runInBackground = convar.GetBool();
		}
	}
}