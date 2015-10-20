using UnityEngine;
using System.Collections;

public class TutWindForce : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.ShowOverlay(true);
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().DisableAll(true);
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().disableMainCircle = false;
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().disableWindForceLine = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
