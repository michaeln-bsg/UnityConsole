using System;
using UnityEngine;

namespace Rubycone.UConsole {
    public static class DefaultConVars {
        public static CVar uVersion { get; private set; }
        public static CVar platform { get; private set; }
        public static CVar loadedLevel { get; private set; }
        public static CVar loadedLevelName { get; private set; }
        public static CVar internetReachability { get; private set; }
        public static CVar time { get; private set; }
        public static CVar scaledTime { get; private set; }
        public static CVar dateTime { get; private set; }
        public static CVar about { get; private set; }

        static object @lock = new object();
        static bool loaded;

        //Just in case something tries to access before Load is called
        static DefaultConVars() {
            Load();
        }

        // Use this for initialization
        public static void Load() {
            if(!loaded)
                RegisterUnityConVars();
        }

        // Update is called once per frame
        static void RegisterUnityConVars() {
            lock(@lock) {
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
                about = new CVar("", "Info string regarding this product.", CVarFlags.ReadOnly, "about");

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
                loaded = true;
            }
        }

        static void Phys2DPosSolverCVarChanged(string oldVal, CVar convar) {
            int result;
            if(convar.TryGetInt(out result))
                Physics2D.positionIterations = result;
        }

        static void Phys2DVelSolverCVarChanged(string oldVal, CVar convar) {
            int result;
            if(convar.TryGetInt(out result))
                Physics2D.velocityIterations = result;
        }

        static void PhysSolverCVarChanged(string oldVal, CVar convar) {
            int result;
            if(convar.TryGetInt(out result))
                Physics.solverIterationCount = result;
        }

        static void GravityCVarChanged(string oldVal, CVar convar) {
            Vector3 result;
            if(convar.TryGetVec3(out result))
                Physics.gravity = result;
        }
        static void Gravity2DCVarChanged(string oldVal, CVar convar) {
            Vector2 result;
            if(convar.TryGetVec2(out result))
                Physics2D.gravity = result;
        }

        static void TimescaleCVarChanged(string oldVal, CVar convar) {
            float result;
            if(convar.TryGetFloat(out result))
                Time.timeScale = result;
        }
        static void RunInBackgroundCVarChanged(string oldVal, CVar convar) {
            Application.runInBackground = convar.GetBool();
        }
    }
}