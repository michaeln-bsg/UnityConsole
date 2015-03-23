using UnityEngine;
using System.Collections;

public class UConsoleCreator : MonoBehaviour {
	[SerializeField]
	GameObject consolePrefab;

	// Use this for initialization
	void Start() {
		(GameObject.Instantiate(consolePrefab) as GameObject).name = "UConsole Canvas";
		Destroy(gameObject);
	}
}
