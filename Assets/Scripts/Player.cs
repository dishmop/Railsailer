using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Vectrosity;

public class Player : MonoBehaviour {
	public GameObject bodyGO;
	public GameObject sailGO;
	public float sailStrength = 1f;
	public float boatWindStrength = 0.1f;
	public float mass = 1;
	public string joystickId = "-J1";
	public string playerName;
	public GameObject hud;
	public GameObject wakeParticleSystem;
	public bool enableAI = false;
	public bool enableOptimalJib = false;
	public float aiLookAhead = 2;
	public Vector3 boatDir;
	
	public Vector3 sternPos;
	public Vector3 sailTipPos;
	public Vector3 pivotPos;
	
	
	GameObject sailForceGraphGO;
	GameObject fwForceGraphGO;
	GameObject velVecGraphGO;

	
	VectorLine jibLine;
	Material jibMaterial;
	
	public int numLapsComplete = 0;
	
	
	string lastTriggerName = "FWTrigger";
	bool enableLapCounter = false;
	
	 bool hasTriggerChanged = false;
	
	public float boatAngle = 180;
	float boatAngleVel = 0;
	public  Vector3 currentVel = Vector3.zero;
	
	int fwCursorID = -1;
	int sailCursorID = -1;
	
	int velWindID = -1;
	int velBoatID = -1;
	int velRelWindID;
	
	float jibAngle = 0;
	
	
	
	public float sailAngle = 0;
	float sailAngleGlob = 0;
	
	// Use this for initialization
	void Start () {
//		fwCursorID = fwForceGraphGO.GetComponent<UIGraph>().AddVCursor();
		fwForceGraphGO = hud.transform.FindChild("Graphs").FindChild ("FwForceGraph").gameObject;
		sailForceGraphGO = hud.transform.FindChild("Graphs").FindChild ("SailForceGraph").gameObject;
		velVecGraphGO = hud.transform.FindChild("Graphs").FindChild("VelocityVectorGraph").gameObject;
		
		sailCursorID = sailForceGraphGO.GetComponent<UIGraph>().AddVCursor();
		fwCursorID = fwForceGraphGO.GetComponent<UIGraph>().AddVCursor();
		
		jibMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		jibMaterial.color = Color.white;
		
		SetupVelGraph();
		
	}
	
