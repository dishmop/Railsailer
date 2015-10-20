using UnityEngine;
using System.Collections;

public class TutFwForce : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().disableFwForceLine = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
