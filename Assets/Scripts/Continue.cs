using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class Continue : MonoBehaviour {
	public UnityEvent trigger;
	public string keyboardText = "Press SPACE to continue";
	public string joystickText = "Press A to continue";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (PlayerPrefs.GetString("Player1") == "Joystick"){
			GetComponent<TextMesh>().text = joystickText;
		}
		else{
			GetComponent<TextMesh>().text = keyboardText;
		}
		if (Input.GetKeyDown(KeyCode.Space)){
 			trigger.Invoke();
 			GameMode.singleton.PlayClickWhir();
		}
		if (Input.GetKeyDown("joystick 1 button 16") || Input.GetKeyDown("joystick 1 button 0") || Input.GetKeyDown("joystick 2 button 16") || Input.GetKeyDown("joystick 2 button 0") || Input.GetKeyDown(KeyCode.A)){
			trigger.Invoke();
			GameMode.singleton.PlayClickWhir();
		}
		
	}
}
