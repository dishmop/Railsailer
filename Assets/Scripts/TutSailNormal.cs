using UnityEngine;
using System.Collections;

public class TutSailNormal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().disableSailDirLine = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
