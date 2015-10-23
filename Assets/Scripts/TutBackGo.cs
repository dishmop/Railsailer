using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class TutBackGo : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().maxSpeed = -1;
		Tutorial.singleton.player.GetComponent<Player>().desAngle = 60;
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = false;
		
		Analytics.CustomEvent("tutorial3MoveReally", new Dictionary<string, object>
		{
			{ "tutTime", Tutorial.singleton.GetTutorialTime()},
		});		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
