using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Continue : MonoBehaviour {
	public UnityEvent trigger;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerPrefs.GetString("Player1") == "Joystick"){
			GetComponent<TextMesh>().text = "Press A to continue";
		}
		else{
			GetComponent<TextMesh>().text = "Press SPACE to continue";
		}
		if (Input.GetKeyDown(KeyCode.Space)){
 			trigger.Invoke();
		}
		if (Input.GetKeyDown("joystick 1 button 16") || Input.GetKeyDown("joystick 1 button 0")){
			trigger.Invoke();
		}
		
	}
}
