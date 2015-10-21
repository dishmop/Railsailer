using UnityEngine;
using System.Collections;
using System.Text;

public class TutRudder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tutorial.singleton.player.GetComponent<Player>().lockPosition = true;
		Tutorial.singleton.player.GetComponent<Player>().maxSpeed = -1;
		Tutorial.singleton.player.GetComponent<Player>().disableRudder = false;

		if (PlayerPrefs.GetString("Player1") == "Joystick"){
			StringBuilder builder = new StringBuilder();
			builder.Append("I've enabled your rudder so you can steer.\n");
			builder.Append("\n");
			builder.Append("Use the left analog stick to turn the boat. \n");
			builder.Append("You can turn slowly when stationary, but will\n"); 
			builder.Append("turn much faster when moving forward.\n"); 
			GetComponent<TextMesh>().text =builder.ToString();
			
		}
		else{
			StringBuilder builder = new StringBuilder();
			builder.Append("I've enabled your rudder so you can steer.\n");
			builder.Append("\n");
			builder.Append("Use the [A] and [D] keys to turn the boat. \n");
			builder.Append("You can turn slowly when stationary, but will\n"); 
			builder.Append("turn much faster when moving forward.\n"); 
			GetComponent<TextMesh>().text =builder.ToString();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
