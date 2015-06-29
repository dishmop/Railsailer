using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public float trackDist; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (RailMaker.singleton.HasLocationData()){
			RailMaker.LocationData data = RailMaker.singleton.GetTrackLocation(trackDist);
			transform.position = data.pos;
		}
		else{
			transform.position = Vector3.zero;
		}
		
	
	}
}
