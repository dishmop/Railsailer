﻿using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class TutFwMoveReally : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = false;
		
		GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "tutorial2MoveReally", "", Tutorial.singleton.GetTutorialTime());
		GoogleAnalytics.Client.SendScreenHit("tutorial2MoveReally");
		
//		Analytics.CustomEvent("tutorial2MoveReally", new Dictionary<string, object>
//		{
//			{ "tutTime", Tutorial.singleton.GetTutorialTime()},
//		});	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
