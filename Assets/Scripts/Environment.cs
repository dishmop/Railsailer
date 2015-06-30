using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {
	public static Environment singleton = null;
	public bool debugShowWindDir = false;
	public Vector3 windVel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (debugShowWindDir){
			Vector3 startPos = new Vector3(3, 3, 0);
			DebugUtils.DrawArrow(startPos, startPos + windVel, Color.blue);
		}
		
	}
	
	
	//----------------------------------------------
	void Awake(){
		if (singleton != null) Debug.LogError ("Error assigning singleton");
		singleton = this;
	}
	
	
	
	void OnDestroy(){
		singleton = null;
	}
}