	void Update(){
		hud.transform.FindChild("NameBox").GetComponent<Text>().text = playerName;
		if (GameMode.singleton.mode != GameMode.Mode.kRaceComplete){
			hud.transform.FindChild("LapInfo").GetComponent<Text>().text = "Lap " + (numLapsComplete + 1) +  " of " + GameConfig.singleton.totalNumLaps;
		}
		else{
			hud.transform.FindChild("LapInfo").GetComponent<Text>().text = "";
			if (GameMode.singleton.winningPlayer == gameObject){
				//hud.transform.FindChild("LapInfo").GetComponent<Text>().text = "Winner";
			}
			else{
			//	hud.transform.FindChild("LapInfo").GetComponent<Text>().text = "Loser";
			}
		}
		
		if (!GameMode.singleton.IsRacing()){
			wakeParticleSystem.GetComponent<ParticleSystem>().startSpeed = 0;
			wakeParticleSystem.GetComponent<ParticleSystem>().startLifetime = 0;
		}
		
		// SAIL
		if (!enableAI){
			float triggerValue = Input.GetAxis("RightTrigger" + joystickId);
			if (triggerValue != 0){
				hasTriggerChanged = true;
			}
			if (!hasTriggerChanged){
				triggerValue = -1;
			}
			float unitJibLength = 1 - 0.5f*(triggerValue + 1);
			jibAngle = 90 * (Mathf.Pow(unitJibLength, 2));
		}
		
		sailAngleGlob = sailGO.transform.rotation.eulerAngles.z;
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		
		// BOAT
		float power = -1;
		if (!enableAI){
			power = -100*Input.GetAxis("Horizontal" + joystickId);
		}
		else{		
			// Find a point on the rail ahead of where we are and draw it
			RailMaker.LocationData data = RailMaker.singleton.GetTrackLocation(GetComponent<Rigidbody2D>().position);
			RailMaker.LocationData datalookAhead = RailMaker.singleton.GetTrackLocation(data.dist + aiLookAhead);
			//DebugUtils.DrawCircle(datalookAhead.pos, 0.2f, Color.cyan);
			
			Vector3 desDir = (datalookAhead.pos - transform.position).normalized;
			float crossResult = Vector3.Cross (desDir, boatDir).z;
			power = -100*crossResult;
			
		}
		float speed = Vector3.Dot(boatDir, GetComponent<Rigidbody2D>().velocity);
		if (speed >= 0f){
			speed += 0.5f;
		}
		else{
			speed -= 0.5f;
		}
		float angleAccn = power * speed;
		boatAngleVel += angleAccn * Time.deltaTime;
		boatAngleVel *= 0.9f;
		boatAngle += boatAngleVel * Time.deltaTime;
		
		bodyGO.transform.rotation = Quaternion.Euler(0, 0, boatAngle);
		//GetComponent<Rigidbody2D>().AddTorque(angleAccn);
		GetComponent<SliderJoint2D>().angle = boatAngle+90;
		GetComponent<SliderJoint2D>().connectedAnchor = transform.position;	

		
		
//		if (GameConfig.singleton.showForceVisulisation) RenderRadialForceGraph();
//		CreateFwGraph();
		
		
		float wakeStrength = Mathf.Max (0, Vector3.Dot (GetComponent<Rigidbody2D>().velocity, boatDir));
		wakeParticleSystem.GetComponent<ParticleSystem>().startSpeed = 0.3f * wakeStrength;
		wakeParticleSystem.GetComponent<ParticleSystem>().startLifetime = 2f * wakeStrength;
		
		DrawJibRope(Mathf.Abs (sailAngle), jibAngle);
		DrawVelGraph();
		

	}
//	
//	void OldUpdate(){
//		hud.transform.FindChild("NameBox").GetComponent<Text>().text = playerName;
//		hud.transform.FindChild("LapInfo").GetComponent<Text>().text = "Lap " + (numLapsComplete + 1) +  " of " + GameConfig.singleton.totalNumLaps;
//		
//		if (!GameMode.singleton.IsRacing()){
//			wakeParticleSystem.GetComponent<ParticleSystem>().startSpeed = 0;
//			wakeParticleSystem.GetComponent<ParticleSystem>().startLifetime = 0;
//			
//			return;
//		}
//		
//		
//		
//		// SAIL
//		if (GameConfig.singleton.enableAutoSail){
//			sailAngle = CalcOptimalAngle();
//			sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
//		}
//		else{
//			if (GameConfig.singleton.enableJibSailControl){
//				float triggerValue = Input.GetAxis("RightTrigger" + joystickId);
//				if (triggerValue != 0){
//					hasTriggerChanged = true;
//				}
//				if (!hasTriggerChanged){
//					triggerValue = -1;
//				}
//				float unityJibLength = 1 - 0.5f*(triggerValue + 1);
//				jibAngle = 90 * (Mathf.Pow(unityJibLength, 2));
//				//Debug.Log (unityJibLength);
//				
//			}
//			else if (GameConfig.singleton.enable2DSailOrient){
//				Vector2 dir = new Vector2 (Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
//				if (dir.magnitude > 0.75){
//					dir.Normalize ();
//					//Debug.DrawLine(transform.position, transform.position + new Vector3(dir.x, dir.y, 0), Color.white);
//					sailAngle = 360 + 90 + Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
//					sailAngle = sailAngle % 360;
//					sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
//				}
//			}
//			else{		
//				if (GameConfig.singleton.enableAbsoluteSailTurn){
//					sailAngle = 90 * Input.GetAxis("Horizontal2");
//					sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
//				}
//				else{
//					float angleDelta = -2 * Input.GetAxis("Horizontal2");
//					sailAngle += angleDelta;
//					sailGO.transform.Rotate(0, 0, angleDelta);
//				}
//			}
//		}
//		sailAngleGlob = sailGO.transform.rotation.eulerAngles.z;
//		
//		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
//		
//		
//		// BOAT
//		if (!GameConfig.singleton.enableRail){
//			if (GameConfig.singleton.enable2DSailOrient){
//				Vector2 dir = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
//				if (dir.magnitude > 0.75){
//					dir.Normalize ();
//					//Debug.DrawLine(transform.position, transform.position + new Vector3(dir.x, dir.y, 0), Color.white);
//					boatAngle = 90 + Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
//					bodyGO.transform.rotation = Quaternion.Euler(0, 0, boatAngle);
//				}
//			}
//			else{
//				if (GameConfig.singleton.enableAbsoluteBoatTurn){
//					boatAngle = 90 * Input.GetAxis("Horizontal");
//					bodyGO.transform.rotation = Quaternion.Euler(0, 0, boatAngle);
//				}
//				else{
//					float power = -100*Input.GetAxis("Horizontal" + joystickId);
//					float speed = 0.25f + Vector3.Dot(boatDir, GetComponent<Rigidbody2D>().velocity);
//					float angleAccn = power * speed;
//					boatAngleVel += angleAccn * Time.deltaTime;
//					boatAngleVel *= 0.9f;
//					boatAngle += boatAngleVel * Time.deltaTime;
//					
//					bodyGO.transform.rotation = Quaternion.Euler(0, 0, boatAngle);
//					//GetComponent<Rigidbody2D>().AddTorque(angleAccn);
//					GetComponent<SliderJoint2D>().angle = boatAngle+90;
//					GetComponent<SliderJoint2D>().connectedAnchor = transform.position;
//				}
//				
//			}
//			
//			
//		}
//		
//		
//		if (GameConfig.singleton.showForceVisulisation) RenderRadialForceGraph();
//		CreateFwGraph();
//		
//		
//		float wakeStrength = Mathf.Max (0, Vector3.Dot (GetComponent<Rigidbody2D>().velocity, boatDir));
//		wakeParticleSystem.GetComponent<ParticleSystem>().startSpeed = 0.3f * wakeStrength;
//		wakeParticleSystem.GetComponent<ParticleSystem>().startLifetime = 2f * wakeStrength;
//		//Debug.Log(Vector3.Dot (GetComponent<Rigidbody2D>().velocity, boatDir));
//	}
//	
	void DrawJibRope(float sailAngle, float jibAngle){
		sternPos = bodyGO.transform.TransformPoint(new Vector3(0, 0.5f, 0));
		sailTipPos = sailGO.transform.TransformPoint(new Vector3(0, 0.8f, 0));
		pivotPos = sailGO.transform.TransformPoint(new Vector3(0, 0f, 0));
		
		float slack = 0.002f * (jibAngle - sailAngle);
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
		
		if (jibLine == null){
			jibLine = new VectorLine("Cursor", drawPoints, null, 2.0f, LineType.Continuous);
			
		}
		else{
			for (int i = 0; i < drawPoints.Count(); ++i){
				jibLine.points3[i] = drawPoints[i];
			}
			
		}
		jibLine.Draw3D ();


				
	}
	

	
	// Update is called once per frame
	void FixedUpdate () {
		
		currentVel = GetComponent<Rigidbody2D>().velocity;
		// calc current Vel;
		boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		if (enableOptimalJib){
			jibAngle = CalcOptimalJib();
		}
		
		sailAngle = ConvertJibToSailAngle(jibAngle);
		

		
		sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle);
			
		
		Vector3 sailForce;
		Vector3 fwForce;
		Vector3 windForce;
		CalcVectors(sailAngleGlob, currentVel, boatDir, out windForce, out sailForce, out fwForce);
		
		
		Quaternion sailRot = Quaternion.Euler(0, 0, sailAngleGlob);
		Vector3 saleNormal = sailRot * new Vector3(1, 0, 0);
		
