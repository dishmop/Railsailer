using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	enum State {
		kOff,
		kInitialise,
		kControls,
	}
	
	State state = State.kOff;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
	}
}
