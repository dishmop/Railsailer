using UnityEngine;
using System.Collections;

public class TutPassFinish : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().overrideControls = true;
		Tutorial.singleton.player.GetComponent<Player>().maxSpeed = 2;
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
