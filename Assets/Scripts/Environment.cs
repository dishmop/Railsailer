using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {
	public static Environment singleton = null;
	public bool debugShowWindDir = false;
	public Vector3 windVel;
	public GameObject particlesGO;
	public GameObject playerGO;

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
		float angle = Mathf.Rad2Deg * Mathf.Atan2(windVel.x, windVel.y);
		particlesGO.transform.rotation = Quaternion.Euler(angle-90, 90, 90);
		ParticleSystem particles = particlesGO.GetComponent<ParticleSystem>();
		particles.startSpeed = windVel.magnitude;
		
		// position it off screen
		Vector3 camCentrePos = Camera.main.transform.position;
		Vector3 camCornerPos = Camera.main.ScreenToWorldPoint(Vector3.zero);
		camCentrePos.z = 0;
		camCornerPos.z = 0;
		float diagDist = (camCentrePos - camCornerPos).magnitude;
		
		Vector3 windDir = windVel.normalized;
		Vector3 basePos = playerGO.transform.position;
		basePos.z = -0.2f;
		particlesGO.transform.position = basePos- windDir * diagDist;
		
		
		
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
