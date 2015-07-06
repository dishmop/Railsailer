using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {
	public GameObject bodyGO;
	public GameObject sailGO;
	public GameObject fwForceGraphGO;
	public GameObject sailForceGraphGO;
	public float sailStrength = 1f;
	public float boatWindStrength = 0.1f;
	public float mass = 1;
	public string joystickId = "-J1";
	public string playerName;
	public GameObject hud;
	public GameObject wakeParticleSystem;
	
	public enum RacingState{
		kWaitingToStart,
		kRacing,
		kCompleteWinner,
		kCompleteLoser
	}
	
	public RacingState racingState = RacingState.kWaitingToStart;
	public int numLapsComplete = 0;
	
	// For use when on rail
	public float railDist;
	
	string lastTriggerName = "FWTrigger";
	bool enableLapCounter = false;
	
	 bool hasTriggerChanged = false;
	
	float boatAngle = 180;
	float boatAngleVel = 0;
	Vector3 currentVel = Vector3.zero;
	
	int fwCursorID = -1;
	int sailCursorID = -1;
	float jibAngle = 0;
	
	
	
	public float speed = 0;
	float sailAngle = 0;
	float sailAngleGlob = 0;
	
	// Use this for initialization
	void Start () {
		fwCursorID = fwForceGraphGO.GetComponent<UIGraph>().AddVCursor();
		sailCursorID = sailForceGraphGO.GetComponent<UIGraph>().AddVCursor();
	
	}
	
	void Update(){
		hud.transform.FindChild("NameBox").GetComponent<Text>().text = playerName;
		hud.transform.FindChild("LapInfo").GetComponent<Text>().text = "Lap " + (numLapsComplete + 1) +  " of " + GameConfig.singleton.totalNumLaps;
		
		if (!GameMode.singleton.IsRacing()){
			wakeParticleSystem.GetComponent<ParticleSystem>().startSpeed = 0;
			wakeParticleSystem.GetComponent<ParticleSystem>().startLifetime = 0;
			
			return;
		}
		else if (racingState == RacingState.kWaitingToStart){
			racingState = RacingState.kRacing;
		}
		
	
		
		// SAIL
		if (GameConfig.singleton.enableAutoSail){
			sailAngle = CalcOptimalAngle();
			sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
		}
		else{
			if (GameConfig.singleton.enableJibSailControl){
				float triggerValue = Input.GetAxis("RightTrigger" + joystickId);
				if (triggerValue != 0){
					hasTriggerChanged = true;
				}
				if (!hasTriggerChanged){
					triggerValue = -1;
				}
				float unityJibLength = 1 - 0.5f*(triggerValue + 1);
				jibAngle = 90 * (Mathf.Pow(unityJibLength, 2));
				//Debug.Log (unityJibLength);
				
			}
			else if (GameConfig.singleton.enable2DSailOrient){
				Vector2 dir = new Vector2 (Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
				if (dir.magnitude > 0.75){
					dir.Normalize ();
					//Debug.DrawLine(transform.position, transform.position + new Vector3(dir.x, dir.y, 0), Color.white);
					sailAngle = 360 + 90 + Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
					sailAngle = sailAngle % 360;
					sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
				}
			}
			else{		
				if (GameConfig.singleton.enableAbsoluteSailTurn){
					sailAngle = 90 * Input.GetAxis("Horizontal2");
					sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
				}
				else{
					float angleDelta = -2 * Input.GetAxis("Horizontal2");
					sailAngle += angleDelta;
					sailGO.transform.Rotate(0, 0, angleDelta);
				}
			}
		}
		sailAngleGlob = sailGO.transform.rotation.eulerAngles.z;
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		
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
					float power = -100*Input.GetAxis("Horizontal" + joystickId);
					float speed = 0.25f + Vector3.Dot(boatDir, GetComponent<Rigidbody2D>().velocity);
					float angleAccn = power * speed;
					boatAngleVel += angleAccn * Time.deltaTime;
					boatAngleVel *= 0.9f;
					boatAngle += boatAngleVel * Time.deltaTime;
					
					bodyGO.transform.rotation = Quaternion.Euler(0, 0, boatAngle);
					//GetComponent<Rigidbody2D>().AddTorque(angleAccn);
					GetComponent<SliderJoint2D>().angle = boatAngle+90;
					GetComponent<SliderJoint2D>().connectedAnchor = transform.position;
				}
	
			}
			
			
		}
		
		
		if (GameConfig.singleton.showForceVisulisation) RenderRadialForceGraph();
		CreateFwGraph();
		
		
		float wakeStrength = Mathf.Max (0, Vector3.Dot (GetComponent<Rigidbody2D>().velocity, boatDir));
		wakeParticleSystem.GetComponent<ParticleSystem>().startSpeed = 0.3f * wakeStrength;
		wakeParticleSystem.GetComponent<ParticleSystem>().startLifetime = 2f * wakeStrength;
		//Debug.Log(Vector3.Dot (GetComponent<Rigidbody2D>().velocity, boatDir));
	}
	
	void DrawJibRope(float sailAngle, float jibAngle){
		Vector3 sternPos = bodyGO.transform.TransformPoint(new Vector3(0, 0.5f, 0));
		Vector3 sailTipPos = sailGO.transform.TransformPoint(new Vector3(0, 0.8f, 0));
		Vector3 pivotPos = sailGO.transform.TransformPoint(new Vector3(0, 0f, 0));
		
		float slack = 0.005f * (jibAngle - sailAngle);
		Vector3 fwVec = sternPos - sailTipPos;
		int numPoints = 20;
		
		float halfILength = ((float)numPoints-1f)/2f;
		Vector3[] drawPoints = new Vector3[numPoints];
		for (int i = 0; i < numPoints; ++i){
			float propTaut = (1/(float)numPoints) * (halfILength * halfILength - ((float)i - halfILength) * ((float)i - halfILength));
			Vector3 tautPos = sailTipPos + fwVec * i / (numPoints-1);
			Vector3 outVec = tautPos - pivotPos;
			drawPoints[i] = tautPos + slack * propTaut * outVec;
		}
		for (int i = 0; i < numPoints-1; ++i){
			Debug.DrawLine(drawPoints[i], drawPoints[i+1], Color.cyan);
		}
				
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!GameMode.singleton.IsRacing()){
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			return;
		}
		
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
		
		if (GameConfig.singleton.enableJibSailControl){
			// figure out which way the wind is blowing the sale
			Vector3 saleNormal = sailGO.transform.rotation * new Vector3(1, 0, 0);
			Vector3 relWindVel = Environment.singleton.windVel - currentVel;
			Vector3 windForceLoc = (relWindVel * sailStrength).normalized; 
			float dotResult = Vector3.Dot (saleNormal, windForceLoc);
			
			// Clamp to -1, +1
			dotResult = Mathf.Min (1, Mathf.Max (-1, dotResult));
			float maxAngle = sailAngle - Mathf.Rad2Deg * Mathf.Asin(dotResult);
//			Debug.Log (jibAngle);
			if (dotResult > 0){
				sailAngle = Mathf.Max (-jibAngle, maxAngle);
			}
			else{
				sailAngle = Mathf.Min (jibAngle, maxAngle);
			}
			                 
			DrawJibRope(Mathf.Abs (sailAngle), jibAngle);
			
		
			sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
			
		}
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		CalcVectors(sailAngleGlob, currentVel, boatDir, out windForce, out sailForce, out fwForce);
		
		
		// Work out force from wind pushing against boat
		Vector3 boatForce = boatWindStrength * boatDir * Vector3.Dot (windForce, boatDir);
	
		
		// Calc accn
		
		Vector3 accn = (boatForce + fwForce) * mass;
		
		
		Vector3 motionVec = currentVel * Time.fixedDeltaTime;// + 0.5f * accn * Time.fixedDeltaTime * Time.fixedDeltaTime;
		Vector3 nextVel = currentVel + accn * Time.fixedDeltaTime;
		if (GameConfig.singleton.enableRail){
			railDist += Vector3.Dot (motionVec, boatDir);
		}
		else{
			GetComponent<Rigidbody2D>().AddForce(boatForce + fwForce);
			//transform.position += nextVel * Time.fixedDeltaTime;// + 0.5f * accn * Time.fixedDeltaTime;
			//transform.position += motionVec;
		}
		
		speed = Vector3.Dot (nextVel, boatDir);
		currentVel = nextVel * (1-0.3f * Time.fixedDeltaTime);
		
		
		// Draw debug fwind vel
	//	DebugUtils.DrawArrow(transform.position, transform.position + windForce, Color.blue);
		
		// Draw debug Sail normal
	//	DebugUtils.DrawArror(transform.position, transform.position + saleNormal, Color.yellow);
		
		// Debug draw sail push
	//	DebugUtils.DrawArrow(transform.position, transform.position + sailForce, Color.red);
		
		// Debug draw boat dir
	//	DebugUtils.DrawArror(transform.position, transform.position + currentVel, Color.green);
		//DebugUtils.DrawArror(transform.position, transform.position + nextVel, Color.cyan);
		
		// Debug draw fwd force on baoat
	//	DebugUtils.DrawArrow(transform.position, transform.position + fwForce, Color.magenta);
	
		HandleRacingState();
		
	}
	
	void HandleRacingState(){
	}
	
	void CalcVectors(float sailAngleLoc, Vector3 boatVel, Vector3 boatFwDir, out Vector3 windForce, out Vector3 sailForce, out Vector3 fwForce){
	
		Quaternion sailOrient = Quaternion.Euler(0, 0, sailAngleLoc);
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
			Quaternion testRot = Quaternion.Euler(0, 0, testAngle);
			Vector3 saleDir = testRot * new Vector3(0, fwForce.magnitude, 0);
			Color col = (Vector3.Dot (fwForce, boatDir) > 0) ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f); 
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
	
	
	void CreateFwGraph(){
		UIGraph fwGraph = fwForceGraphGO.GetComponent<UIGraph>();
		float maxWindForce = Environment.singleton.windVel.magnitude * sailStrength;
		fwGraph.SetAxesRanges(-180, 180, -maxWindForce, maxWindForce);
		
		UIGraph sailGraph = sailForceGraphGO.GetComponent<UIGraph>();
		sailGraph.SetAxesRanges(-180, 180, -maxWindForce, maxWindForce);
		
		
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		Vector3 currentVel = speed * boatDir;
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		
		Vector2[] fwData = new Vector2[360];
		Vector2[] sailData = new Vector2[360];
		for (int i = 0; i < 360; ++i){
			float x = (float)i - 180f;
			
			CalcVectors(x, currentVel, boatDir, out windForce, out sailForce, out fwForce);
			
			float y = Vector3.Dot (fwForce, boatDir);
			fwData[i] = new Vector2(-x, y);
			sailData[i] = new Vector2(-x, sailForce.magnitude);
		}
		fwGraph.UploadData(fwData);
		sailGraph.UploadData(sailData);
		
		CalcVectors(sailAngleGlob, currentVel, boatDir, out windForce, out sailForce, out fwForce);
		
		fwGraph.SetVCursor(fwCursorID, new Vector2(180-sailAngle, Vector3.Dot (fwForce, boatDir)));
		sailGraph.SetVCursor(sailCursorID, new Vector2(180-sailAngle, sailForce.magnitude));
//		Debug.Log(sailAngle);
		
	}
	
	void OnTriggerStartLine(){
		if (numLapsComplete == GameConfig.singleton.totalNumLaps){
			GameMode.singleton.TriggerWinner(gameObject);
		}
	}
	
	
	
	void OnTriggerEnter2D(Collider2D collider){
		string triggerName = collider.gameObject.name;
		
		if (triggerName == "FWTrigger"){
			enableLapCounter = true;
			
		}
		
		if (!enableLapCounter) return;
		
		Debug.Log ("triggerName = " + triggerName + ", lastTriggerName = " + lastTriggerName);
		
		if (lastTriggerName == "BWTrigger" && triggerName == "StartLine"){
			numLapsComplete++;
			OnTriggerStartLine();
			
		}
		else if (lastTriggerName == "FWTrigger" && triggerName == "StartLine"){
			numLapsComplete--;
		}
		
		lastTriggerName = triggerName;
		

	}

	
}
