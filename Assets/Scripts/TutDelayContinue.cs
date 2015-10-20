using UnityEngine;
using System.Collections;

public class TutDelayContinue : MonoBehaviour {

	public float cumulativeDeg ;
	float lastSailAngle = 1000;
	
	// Use this for initialization
	void Start () {
		
		transform.FindChild("ContinueButton").gameObject.SetActive(false);
		cumulativeDeg = 0;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float sailAngle = Tutorial.singleton.player.GetComponent<Player>().sailAngle;
		if (lastSailAngle < 999){
			cumulativeDeg += Mathf.Abs(lastSailAngle - sailAngle);
		}
		lastSailAngle = sailAngle;
		if (cumulativeDeg > 200){
			transform.FindChild("ContinueButton").gameObject.SetActive(true);
		}
	
	}
}
