using UnityEngine;
using System.Collections;

public class QuitOnEsc : MonoBehaviour {

	public string OnQuitLevelName;
	public bool showCursorOnStartup = false;
	public bool hideCursorOnStartup = false;
	

	// Use this for initialization
	void Start () {
//		Cursor.visible = false;
		
		//Time.timeScale = 0.2f;
		if (showCursorOnStartup) Cursor.visible = true;
		if (hideCursorOnStartup) Cursor.visible = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		
		// Test for exit
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (OnQuitLevelName != null && OnQuitLevelName != ""){
				Application.LoadLevel(OnQuitLevelName);
			}
			else{
				AppHelper.Quit();
			}
		}
		

			
		
	}
}
