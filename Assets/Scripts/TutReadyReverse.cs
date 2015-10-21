using UnityEngine;
using System.Collections;

public class TutReadyReverse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().maxSpeed = 0;
		Tutorial.singleton.player.GetComponent<Player>().desAngle = 60;
		Tutorial.singleton.player.GetComponent<Player>().overrideControls = false;
		
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Player player = Tutorial.singleton.player.GetComponent<Player>();
		if (player.maxSpeed == 0){
			if (Vector3.Dot(player.currentVel, player.boatDir) < 0){
				transform.FindChild ("ContinueButton").gameObject.SetActive(true);
				Tutorial.singleton.player.GetComponent<Player>().lockPosition = true;
			}
		}
	
	}
}
