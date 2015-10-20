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
	public string playerName;
	public GameObject hud;
	public GameObject wakeParticleSystem;
	public float aiLookAhead = 2;
	public Vector3 boatDir = new Vector3(0, 1, 0);
	public Vector3 sailForceGlob = Vector3.zero;
	public Vector3 fwForceGlob = Vector3.zero;
	public Material jibLineMaterial;
	
	public Vector3 sternPos;
	public Vector3 sailTipPos;
	public Vector3 pivotPos;
	
	public Vector2[] sailPoints;
	public Vector2[] fwPoints;
	public int numGraphPoints = 360;
	public AudioSource wind;
	public AudioSource waves;
	public AudioSource crash;
	public bool lockPosition = false;
	public bool disableJib = false;
	public bool disableRudder = false;
	public bool overrideControls = false;
	public float maxSpeed = -1;
	
	public enum InputMethod{
		kNone,	
		kJoystick,
		kKeyboardAndMouse,
		kAI
	}
	
	public InputMethod	inputMethod = InputMethod.kJoystick;
	
	public string joystickId;
	
	float removeSliderTime = -100;
	float removeSliderDuration = 0.5f;
	
	// Steering control vals
	float lastJoystickVal = 0;
	float power = 0;
	
	
	
	GameObject sailForceGraphGO;
	GameObject fwForceGraphGO;
	GameObject velVecGraphGO;

	
	VectorLine jibLine;
	Material jibMaterial;
	
	public int numLapsComplete = 0;
	
	
	string lastTriggerName = "FWTrigger";
	bool enableLapCounter = false;
	
	bool hasTriggerChangedOSX = false;
	bool hasTriggerChangedPC = false;
	
	public float boatAngle = 180;
	float boatAngleVel = 0;
	public  Vector3 currentVel = Vector3.zero;
	
//	int fwCursorID = -1;
//	int sailCursorID = -1;
//	
//	int velWindID = -1;
//	int velBoatID = -1;
//	int velRelWindID;
	
	float jibAngle = 0;
