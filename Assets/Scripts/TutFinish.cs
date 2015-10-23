using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class TutFinish : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = false;
		Analytics.CustomEvent("tutorial4Finish", new Dictionary<string, object>
		                      {
			{ "tutTime", Tutorial.singleton.GetTutorialTime()},
		});		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
