using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public GameObject bodyGO;
	public GameObject sailGO;
	public float sailStrength = 1f;
	public float mass = 1;
	
	public float trackDist; 
	
	public float speed = 0;
	float sailAngle = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update(){
		// relative turning
		float angleDelta = -2 * Input.GetAxis("Horizontal");
		sailGO.transform.Rotate(0, 0, angleDelta);
		
		// Absolute orientation
		sailAngle = 90 * Input.GetAxis("Horizontal");
		sailGO.transform.rotation = Quaternion.Euler(0, 0, sailAngle);
		//Debug.Log(sailAngle);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (RailMaker.singleton.HasLocationData()){
			RailMaker.LocationData data = RailMaker.singleton.GetTrackLocation(trackDist);
			transform.position = data.pos;
			bodyGO.transform.rotation = data.rotation;
			// Temp control
			//sailGO.transform.rotation = data.rotation;
		}
		else{
			transform.position = Vector3.zero;
		}
		
		// calc current Vel;
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		Vector3 currentVel = speed * boatDir;
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		CalcVectors(sailAngle, currentVel, boatDir, out windForce, out sailForce, out fwForce);
	
		
		// Calc the forces
	
		Vector3 accn = fwForce * mass;
		
		
		Vector3 motionVec = currentVel * Time.fixedDeltaTime + 0.5f * accn * Time.fixedDeltaTime * Time.fixedDeltaTime;
		trackDist += Vector3.Dot (motionVec, boatDir);
		
		Vector3 nextVel = currentVel + accn * Time.fixedDeltaTime;
		speed = Vector3.Dot (nextVel, boatDir);
		
		
		// Draw debug fwind vel
		DebugUtils.DrawArrow(transform.position, transform.position + windForce, Color.blue);
		
		// Draw debug Sail normal
	//	DebugUtils.DrawArror(transform.position, transform.position + saleNormal, Color.yellow);
		
		// Debug draw sail push
		DebugUtils.DrawArrow(transform.position, transform.position + sailForce, Color.red);
		
		// Debug draw boat dir
	//	DebugUtils.DrawArror(transform.position, transform.position + currentVel, Color.green);
		//DebugUtils.DrawArror(transform.position, transform.position + nextVel, Color.cyan);
		
		// Debug draw fwd force on baoat
		DebugUtils.DrawArrow(transform.position, transform.position + fwForce, Color.magenta);
		
	}
	
	void CalcVectors(float sailAngle, Vector3 boatVel, Vector3 boatFwDir, out Vector3 windForce, out Vector3 sailForce, out Vector3 fwForce){
	
		Quaternion sailOrient = Quaternion.Euler(0, 0, sailAngle);
		Vector3 saleNormal = sailOrient * new Vector3(1, 0, 0);
		
		
		Vector3 relWindVel = Environment.singleton.windVel - boatVel;
		windForce = relWindVel * sailStrength; 
		
		sailForce = saleNormal * Vector3.Dot(windForce, saleNormal);
		fwForce = boatFwDir * Vector3.Dot (sailForce, boatFwDir);
		
	
	}
}
