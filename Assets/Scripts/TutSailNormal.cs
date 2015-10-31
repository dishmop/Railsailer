using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Analytics;


public class TutSailNormal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.overlay.GetComponent<PlayerOverlay>().disableSailDirLine = false;
		
		GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "tutorial1SailNormal", "", Tutorial.singleton.GetTutorialTime());
		GoogleAnalytics.Client.SendScreenHit("tutorial1SailNormal");
		
//		Analytics.CustomEvent("tutorial1SailNormal", new Dictionary<string, object>
//		{
//			{ "tutTime", Tutorial.singleton.GetTutorialTime()},
//		});
		                      
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
