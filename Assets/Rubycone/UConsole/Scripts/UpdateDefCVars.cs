using UnityEngine;
using System.Collections;
using System;

namespace Rubycone.UConsole {
    public class UpdateDefCVars : MonoBehaviour {
        [SerializeField, TextArea]
        string aboutStr;

        // Update is called once per frame
        void Update() {
            DefaultConVars.uVersion.SetVal(Application.unityVersion);
            DefaultConVars.platform.SetVal(Application.platform);
            DefaultConVars.loadedLevelName.SetVal(Application.loadedLevelName);
            DefaultConVars.loadedLevel.SetVal(Application.loadedLevel);
            DefaultConVars.internetReachability.SetVal(Application.internetReachability);
            DefaultConVars.time.SetVal(Time.realtimeSinceStartup);
            DefaultConVars.scaledTime.SetVal(Time.time);
            DefaultConVars.dateTime.SetVal(DateTime.Now);
            DefaultConVars.about.SetVal(aboutStr);
        }
    }
}