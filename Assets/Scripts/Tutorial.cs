using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tutorial : MonoBehaviour {	
	public static Tutorial singleton = null;
	public GameObject player;
	public GameObject overlay;
	public int step = -1;
	public float startDelay = 8;
	float startTime;
	
	List<GameObject> tutGOs = new List<GameObject>();

//	enum State {
//		kOff,
//		kInitialise,
//		kControls,
//	}
//	
//	State state = State.kOff;
	
	public void ShowOverlay(bool show){
		overlay.GetComponent<PlayerOverlay>().hideOverlay = !show;
	}
	
	public void MoveOn(){
		step++;
	}

	// Use this for initialization
	void Start () {
	
		foreach (Transform child in transform){
			tutGOs.Add (child.gameObject);
		}
		
		foreach (GameObject go in tutGOs){	
			go.SetActive(false);
		}
		
		startTime = Time.time;
		
		ShowOverlay(false);
		player.GetComponent<Player>().lockPosition = true;
		player.GetComponent<Player>().disableJib = true;
		player.GetComponent<Player>().disableRudder = true;
	
	}
	
	void FixedUpdate(){
		if (step < 0){
			if (Time.time > startTime + startDelay){
				step = 0;
			}
		}
		
		for (int i = 0; i < tutGOs.Count(); ++i){
			tutGOs[i].SetActive(i == step);
		}
		
//		if (step == 1){
//			ShowOverlay(true);
//			player.GetComponent<Player>().lockPosition = false;
//			player.GetComponent<Player>().disableJib = false;
//		}
		
	}
	
	
	void Awake(){
		if (singleton != null) Debug.LogError ("Error assigning singleton");
		singleton = this;
	}
	
	
	
	void OnDestroy(){
		singleton = null;
	}
	
}
