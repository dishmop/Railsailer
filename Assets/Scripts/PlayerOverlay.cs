using UnityEngine;
using System.Collections;
using System.Linq;
using Vectrosity;

public class PlayerOverlay : MonoBehaviour {

	public GameObject playerGO;
	public GameObject cameraGO;
	public Color mainCircleCol;
	public float scaleAsPropOfScreenWidth= 0.1f;
	public Material mainCircleMaterial;
	
	
	Player player;
	
	// Main circle
	VectorLine mainCircleLine;
	int numPointsInCircle = 100;
	Vector2[] mainCirclePoints;
	float mainCircleThickness = 20;
	
	
	// Wind vel
	VectorLine windDirLine;
	
	


	void ConstructLines(){
	
		// Main circle
		mainCirclePoints = new Vector2[numPointsInCircle];
		for (int i = 0; i < numPointsInCircle; ++i){
			float angleRad = 2 * Mathf.PI * (float)i / (float)(numPointsInCircle-1);
			mainCirclePoints[i] = new Vector2(Mathf.Sin (angleRad), Mathf.Cos(angleRad));
		}
		mainCircleLine = new VectorLine("Main Circle", mainCirclePoints, mainCircleMaterial, mainCircleThickness, LineType.Continuous, Joins.Weld);
		mainCircleLine.color = Color.white;
		
		
	
	}
	
	
	
	void DrawMainCircle(){
		Camera thisCam = cameraGO.GetComponent<Camera>();
		Vector2 centrePos = thisCam.WorldToScreenPoint(player.pivotPos);
		float radius = scaleAsPropOfScreenWidth * Screen.width;
		
		for (int i = 0; i < numPointsInCircle; ++i){
			mainCircleLine.points2[i] = centrePos + radius * mainCirclePoints[i];
		}
		mainCircleLine.Draw ();
	}
	
	// Use this for initialization
	void Start () {
		player = playerGO.GetComponent<Player>();
		ConstructLines();
		
	
	}
	
	// Update is called once per frame
	void Update () {
		DrawMainCircle();
	
	}
}
