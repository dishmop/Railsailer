using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class TutBoatXBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetString("Player1") == "Joystick"){
			StringBuilder builder = new StringBuilder();
			builder.Append("This is your boat.\n");
			builder.Append("It is anchored right now.\n");
			builder.Append("\n");
			builder.Append("The best way to control it is with \n");
			builder.Append("an XBox360 controller - which is\n"); 
			builder.Append("what you have chosen to do.\n"); 
			GetComponent<TextMesh>().text =builder.ToString();

		}
		else{
			StringBuilder builder = new StringBuilder();
			builder.Append("This is your boat.\n");
			builder.Append("It is anchored right now.\n");
			builder.Append("\n");
			builder.Append("The best way to control it is with \n");
			builder.Append("an XBox360 controller. If you\n"); 
			builder.Append("have one I recommend you press\n"); 
			builder.Append("ESC to go back to the main menu\n"); 
			builder.Append("and select it as the input device\n"); 
			builder.Append("for Player 1, otherwise...\n");
			GetComponent<TextMesh>().text =builder.ToString();
		}
		
		Tutorial.singleton.player.GetComponent<Player>().disableJib = true;
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = true;		
		
		GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "tutorial0Start", "", Tutorial.singleton.GetTutorialTime());
		GoogleAnalytics.Client.SendScreenHit("tutorial0Start");

//		Analytics.CustomEvent("tutorial0Start", new Dictionary<string, object>
//		{
//			{ "tutTime", Tutorial.singleton.GetTutorialTime()},
//		});			
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
