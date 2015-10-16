using UnityEngine;
using System.Collections;
using System.Linq;
using Vectrosity;

public class PlayerOverlay : MonoBehaviour {

	public GameObject playerGO;
	public GameObject cameraGO;
	public Color mainCircleCol;
	public Color windDirCol;
	public Color sailDirCol;
	public Color boatDirCol;
	public float scaleAsPropOfScreenWidth= 0.1f;

	public float rightAngleBoxSizeProp = 0.01f;
	public float transparencyArrows = 1f;
	public float transparencyProjections = 1f;
	public float transparencyMainCircle = 1f;
	public float transparencyDotted = 1f;
	public float transparencyFwForce = 1f;
	public float globalTransparency = 0f;
	public bool hideOverlay = false;
	
		
	Color transparencyColArrows;
	Color transparencyColProjections;
	Color transparencyColMainCircle;
	Color transparencyColDotted;	
	Color transparencyColFwForce;
	
	
	Player player;
	
	// Main circle
	VectorLine mainCircleLine;
	int numPointsInCircle = 100;
	Vector2[] mainCirclePoints;
	float mainCircleThickness = 20;
	
	
	// Dotted lines
	VectorLine windDirLine;
	VectorLine sailDirLine;
	VectorLine boatDirLine;
	
	// Arrows
	VectorLine windForceLine;
	VectorLine sailForceLine;
	VectorLine fwForceLine;
	
	// Projection lines
	VectorLine windToSailForceFrom;
	VectorLine windToSailForceTo;
	
	VectorLine sailToFwForceFrom;
	VectorLine sailToFwForceTo;
	
	// Right angle symbols
	VectorLine windToSailRA;
	VectorLine sailToFwRA;
	
	// Cost functin
	VectorLine costFunctionLine;
	
	


	void ConstructLines(){
	
		// Main circle
		mainCirclePoints = new Vector2[numPointsInCircle];
		for (int i = 0; i < numPointsInCircle; ++i){
			float angleRad = 2 * Mathf.PI * (float)i / (float)(numPointsInCircle-1);
			mainCirclePoints[i] = new Vector2(Mathf.Sin (angleRad), Mathf.Cos(angleRad));
		}
		mainCircleLine = new VectorLine("Main Circle", mainCirclePoints, UI.singleton.mainCircleMaterial, mainCircleThickness, LineType.Continuous, Joins.Weld);
		
		// Dummy points, used for all straight lines
		Vector2[] dummyPoints2 = new Vector2[2];
		
		// Dotted lines
		windDirLine = new VectorLine("Wind Dir", dummyPoints2, UI.singleton.smallDottedLineMaterial, 5);
		windDirLine.textureScale = 1;
		
		sailDirLine = new VectorLine("Sail Dir", dummyPoints2, UI.singleton.smallDottedLineMaterial, 5);
		sailDirLine.textureScale = 1;
		
		boatDirLine = new VectorLine("Boat Dir", dummyPoints2, UI.singleton.smallDottedLineMaterial, 5);
		boatDirLine.textureScale = 1;
		

		
		// Arrows
		windForceLine = new VectorLine("Wind Force Arrow", dummyPoints2, UI.singleton.arrowMaterial, 10);
		windForceLine.endCap = "rounded_arrow";
		
		sailForceLine = new VectorLine("Sail Force Arrow", dummyPoints2, UI.singleton.arrowMaterial, 10);
		sailForceLine.endCap = "rounded_arrow";
		
		fwForceLine = new VectorLine("Forward Force Arrow", dummyPoints2, UI.singleton.arrowMaterial, 10);
		fwForceLine.endCap = "rounded_arrow";
		
		// projection lines
		windToSailForceFrom = new VectorLine("WindToSailFrom", dummyPoints2, UI.singleton.gradiatedLineMaterial, 5);
		windToSailForceFrom.endCap = "gradiated_end";
		
		windToSailForceTo = new VectorLine("WindToSailTo", dummyPoints2, UI.singleton.gradiatedLineMaterial, 5);
		windToSailForceTo.endCap = "gradiated_end";
		
		
		sailToFwForceFrom = new VectorLine("sailToFwForceFrom", dummyPoints2, UI.singleton.gradiatedLineMaterial, 5);
		sailToFwForceFrom.endCap = "gradiated_end";
		
		sailToFwForceTo = new VectorLine("sailToFwForceTo", dummyPoints2, UI.singleton.gradiatedLineMaterial, 5);
		sailToFwForceTo.endCap = "gradiated_end";
		
		// Right angle symbols
		Vector2[] dummyPoints3 = new Vector2[3];
		windToSailRA = new VectorLine("windToSailRA", dummyPoints3, UI.singleton.arrowMaterial, 5, LineType.Continuous, Joins.Weld);
		
		sailToFwRA = new VectorLine("windToSailRA", dummyPoints3, UI.singleton.arrowMaterial, 5, LineType.Continuous, Joins.Weld);

		
		// Cost function
//		Vector2[] dummyPoints = new Vector2[player.numGraphPoints];
//		costFunctionLine = new VectorLine("costFunctionLine", dummyPoints, UI.singleton.middleGradiatedMaterial, 5, LineType.Continuous, Joins.Weld);
	}
	
	
	
