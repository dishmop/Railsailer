using UnityEngine;
using System.Collections;

public class TutWindForceProject : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().disableSailForceLine = false;

	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}
}
