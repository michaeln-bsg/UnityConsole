using UnityEngine;
using System.Collections;
using System;

namespace Rubycone.UConsole {
    public class UpdateDefCVars : MonoBehaviour {
        [SerializeField, TextArea]
        string aboutStr;

        void Awake() {
            DefaultCommands.Load();
            DefaultCVars.Load();
        }

        void Update() {
            DefaultCVars.uVersion.SetValue(Application.unityVersion);
            DefaultCVars.platform.SetValue(Application.platform);
            DefaultCVars.loadedLevelName.SetValue(Application.loadedLevelName);
            DefaultCVars.loadedLevel.SetValue(Application.loadedLevel);
            DefaultCVars.internetReachability.SetValue(Application.internetReachability);
            DefaultCVars.time.SetValue(Time.realtimeSinceStartup);
            DefaultCVars.scaledTime.SetValue(Time.time);
            DefaultCVars.dateTime.SetValue(DateTime.Now);
            DefaultCVars.about.SetValue(aboutStr);
        }
    }
}