		sailForceGraphGO.GetComponent<UIGraph>().SetVCursor(sailCursorID, new Vector2(sailAngle, Vector3.Dot (saleNormal, sailForce)));
		fwForceGraphGO.GetComponent<UIGraph>().SetVCursor(fwCursorID, new Vector2(sailAngle, Vector3.Dot (fwForce, boatDir)));
		
		
		
		// Work out force from wind pushing against boat
		Vector3 boatForce = boatWindStrength * boatDir * Vector3.Dot (windForce, boatDir);
	
		
		// Calc accn
		
		
		GetComponent<Rigidbody2D>().AddForce(boatForce + fwForce);

		
		if (!GameMode.singleton.IsRacing()){
			if (GameMode.singleton.mode != GameMode.Mode.kRaceComplete){
				GetComponent<Rigidbody2D>().velocity = Vector3.zero;
				currentVel = Vector3.zero;
			}
		}
		
		DrawSailGraph();
		
	}
	
	
	void SetupVelGraph(){
		velWindID = velVecGraphGO.GetComponent<UIVectorGraph>().AddVector(Color.cyan);
		velBoatID = velVecGraphGO.GetComponent<UIVectorGraph>().AddVector(Color.cyan);
		velRelWindID = velVecGraphGO.GetComponent<UIVectorGraph>().AddVector(Color.cyan);
	}
	
	Vector2 TransformVectorToBoatSpace(Vector2 vec){
		Vector2 retVec = bodyGO.transform.InverseTransformVector(vec);
		retVec = Quaternion.Euler(0, 0, 180) * retVec;
		return retVec;
	}
	
	void DrawVelGraph(){
		velVecGraphGO.GetComponent<UIVectorGraph>().SetAxesRanges(-5, 5, -5, 5);
		
		Vector2 winVelLoc = TransformVectorToBoatSpace(new Vector2(Environment.singleton.windVel.x, Environment.singleton.windVel.y));
		Vector2 boatVelLoc = TransformVectorToBoatSpace(GetComponent<Rigidbody2D>().velocity);
		Vector2 relWindVelLocStart = boatVelLoc;
		Vector2 relWindVelLocEnd = winVelLoc;
		
		velVecGraphGO.GetComponent<UIVectorGraph>().SetVector(velWindID, Vector2.zero, winVelLoc);
		velVecGraphGO.GetComponent<UIVectorGraph>().SetVector(velBoatID, Vector2.zero, boatVelLoc);
		velVecGraphGO.GetComponent<UIVectorGraph>().SetVector(velRelWindID, relWindVelLocStart, relWindVelLocEnd);
		//velVecGraphGO
		
	}
	
	void DrawSailGraph(){
		UIGraph sailGraph = sailForceGraphGO.GetComponent<UIGraph>();
		UIGraph fwGraph = fwForceGraphGO.GetComponent<UIGraph>();
		
		Vector2[] sailPoints = new Vector2[180];
		Vector2[] fwPoints = new Vector2[180];
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		for (int i = -90; i < 90; i++){
			float testJibAngle = (float)i;
			
			float testSailAngle = testJibAngle;
			Vector3 sailForce;
			Vector3 fwForce;
			Vector3 windForce;
			float useSailAngle = sailAngleGlob - sailAngle + testSailAngle;
			
			Quaternion sailRot = Quaternion.Euler(0, 0, useSailAngle);
			Vector3 saleNormal = sailRot * new Vector3(1, 0, 0);
			
			CalcVectors(useSailAngle, currentVel, boatDir, out windForce, out sailForce, out fwForce);
			//			points[i] = Vector3.Dot(fwForce, boatDir);;
			sailPoints[i+90] = new Vector2(testSailAngle, Vector3.Dot(sailForce, saleNormal));
			fwPoints[i+90] = new Vector2(testSailAngle, Vector3.Dot(fwForce, boatDir));
			
		}
		sailGraph.SetAxesRanges(-90, 90, -8, 8);
		sailGraph.UploadData(sailPoints);
		fwGraph.SetAxesRanges(-90, 90, -8, 8);
		fwGraph.UploadData(fwPoints);
		
	}
	
	float CalcOptimalJib(){
		float maxPower = 0;
		float maxPowerAngle = -1;
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
//		Debug.Log("sailAngleGlob - sailAngle = " + (sailAngleGlob -sailAngle));
		int numSteps = 360;
		for (int i = 0; i < 90; i++){
			float testJibAngle = 90f * (float)i / (float)(numSteps-1);
		
			float testSailAngle = ConvertJibToSailAngle(testJibAngle);
			Vector3 sailForce;
			Vector3 fwForce;
			Vector3 windForce;
			
			
			CalcVectors(sailAngleGlob - sailAngle + testSailAngle, currentVel, boatDir, out windForce, out sailForce, out fwForce);
			float thisPower = Vector3.Dot(fwForce, boatDir);
			if (thisPower > maxPower){
				maxPowerAngle = testJibAngle;
				maxPower = thisPower;
			}
		}
//		Debug.Log("maxPowerAngle = " + maxPowerAngle);
		return maxPowerAngle;
	}
	
	float ConvertJibToSailAngle(float jibAngle){
		
		// Work out the maximum/minimum angle the wind would be blowing it
		Vector3 sailNormal = sailGO.transform.rotation * new Vector3(1, 0, 0);
		Vector3 relWindVel = Environment.singleton.windVel - currentVel;
		Vector3 windForceLoc = (relWindVel * sailStrength).normalized; 
		float dotResult = Vector3.Dot (sailNormal, windForceLoc);
		// Clamp to -1, +1
		dotResult = Mathf.Min (1, Mathf.Max (-1, dotResult));
		float maxAngle = sailAngle - Mathf.Rad2Deg * Mathf.Asin(dotResult);
		
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, 1, 0);
		Vector3 sailNormalDir = sailNormal + boatDir * 0.0001f;
		float dotResultDir = Vector3.Dot (sailNormalDir, windForceLoc);
