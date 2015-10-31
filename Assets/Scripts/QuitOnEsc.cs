using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class QuitOnEsc : MonoBehaviour {

	public string OnQuitLevelName;
	public bool showCursorOnStartup = false;
	public bool hideCursorOnStartup = false;
	
	float startTime = -1;
	

	// Use this for initialization
	void Start () {
//		Cursor.visible = false;
		
		//Time.timeScale = 0.2f;
		if (showCursorOnStartup){
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}	
		if (hideCursorOnStartup) Cursor.visible = false;
		
		startTime = Time.fixedTime;
	
	}
	
	// Update is called once per frame
	void Update () {

		
		// Test for exit
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (OnQuitLevelName != null && OnQuitLevelName != ""){
				GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "quitLevel", Application.loadedLevelName, Time.fixedTime - startTime);
				
//				Analytics.CustomEvent("quitLevel", new Dictionary<string, object>
//				                      {
//					{ "duration", Time.fixedTime - startTime },
//				});	
				Application.LoadLevel(OnQuitLevelName);
			}
			else{
				if (GetComponent<FronEndUI>().mode == FronEndUI.Mode.kInstructions){
					GetComponent<FronEndUI>().GoToMainMenu();
				}
				else{
				
					AppHelper.Quit();
				}
			}
		}
		

			
		
	}
}
