using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	public GameObject objToTrack;
	Vector3 initPos;

	// Use this for initialization
	void Start () {
		initPos = transform.position;
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (GameConfig.singleton.enableTracking){
			Vector3 pos = objToTrack.transform.position;
			
			pos.z = transform.position.z;
			transform.position = pos;
		}
		else{
			transform.position = initPos;
		
		}
	
	}
}