	void DrawAll(){
	
		if (player.IsEnableAI() || GameMode.singleton.mode == GameMode.Mode.kSignalOff || GameMode.singleton.mode == GameMode.Mode.kGetJoystick || hideOverlay){
			globalTransparency = 0;
		}
		else{
			globalTransparency = 1.25f;
			
		}
		
		transparencyColArrows = new Color(1, 1, 1, transparencyArrows * globalTransparency);
		transparencyColProjections = new Color(1, 1, 1, transparencyProjections * globalTransparency);
		transparencyColMainCircle = new Color(1, 1, 1, transparencyMainCircle * globalTransparency);
		transparencyColDotted = new Color(1, 1, 1, transparencyDotted * globalTransparency);
		transparencyColFwForce =new Color(1, 1, 1, transparencyFwForce * globalTransparency);

		// Main circle
		Camera thisCam = cameraGO.GetComponent<Camera>();
		Vector2 centrePos = thisCam.WorldToScreenPoint(player.pivotPos);
		float radius = scaleAsPropOfScreenWidth * Screen.width + 0.5f * mainCircleThickness;
		
		for (int i = 0; i < numPointsInCircle; ++i){
			mainCircleLine.points2[i] = centrePos + radius * mainCirclePoints[i];
		}
		mainCircleLine.color = mainCircleCol * transparencyColMainCircle;
		mainCircleLine.Draw ();
		
		
		// Figure out the scale so that the wind vel realtive to a stationary ship 
		// reaches the radius
		float visScale = radius / Environment.singleton.windVel.magnitude;
		
		
		/* Dotted Lines */

		// Relative wind Dir
		Quaternion invBoatRot = Quaternion.Euler(0, 0, -player.boatAngle);
		Vector2 relWindVel = Environment.singleton.windVel - player.currentVel;
		Vector2 relWindVelLoc = invBoatRot * relWindVel;
		Vector2 relWindDir = relWindVelLoc.normalized;
		Vector2 windDirStartPos = centrePos + radius * relWindDir;
		Vector2 windDirEndPos = centrePos - radius * relWindDir;
		
		windDirLine.points2[0] = windDirStartPos;
		windDirLine.points2[1] = windDirEndPos;
		windDirLine.color = windDirCol * transparencyColDotted;
		windDirLine.Draw ();
		
		// Sail Dir
		Quaternion sailRot = Quaternion.Euler(0, 0, player.sailAngle);
		Vector2 sailVecLoc = sailRot * new Vector2(1, 0); 
		Vector2 sailDir = sailVecLoc.normalized;
		Vector2 sailDirStartPos = centrePos + radius * sailDir;
		Vector2 sailDirEndPos = centrePos - radius * sailDir;
		sailDirLine.points2[0] = sailDirStartPos;
		sailDirLine.points2[1] = sailDirEndPos;
		sailDirLine.color = sailDirCol * transparencyColDotted;
		sailDirLine.Draw ();
		
		/// Boat Dir
		Vector2 boatDirStartPos = centrePos + radius * new Vector2(0, 1);
		Vector2 boatDirEndPos = centrePos - radius * new Vector2(0, 1);
		boatDirLine.points2[0] = boatDirStartPos;
		boatDirLine.points2[1] = boatDirEndPos;
		boatDirLine.color = boatDirCol * transparencyColDotted;
		boatDirLine.Draw ();
		
		/* Arrows */
		// Force of the wind on the sale in local space
		Vector2 sailForceLoc = invBoatRot * player.sailForceGlob;
		Vector2 fwForceLoc = invBoatRot * player.fwForceGlob;
		
		
		// Wind Force
		windForceLine.points2[0] = centrePos - relWindVelLoc * visScale;
		windForceLine.points2[1] = centrePos;
		windForceLine.color = windDirCol * transparencyColArrows;
		windForceLine.Draw ();
		
		// Sail Force
		sailForceLine.points2[0] = centrePos - sailForceLoc * visScale;
		sailForceLine.points2[1] = centrePos;
		sailForceLine.color = sailDirCol * transparencyColArrows;
		sailForceLine.Draw ();
		
		// Forward Force on boat
		fwForceLine.points2[0] = centrePos - fwForceLoc * visScale;
		fwForceLine.points2[1] = centrePos;
		fwForceLine.color = boatDirCol * transparencyColFwForce;
		fwForceLine.Draw ();
		
		
		/* Projection lines */
		
		// Wind to Sail
		windToSailForceFrom.points2[0] = centrePos - relWindVelLoc * visScale;
		windToSailForceFrom.points2[1] = centrePos - sailForceLoc * visScale;
		windToSailForceFrom.color = windDirCol * transparencyColProjections;
		windToSailForceFrom.Draw();
		
		windToSailForceTo.points2[0] = centrePos - sailForceLoc * visScale;
		windToSailForceTo.points2[1] = centrePos - relWindVelLoc * visScale;
		windToSailForceTo.color = sailDirCol * transparencyColProjections;
		windToSailForceTo.Draw();
		
		sailToFwForceFrom.points2[0] = centrePos - sailForceLoc * visScale;
		sailToFwForceFrom.points2[1] = centrePos - fwForceLoc * visScale;
		sailToFwForceFrom.color = sailDirCol * transparencyColProjections;
		sailToFwForceFrom.Draw();
		
		sailToFwForceTo.points2[0] = centrePos - fwForceLoc * visScale;
		sailToFwForceTo.points2[1] = centrePos - sailForceLoc * visScale;
		sailToFwForceTo.color = boatDirCol * transparencyColProjections;
		sailToFwForceTo.Draw();
		
		/* right angle symbols */
		float rightAngleBoxSize = rightAngleBoxSizeProp * Screen.width;
		
		// Scale size if box needs to be smaller
		float windToSailMaxSize = (sailForceLoc * visScale).magnitude;
		float windToSailBoxSize = Mathf.Min (rightAngleBoxSize, windToSailMaxSize);
		
		Vector2 windToSailGoDir0 = (sailForceLoc - relWindVelLoc).normalized * windToSailBoxSize;
		Vector2 windToSailGoDir1 = (sailForceLoc).normalized * windToSailBoxSize;
		windToSailRA.points2[0] = centrePos - sailForceLoc * visScale + windToSailGoDir0;
		windToSailRA.points2[1] = windToSailRA.points2[0] + windToSailGoDir1;
		windToSailRA.points2[2] = windToSailRA.points2[1] - windToSailGoDir0;
		windToSailRA.color = sailDirCol * transparencyColProjections;
		windToSailRA.Draw ();
		
		// Scale size if box needs to be smaller
		float sailToFwMaxSize = (sailForceLoc * visScale).magnitude;
		float sailToFwBoxSize = Mathf.Min (rightAngleBoxSize, sailToFwMaxSize);
		
		Vector2 sailToFwGoDir0 = (fwForceLoc - sailForceLoc).normalized * sailToFwBoxSize;
		Vector2 sailToFwGoDir1 = (fwForceLoc).normalized * sailToFwBoxSize;
		sailToFwRA.points2[0] = centrePos - fwForceLoc * visScale + sailToFwGoDir0;
		sailToFwRA.points2[1] = sailToFwRA.points2[0] + sailToFwGoDir1;
		sailToFwRA.points2[2] = sailToFwRA.points2[1] - sailToFwGoDir0;
		sailToFwRA.color = boatDirCol * transparencyColProjections;
		sailToFwRA.Draw ();
		
		
		// Cost function graph
//		float maxCost = 20;
//		float multiplier = radius / maxCost;
//		
//		for (int i = 0; i < player.fwPoints.Count (); ++i){
//			float angleRad = Mathf.Deg2Rad * player.fwPoints[i].x;
//			float thisRad = radius + player.fwPoints[i].y * multiplier;
//			
//			costFunctionLine.points2[i] = centrePos + thisRad * new Vector2( Mathf.Sin (angleRad), -Mathf.Cos (angleRad));
//		}
//		costFunctionLine.color = boatDirCol;
//		costFunctionLine.Draw ();
		
		
	}
	
	// Use this for initialization
	void Start () {
		player = playerGO.GetComponent<Player>();
		ConstructLines();
		
	
	}
	
	// Update is called once per frame
	void Update () {
		DrawAll();
	
	}
}
