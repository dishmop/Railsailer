using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public GameObject bodyGO;
	public GameObject sailGO;
	public GameObject fwForceGraphGO;
	public float sailStrength = 1f;
	public float mass = 1;
	
	// For use when on rail
	public float railDist; 
	
	float boatAngle = 0;
	Vector3 currentVel = Vector3.zero;
	
	int cursorID = -1;
	
	
	
	public float speed = 0;
	float sailAngle = 0;

	// Use this for initialization
	void Start () {
		cursorID = fwForceGraphGO.GetComponent<UIGraph>().AddVCursor();
	
	}
	
	void Update(){
		// SAIL
		if (GameConfig.singleton.enableAudioSail){
			sailAngle = CalcOptimalAngle();
			sailGO.transform.rotation = Quaternion.Euler(0, 0, sailAngle);
		}
		else{
			if (GameConfig.singleton.enable2DSailOrient){
				Vector2 dir = new Vector2 (Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
				if (dir.magnitude > 0.75){
					dir.Normalize ();
					//Debug.DrawLine(transform.position, transform.position + new Vector3(dir.x, dir.y, 0), Color.white);
					sailAngle = 360  + Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
					sailAngle = sailAngle % 360;
					sailGO.transform.rotation = Quaternion.Euler(0, 0, sailAngle);
				}
			}
			else{		
				if (GameConfig.singleton.enableAbsoluteSailTurn){
					sailAngle = 90 * Input.GetAxis("Horizontal2");
					sailGO.transform.rotation = Quaternion.Euler(0, 0, sailAngle);
				}
				else{
					float angleDelta = -2 * Input.GetAxis("Horizontal2");
					sailAngle += angleDelta;
					sailGO.transform.Rotate(0, 0, angleDelta);
				}
			}
		}
		
		// BOAT
		if (!GameConfig.singleton.enableRail){
			if (GameConfig.singleton.enable2DSailOrient){
				Vector2 dir = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
				if (dir.magnitude > 0.75){
					dir.Normalize ();
					//Debug.DrawLine(transform.position, transform.position + new Vector3(dir.x, dir.y, 0), Color.white);
					boatAngle = 90 + Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
					bodyGO.transform.rotation = Quaternion.Euler(0, 0, boatAngle);
				}
			}
			else{
				if (GameConfig.singleton.enableAbsoluteBoatTurn){
					boatAngle = 90 * Input.GetAxis("Horizontal");
					bodyGO.transform.rotation = Quaternion.Euler(0, 0, boatAngle);
				}
				else{
					float angleDelta = -2 * Input.GetAxis("Horizontal");
					boatAngle += angleDelta;
					bodyGO.transform.Rotate(0, 0, angleDelta);
				}
	
			}
			
			
		}
		
		
		if (GameConfig.singleton.showForceVisulisation) RenderRadialForceGraph();
		CreateGraphs();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GameConfig.singleton.enableRail){
			if (RailMaker.singleton.HasLocationData()){
				RailMaker.LocationData data = RailMaker.singleton.GetTrackLocation(railDist);
				transform.position = data.pos;
				bodyGO.transform.rotation = data.rotation;
				// Temp control
				//sailGO.transform.rotation = data.rotation;
			}
			else{
				transform.position = Vector3.zero;
			}
		}
		
		
		// calc current Vel;
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		if (GameConfig.singleton.enableRail){
			currentVel = speed * boatDir;
		}
		else{
			currentVel = boatDir * Vector3.Dot(boatDir, currentVel);
		}
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		CalcVectors(sailAngle, currentVel, boatDir, out windForce, out sailForce, out fwForce);
	
		
		// Calc the forces
	
		Vector3 accn = fwForce * mass;
		
		
		Vector3 motionVec = currentVel * Time.fixedDeltaTime + 0.5f * accn * Time.fixedDeltaTime * Time.fixedDeltaTime;
		Vector3 nextVel = currentVel + accn * Time.fixedDeltaTime;
		if (GameConfig.singleton.enableRail){
			railDist += Vector3.Dot (motionVec, boatDir);
		}
		else{
			GetComponent<Rigidbody2D>().velocity = new Vector2(nextVel.x, nextVel.y);
			//transform.position += motionVec;
		}
		
		speed = Vector3.Dot (nextVel, boatDir);
		currentVel = nextVel;
		
		
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
	
	void RenderRadialForceGraph(){
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		Vector3 currentVel = speed * boatDir;
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		
		
		
		for (int i = 0; i < 360; ++i){
			float testAngle = i - 180f;
				
			CalcVectors(testAngle, currentVel, boatDir, out windForce, out sailForce, out fwForce);
			Quaternion testRot = Quaternion.Euler(0, 0, testAngle-90);
			Vector3 saleDir = testRot * new Vector3(0, fwForce.magnitude, 0);
			Color col = (Vector3.Dot (fwForce, boatDir) > 0) ? Color.green : Color.red; 
			Debug.DrawLine (transform.position, transform.position + saleDir, col);
			
			
		}
	}
	
	float CalcOptimalAngle(){
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		Vector3 currentVel = speed * boatDir;
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		
		float maxForce = -1f;
		float bestAngle = 0f;
		
		for (int i = 0; i < 360; ++i){
			float x = (float)i - 180f;
			
			CalcVectors(x, currentVel, boatDir, out windForce, out sailForce, out fwForce);
			
			float forceMag = Vector3.Dot (fwForce, boatDir);
			if (forceMag > maxForce){
				maxForce = forceMag;
				bestAngle = x;
			}
			
		}
		return bestAngle;
	}
	
	
	void CreateGraphs(){
		UIGraph uiGraph = fwForceGraphGO.GetComponent<UIGraph>();
		float maxWindForce = Environment.singleton.windVel.magnitude * sailStrength;
		uiGraph.SetAxesRanges(-180, 180, -maxWindForce, maxWindForce);
		
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		Vector3 currentVel = speed * boatDir;
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		
		Vector2[] data = new Vector2[360];
		for (int i = 0; i < 360; ++i){
			float x = (float)i - 180f;
			
			CalcVectors(x, currentVel, boatDir, out windForce, out sailForce, out fwForce);
			
			float y = Vector3.Dot (fwForce, boatDir);
			data[i] = new Vector2(-x,y);
		}
		uiGraph.UploadData(data);
		
		CalcVectors(sailAngle, currentVel, boatDir, out windForce, out sailForce, out fwForce);
		
		uiGraph.SetVCursor(cursorID, new Vector2(180-sailAngle, Vector3.Dot (fwForce, boatDir)));
//		Debug.Log(sailAngle);
		
	}
	
	
	void OnTriggerEnter2D(Collider2D collider){
		Debug.Log ("OnTriggerEnter2D");
		currentVel = -0.2f * currentVel;
		GetComponent<Rigidbody2D>().velocity = new Vector2(currentVel.x, currentVel.y);
		GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + 5 * GetComponent<Rigidbody2D>().velocity * Time.fixedDeltaTime);
		
	}
	
	void OnTriggerStay2D(Collider2D collider){
		Debug.Log ("OnTriggerStay2D");
		//currentVel = Vector3.zero;
		
	}
	
}
