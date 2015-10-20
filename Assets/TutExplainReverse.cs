using UnityEngine;
using System.Collections;

public class TutExplainReverse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = true;
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
