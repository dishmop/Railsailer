using UnityEngine;
using System.Collections;

public class TutBackGo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().maxSpeed = -1;
		Tutorial.singleton.player.GetComponent<Player>().desAngle = 60;
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