//	bool sailJibError = false;
	
	int audioPlayCount = 10;
	public bool isSailWobbling;
	public float wobbleStartTime = 0;
	
	
	
	public float sailAngle = 0;
	float sailAngleGlob = 0;
	
	public bool IsEnableAI(){
		return inputMethod == InputMethod.kAI || overrideControls;
	}
	
	// Use this for initialization
	void Start () {


//		fwCursorID = fwForceGraphGO.GetComponent<UIGraph>().AddVCursor();
//		fwForceGraphGO = hud.transform.FindChild("Graphs").FindChild ("FwForceGraph").gameObject;
//		sailForceGraphGO = hud.transform.FindChild("Graphs").FindChild ("SailForceGraph").gameObject;
//		velVecGraphGO = hud.transform.FindChild("Graphs").FindChild("VelocityVectorGraph").gameObject;
		
//		sailCursorID = sailForceGraphGO.GetComponent<UIGraph>().AddVCursor();
//		fwCursorID = fwForceGraphGO.GetComponent<UIGraph>().AddVCursor();
		
		jibMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		jibMaterial.color = Color.white;
		
		SetupVelGraph();
		
		sailPoints = new Vector2[numGraphPoints];
		fwPoints = new Vector2[numGraphPoints];
		
		// If player 1
		string key = "";
		if (name == "Player_1"){
			key = "Player1";
		}
		else if (name == "Player_2"){
			key = "Player2";
		}
		
		if (key != ""){
			string inputMethodString = PlayerPrefs.GetString(key);
			switch (inputMethodString){
				case "Joystick":{
					inputMethod = InputMethod.kJoystick;
					joystickId = "Junk";
					break;
				}
				case "Keyboard":{
					inputMethod = InputMethod.kKeyboardAndMouse;
					break;
				}
				case "AI":{
					inputMethod = InputMethod.kAI;
					if (name == "Player_1"){
						playerName = "Computer 1";
					}
					else if (name == "Player_2"){
						playerName = "Computer 2";
					}
					break;
				}
				default:{
					DebugUtils.Assert (false, "Failed to recoginse input method");
					break;
				}
			}
		}

		
		
		
	}
	
	void Update(){
		if (audioPlayCount-- == 0){
			if (wind != null){
				wind.Play();
				wind.volume = 0;
			}
			if (waves!= null){
				waves.Play();
				waves.volume = 0;
			}
		}

		if (hud != null){		
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
		}
		
		if (!GameMode.singleton.IsRacing()){
			wakeParticleSystem.GetComponent<ParticleSystem>().startSpeed = 0;
			wakeParticleSystem.GetComponent<ParticleSystem>().startLifetime = 0;
		}
		
		// SAIL
		if (!IsEnableAI() && joystickId != "Junk"){
			if (inputMethod == InputMethod.kJoystick){
				float triggerValueOSX = Input.GetAxis("OSX_RightTrigger" + joystickId);
				float triggerValuePC = Input.GetAxis("PC_RightTrigger" + joystickId);


				if (triggerValueOSX != 0){
					hasTriggerChangedOSX = true;
					hasTriggerChangedPC = false;
				}
				if (!hasTriggerChangedOSX){
					triggerValueOSX = -1;
				}
				if (triggerValuePC != 0){
					hasTriggerChangedPC = true;
					hasTriggerChangedOSX = false;
				}
				if (!hasTriggerChangedPC){
					triggerValuePC = -1;
				}				
				float triggerValue = triggerValueOSX + triggerValuePC + 1;
				triggerValue = Mathf.Clamp(triggerValue, -1, 1);
				if (disableJib){
					triggerValue= -1;
				}
								
				float unitJibLength = 1 - 0.5f*(triggerValue + 1);
				jibAngle = 90 * (Mathf.Pow(unitJibLength, 2));
			}
			else if (inputMethod == InputMethod.kKeyboardAndMouse){
				float mouseDelta = Input.GetAxis("Mouse Y");
				jibAngle += 5 * mouseDelta;
				jibAngle = Mathf.Min (90, Mathf.Max (0, jibAngle));
				if (disableJib){
					jibAngle= 90;
				}
				
				//Debug.Log ("mouseDelta = " + mouseDelta);
			}
		}
		
		sailAngleGlob = bodyGO.transform.rotation.eulerAngles.z + sailAngle;
		 
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		
		// BOAT
		if (!IsEnableAI() && joystickId != "Junk"){
			if (GameMode.singleton.IsRacing()){
				if (inputMethod == InputMethod.kJoystick){
					float joystickVal = Input.GetAxis("Horizontal" + joystickId);
					if (joystickVal != lastJoystickVal){
						lastJoystickVal = joystickVal;
						power = -100 * joystickVal;
					}
				}
				else if (inputMethod == InputMethod.kKeyboardAndMouse){
					bool left = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
					bool right = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
					power += left ? 10 : 0;
					power -= right ? 10 : 0;
					
					if (!left && !right && Mathf.Abs (power) > 0.1f){
						if (power > 0){
							power -= 10;
						}
						else{
							power += 10;
						}
					}
					
					power = Mathf.Max (-100, Mathf.Min (100, power));
				}
				
				if (disableRudder){
					power = 0;
				}
			
				

				//Debug.Log ("Power = " + power);
			}
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
		if (speed >= -0.25f){
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
			jibLine = new VectorLine("Cursor", drawPoints, jibLineMaterial, 2.0f, LineType.Continuous);
			
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
	
		if (lockPosition){
			GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
		}
		else{
			GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
		}

		if (wind != null){
			Vector3 relWindVel = Environment.singleton.windVel - currentVel;
			float relWindSpeed = relWindVel.magnitude;
			float windSpeedProp = 0.75f * relWindSpeed/Environment.singleton.windVel.magnitude;
			wind.pitch = Mathf.Lerp (0.5f, 2.0f, windSpeedProp);
			wind.volume = Mathf.Lerp (0.25f, 1f, windSpeedProp);
			
			// Direction
			Vector3 relWindDir = relWindVel.normalized;
			float crossRes = Vector3.Cross(boatDir, relWindDir).z;
			wind.panStereo = 0.75f * crossRes;
			
		}
		
		if (waves != null){
			float boatSpeed = currentVel.magnitude;
			float boatSpeedPro = boatSpeed/Environment.singleton.windVel.magnitude;
			waves.pitch = Mathf.Lerp (0.5f, 2.5f, boatSpeedPro);
			waves.volume = Mathf.Lerp (0.0f, 0.75f, boatSpeedPro);
			
		}

		// If we've been bumped, then remove the slider	
		GetComponent<SliderJoint2D>().enabled = (Time.fixedTime > removeSliderTime + removeSliderDuration);
		
		
		currentVel = GetComponent<Rigidbody2D>().velocity;
		// calc current Vel;
		boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		if (IsEnableAI()){
			jibAngle = CalcOptimalJib();
		}
		
		sailAngle = ConvertJibToSailAngle(jibAngle);
		
//		sailJibError = jibAngle < Mathf.Abs (sailAngle) -  1f;
//		
//		if (sailJibError){
//			sailAngle = ConvertJibToSailAngle(jibAngle);
//		}
		
		
		// If the two are different, then rope is loose
		if (Mathf.Abs(Mathf.Abs(sailAngle) - Mathf.Abs (jibAngle)) > 0.001f){
			if (!isSailWobbling){
				isSailWobbling = true;
				wobbleStartTime = Time.fixedTime;
			}
		}
		else{
			isSailWobbling = false;
		}
		
		float wobbleValue = 0;
		if (isSailWobbling){
			wobbleValue = Mathf.Sin(50 * (Time.fixedTime - wobbleStartTime));
		}
	
		sailGO.transform.localRotation = Quaternion.Euler(0, 0, sailAngle + 3 * wobbleValue);
			
		Vector3 windForce;
		CalcVectors(sailAngleGlob, currentVel, boatDir, out windForce, out sailForceGlob, out fwForceGlob);
		
		
//		Quaternion sailRot = Quaternion.Euler(0, 0, sailAngleGlob);
//		Vector3 saleNormal = sailRot * new Vector3(1, 0, 0);
		
//		sailForceGraphGO.GetComponent<UIGraph>().SetVCursor(sailCursorID, new Vector2(sailAngle, Vector3.Dot (saleNormal, sailForceGlob)));
//		fwForceGraphGO.GetComponent<UIGraph>().SetVCursor(fwCursorID, new Vector2(sailAngle, Vector3.Dot (fwForceGlob, boatDir)));
//		Debug.Log ("Vector3.Dot (fwForceGlob, boatDir) = " + Vector3.Dot (fwForceGlob, boatDir));
		HandleSailSound();
		
		
		
		
		// Work out force from wind pushing against boat
		Vector3 boatForce = boatWindStrength * boatDir * Vector3.Dot (windForce, boatDir);
	
		
		// Calc accn
		float currentSpeed = currentVel.magnitude;
		Vector3 maxSpeedForce = Vector3.zero;
		if (maxSpeed >= 0 && currentSpeed > maxSpeed && Vector3.Dot(currentVel, boatDir) > 0){
			maxSpeedForce = -2f * (boatForce + fwForceGlob);
		}

		
		
		if (!float.IsNaN((boatForce + fwForceGlob).x)){
			GetComponent<Rigidbody2D>().AddForce(boatForce + fwForceGlob + maxSpeedForce);
		}
		else{
			Debug.Log("Error: NAN force");
			return;
		}

		
		if (!GameMode.singleton.IsRacing()){
			if (GameMode.singleton.mode != GameMode.Mode.kRaceComplete){
				GetComponent<Rigidbody2D>().velocity = Vector3.zero;
				currentVel = Vector3.zero;
			}
		}
		
		DrawSailGraph();
		
	}
	
	
	void SetupVelGraph(){
//		velWindID = velVecGraphGO.GetComponent<UIVectorGraph>().AddVector(Color.cyan);
//		velBoatID = velVecGraphGO.GetComponent<UIVectorGraph>().AddVector(Color.cyan);
//		velRelWindID = velVecGraphGO.GetComponent<UIVectorGraph>().AddVector(Color.cyan);
	}
	
	Vector2 TransformVectorToBoatSpace(Vector2 vec){
		Vector2 retVec = bodyGO.transform.InverseTransformVector(vec);
		retVec = Quaternion.Euler(0, 0, 180) * retVec;
		return retVec;
	}
	
	void DrawVelGraph(){
//		velVecGraphGO.GetComponent<UIVectorGraph>().SetAxesRanges(-5, 5, -5, 5);
//		
//		Vector2 winVelLoc = TransformVectorToBoatSpace(new Vector2(Environment.singleton.windVel.x, Environment.singleton.windVel.y));
//		Vector2 boatVelLoc = TransformVectorToBoatSpace(GetComponent<Rigidbody2D>().velocity);
//		Vector2 relWindVelLocStart = boatVelLoc;
//		Vector2 relWindVelLocEnd = winVelLoc;
//		
//		velVecGraphGO.GetComponent<UIVectorGraph>().SetVector(velWindID, Vector2.zero, winVelLoc);
//		velVecGraphGO.GetComponent<UIVectorGraph>().SetVector(velBoatID, Vector2.zero, boatVelLoc);
//		velVecGraphGO.GetComponent<UIVectorGraph>().SetVector(velRelWindID, relWindVelLocStart, relWindVelLocEnd);
		//velVecGraphGO
		
	}
	
	void DrawSailGraph(){
//		UIGraph sailGraph = sailForceGraphGO.GetComponent<UIGraph>();
//		UIGraph fwGraph = fwForceGraphGO.GetComponent<UIGraph>();
		
		
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		
		for (int i = -180; i < 180; i++){
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
			sailPoints[i+180] = new Vector2(testSailAngle, Vector3.Dot(sailForce, saleNormal));
			fwPoints[i+180] = new Vector2(testSailAngle, Vector3.Dot(fwForce, boatDir));
			
		}
	//	Debug.Log ("maxfw  =" + maxfw + "minfw  =" + minfw);
//		sailGraph.SetAxesRanges(-90, 90, -8, 8);
//		sailGraph.UploadData(sailPoints);
//		fwGraph.SetAxesRanges(-90, 90, -8, 8);
//		fwGraph.UploadData(fwPoints);
		
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
		Vector3 sailNormal = Quaternion.Euler(0, 0, sailAngle) * bodyGO.transform.rotation * new Vector3(1, 0, 0);
		Vector3 relWindVel = Environment.singleton.windVel - currentVel;
		Vector3 windForceLoc = (relWindVel * sailStrength).normalized; 
		float dotResult = Vector3.Dot (sailNormal, windForceLoc);
		// Clamp to -1, +1
		dotResult = Mathf.Min (1, Mathf.Max (-1, dotResult));
		float maxAngle = sailAngle - Mathf.Rad2Deg * Mathf.Asin(dotResult);
		
		
		Vector3 boatSideDir = bodyGO.transform.rotation * new Vector3(1, 0, 0);
//		Vector3 sailNormalDir = sailNormal + boatDir * 0.0001f;
		float dotResultDir = Vector3.Dot (boatSideDir, windForceLoc);
		
//		Debug.DrawLine(bodyGO.transform.position, bodyGO.transform.position + boatSideDir, Color.red);
//		Debug.DrawLine(bodyGO.transform.position, bodyGO.transform.position + windForceLoc, Color.green);
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

	void HandleSailSound(){
		if (transform.FindChild("SailSound") == null) return;
		
		AudioLowPassFilter filter = transform.FindChild("SailSound").GetComponent<AudioLowPassFilter>();
		
		// Get the range of values for the fwForce
		float minForce = 1000;
		float maxForce = -10000;
		for (int i = 0; i < fwPoints.Count (); ++i){
			minForce = Mathf.Min (minForce, fwPoints[i].y);
			maxForce = Mathf.Max (maxForce, fwPoints[i].y);
		}
		
		minForce = Mathf.Max (0, minForce);
		
		// Get this force
		Vector3 boatDir = bodyGO.transform.rotation * new Vector3(0, -1, 0);
		float thisForce =  Vector3.Dot(fwForceGlob, boatDir);

		
		float propForce = (thisForce - minForce) / (maxForce - minForce);
		
		propForce = Mathf.Pow(propForce, 5);
//		propForce = propForce;
		//Debug.Log ("propForce = " + propForce);
		
		
		filter.lowpassResonanceQ = Mathf.Lerp (1, 2.5f, propForce);
		
		
		// Do the frequency
		float propSail = sailForceGlob.magnitude/6f;
		filter.cutoffFrequency = Mathf.Lerp (500, 2000, propSail);
		transform.FindChild("SailSound").GetComponent<AudioSource>().volume = Mathf.Lerp (0.21f, 0.5f, propSail);
		
		//Debug.Log ("propSail = " + propSail);
		
		
	}
	
	void OnTriggerStartLine(){
		if (numLapsComplete == GameConfig.singleton.totalNumLaps){
			GameMode.singleton.TriggerWinner(gameObject);
		}
	}
	
	
	void OnCollisionAction(){
		removeSliderTime = Time.fixedTime;
		
		GetComponent<SliderJoint2D>().enabled = false;
		GetComponent<Rigidbody2D>().velocity = currentVel;
	}
	
	void OnCollisionEnter2D(Collision2D collision){
		if (crash != null && !crash.isPlaying) crash.Play ();
	
		OnCollisionAction();
	}
	
	void OnCollisionStay2D(Collision2D collision){
		
		//OnCollisionAction();
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
	
//	void OnGUI(){
//		GUI.Label(new Rect(10, 10, 500, 50), "jib = " + jibAngle + ", sail = " + sailAngle + (sailJibError ? " - Error" : "") );
//	}

	
}
