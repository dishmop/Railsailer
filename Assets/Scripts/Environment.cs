using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {
	public static Environment singleton = null;
	public bool debugShowWindDir = false;
	public Vector3 windVel;
	public GameObject playerGO;
	

//	float distDiagProp = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (debugShowWindDir){
			Vector3 startPos = new Vector3(3, 3, 0);
			DebugUtils.DrawArrow(startPos, startPos + windVel, Color.blue);
		}
		
		// Set rotation
//		float angle = Mathf.Rad2Deg * Mathf.Atan2(windVel.x, windVel.y);
//		float diagDist = 50;
		
//		Vector3 windDir = windVel.normalized;
//		Vector3 basePos = playerGO.transform.position;
//
//		basePos = Vector3.zero;
//		basePos.z = -1.5f;
//		
// 		distDiagProp = Mathf.Min (distDiagProp + 0.001f, 1);
		
		
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
