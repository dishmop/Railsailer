﻿using UnityEngine;
using System.Collections;

public class TutFwMoveReally : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = false;
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}