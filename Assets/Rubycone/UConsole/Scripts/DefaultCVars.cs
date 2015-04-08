using System;
using UnityEngine;

namespace Rubycone.UConsole {
    public static class DefaultCVars {
        public static CVar hookMode { get; private set; }
        public static CVar scrollSpeed { get; private set; }
        public static CVar runInBackground { get; private set; }
        public static CVar uVersion { get; private set; }
        public static CVar platform { get; private set; }
        public static CVar loadedLevel { get; private set; }
        public static CVar loadedLevelName { get; private set; }
        public static CVar internetReachability { get; private set; }
        public static CVar time { get; private set; }
        public static CVar scaledTime { get; private set; }
        public static CVar dateTime { get; private set; }
        public static CVar about { get; private set; }
        public static CVar physSolver { get; private set; }
        public static CVar phys2DPSolver { get; private set; }
        public static CVar phys2DVSolver { get; private set; }
        public static CVar timescale { get; private set; }

        static object @lock = new object();
        static bool loaded;

        //Just in case something tries to access before Load is called
        static DefaultCVars() {
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
                hookMode = new CVar("hookmode", "The hookmode for Unity's application output.", 0f, CVarFlags.Archive);
                hookMode.fMin = 0f;
                hookMode.fMax = 3f;

                scrollSpeed = new CVar("scrollspeed", "The console scrollspeed", 0.1f, CVarFlags.Archive);
                scrollSpeed.fMin = 0f;
                scrollSpeed.fMax = 1f;

                //Unity application
                runInBackground = new CVar("runinbackground", "If the Unity Application should run in the background.", Application.runInBackground);
                runInBackground.CVarValueChanged += RunInBackgroundCVarChanged;

                uVersion = new CVar("uversion", "Unity's version", Application.unityVersion, CVarFlags.ReadOnly);
                platform = new CVar("platform", "The current runtime platform.", Application.platform, CVarFlags.ReadOnly);
                loadedLevel = new CVar("levelindex", "The loaded level index.", Application.loadedLevel, CVarFlags.ReadOnly);
                loadedLevelName = new CVar("level", "The loaded level's name", Application.loadedLevelName, CVarFlags.ReadOnly);
                internetReachability = new CVar("webstatus", "Current internet reachability status.", Application.internetReachability, CVarFlags.ReadOnly);
                about = new CVar("about", "Info string regarding this product.", CVarFlags.ReadOnly);

                physSolver = new CVar("phys_solver", "Physics solver iteration count.", Physics.solverIterationCount);
                physSolver.CVarValueChanged += PhysSolverCVarChanged;

                phys2DPSolver = new CVar("phys2d_psolver", "2D Physics position solver iteration count.", Physics2D.positionIterations);
                phys2DPSolver.CVarValueChanged += Phys2DPosSolverCVarChanged;

                phys2DVSolver = new CVar("phys2d_vsolver", "2D Physics velocity solver iteration count.", Physics2D.velocityIterations);
                phys2DVSolver.CVarValueChanged += Phys2DVelSolverCVarChanged;

                //Time
                timescale = new CVar("timescale", "Unity's timescale.", Time.timeScale);
                timescale.CVarValueChanged += TimescaleCVarChanged;

                time = new CVar("time", "Unity's real-time since startup.", Time.realtimeSinceStartup, CVarFlags.ReadOnly);
                scaledTime = new CVar("scaledtime", "Unity's scaled time.", Time.time, CVarFlags.ReadOnly);
                dateTime = new CVar("dt", "Current system datetime.", DateTime.Now, CVarFlags.ReadOnly);
                loaded = true;
            }
        }

        static void Phys2DPosSolverCVarChanged(ValueContainer oldValues, CVar cvar) {
            Physics2D.positionIterations = cvar.iVal;
        }

        static void Phys2DVelSolverCVarChanged(ValueContainer oldValues, CVar cvar) {
            Physics2D.velocityIterations = cvar.iVal;
        }

        static void PhysSolverCVarChanged(ValueContainer oldValues, CVar cvar) {
            Physics.solverIterationCount = cvar.iVal;
        }

        static void TimescaleCVarChanged(ValueContainer oldValues, CVar cvar) {
            Time.timeScale = cvar.fVal;
        }
        static void RunInBackgroundCVarChanged(ValueContainer oldValues, CVar cvar) {
            Application.runInBackground = cvar.iVal != 0;
        }
    }
}