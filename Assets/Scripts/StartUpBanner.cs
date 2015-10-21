using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartUpBanner : MonoBehaviour {
	float startTime;
	
	
	// Use this for initialization
	void Start () {
		startTime = Time.time + 2f;
		

	}
	
	// Update is called once per frame
	void Update () {
		if (GameMode.singleton.mode == GameMode.Mode.kGetJoystick){
			Start ();
		}
		if (GameMode.singleton.mode != GameMode.Mode.kRaceComplete){
			AudioListener.volume = 1-transform.GetComponent<Image>().color.a;
		}
		
		if (Time.time > startTime){
			Color thisCol = transform.GetComponent<Image>().color;
			thisCol.a = Mathf.Max (0, thisCol.a - 0.01f);
			transform.GetComponent<Image>().color = thisCol;
		}
		if (Time.time > startTime + 2f){
			gameObject.SetActive(false);
		}
		
		if (transform.FindChild("P1 Control Text") != null){
			if (PlayerPrefs.GetString("Player1") == "Joystick"){
				transform.FindChild("P1 Joystick Viz").gameObject.SetActive(true);
				transform.FindChild("P1 Keyboard Viz").gameObject.SetActive(false);
				transform.FindChild("P1 Control Text").gameObject.SetActive(true);
			}
			else if (PlayerPrefs.GetString("Player1") == "Keyboard"){
				transform.FindChild("P1 Joystick Viz").gameObject.SetActive(false);
				transform.FindChild("P1 Keyboard Viz").gameObject.SetActive(true);
				transform.FindChild("P1 Control Text").gameObject.SetActive(true);
			}
		}
		
		if (transform.FindChild("P2 Control Text") != null){
			if (PlayerPrefs.GetString("Player2") == "Joystick"){
				transform.FindChild("P2 Joystick Viz").gameObject.SetActive(true);
				transform.FindChild("P2 Keyboard Viz").gameObject.SetActive(false);
				transform.FindChild("P2 Control Text").gameObject.SetActive(true);
			}
			else if (PlayerPrefs.GetString("Player2") == "Keyboard"){
				transform.FindChild("P2 Joystick Viz").gameObject.SetActive(false);
				transform.FindChild("P2 Keyboard Viz").gameObject.SetActive(true);
				transform.FindChild("P2 Control Text").gameObject.SetActive(true);
			}	
			else{
				transform.FindChild("P2 Joystick Viz").gameObject.SetActive(false);
				transform.FindChild("P2 Keyboard Viz").gameObject.SetActive(false);
				transform.FindChild("P2 Control Text").gameObject.SetActive(true);
				transform.FindChild("P2 Control Text").GetComponent<Text>().text = "P2 Controls: AI";
			}	
		}
		
			
	}
}

