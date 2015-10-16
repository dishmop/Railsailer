using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	public GameObject objToTrack;
	Vector3 initPos;
	public float startSize = 10;
	public float finalSize = 4;
	public bool triggered = false;

	// Use this for initialization
	void Start () {
		initPos = transform.position;
	
	}
	
	public void TriggerZoom(){
		triggered = true;
	}
	// Update is called once per frame
	void Update () {
	
		if (triggered){
			startSize = Mathf.Lerp (startSize, finalSize, Time.deltaTime);

		}
		GetComponent<Camera>().orthographicSize = startSize;
	
		if (GameConfig.singleton.enableTracking){
			Vector3 pos = objToTrack.transform.position;
			
			pos.z = transform.position.z;
			transform.position = pos;
		}
		else{
			transform.position = initPos;
		
		}
		
		if (GameConfig.singleton.enableOrientTracking){
			transform.rotation = objToTrack.transform.rotation;
			transform.Rotate (0, 0, 180);
		}
		else{
			transform.rotation = Quaternion.identity;
		}
	
	}
}
