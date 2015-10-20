using UnityEngine;
using System.Collections;

public class TutKeel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().disableBoatDirLine = false;
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
