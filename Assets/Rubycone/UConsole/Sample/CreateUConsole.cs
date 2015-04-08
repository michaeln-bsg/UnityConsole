using UnityEngine;
using System.Collections;
using Rubycone.UConsole;

public class CreateUConsole : MonoBehaviour {
    [SerializeField]
    GameObject prefab;

    void Awake() {
        UConsole.Create(prefab);
        DefaultCommands.Load();
        DefaultCVars.Load();
        Destroy(this);
    }
}
