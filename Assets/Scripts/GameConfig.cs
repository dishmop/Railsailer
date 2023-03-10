using UnityEngine;
using System.Collections;

public class GameConfig : MonoBehaviour {
	public static GameConfig singleton = null;

	public bool enableTracking = true;
	public bool enableOrientTracking = false;
//	public bool enableRail = true;
//	public bool showForceVisulisation = true;
//	public bool enableAbsoluteSailTurn = true;
//	public bool enableAbsoluteBoatTurn = false;
//	public bool enable2DSailOrient = true;
//	public bool enable2DBoatOrient = true;
//	public bool enableJibSailControl = true;
	public bool enableAutoSail = true;
	public int totalNumLaps = 3;
	public bool enableEdit = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