//		
//		// figure out which way the wind is blowing accros the boat
//		Vector3 boatSideDir = bodyGO.transform.rotation * new Vector3(1, 0, 0);
//		float boatDot = Vector3.Dot(boatSideDir, relWindVel);
//		
//		float boatCross = Vector3.Cross(boatDir, relWindVel).z;
		
		//Debug.Log ("boatDot, boatCross = " + boatDot + ", " + boatCross);
		
		
		//			Debug.Log (jibAngle);
		if (dotResultDir > 0){
			return Mathf.Max (-jibAngle, maxAngle);
		}
		else{
			return Mathf.Min (jibAngle, maxAngle);
		}
	}
	
	void CalcVectors(float sailAngleLoc, Vector3 boatVel, Vector3 boatFwDir, out Vector3 windForce, out Vector3 sailForce, out Vector3 fwForce){
	
		Quaternion sailOrient = Quaternion.Euler(0, 0, sailAngleLoc);
		Vector3 saleNormal = sailOrient * new Vector3(1, 0, 0);
		
		
		Vector3 relWindVel = Environment.singleton.windVel - boatVel;
		windForce = relWindVel * sailStrength; 
		
		sailForce = saleNormal * Vector3.Dot(windForce, saleNormal);
		fwForce = boatFwDir * Vector3.Dot (sailForce, boatFwDir);
	}
	/*
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
	*/
	
	float CalcOptimalAngle(){
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
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
	
//	
//	void CreateFwGraph(){
//		UIGraph fwGraph = fwForceGraphGO.GetComponent<UIGraph>();
//		float maxWindForce = Environment.singleton.windVel.magnitude * sailStrength;
//		fwGraph.SetAxesRanges(-180, 180, -maxWindForce, maxWindForce);
//		
//		UIGraph sailGraph = sailForceGraphGO.GetComponent<UIGraph>();
//		sailGraph.SetAxesRanges(-180, 180, -maxWindForce, maxWindForce);
//		
//		
//		
//		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
//		Vector3 currentVel = speed * boatDir;
//		
//		Vector3 sailForce;
//		Vector3 fwForce;
//		Vector3 windForce;
//		
//		Vector2[] fwData = new Vector2[360];
//		Vector2[] sailData = new Vector2[360];
//		for (int i = 0; i < 360; ++i){
//			float x = (float)i - 180f;
//			
//			CalcVectors(x, currentVel, boatDir, out windForce, out sailForce, out fwForce);
//			
//			float y = Vector3.Dot (fwForce, boatDir);
//			fwData[i] = new Vector2(-x, y);
//			sailData[i] = new Vector2(-x, sailForce.magnitude);
//		}
//		fwGraph.UploadData(fwData);
//		sailGraph.UploadData(sailData);
//		
//		CalcVectors(sailAngleGlob, currentVel, boatDir, out windForce, out sailForce, out fwForce);
//		
//		fwGraph.SetVCursor(fwCursorID, new Vector2(180-sailAngle, Vector3.Dot (fwForce, boatDir)));
//		sailGraph.SetVCursor(sailCursorID, new Vector2(180-sailAngle, sailForce.magnitude));
////		
//		
//	}
	
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
		
		//Debug.Log ("triggerName = " + triggerName + ", lastTriggerName = " + lastTriggerName);
		
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
