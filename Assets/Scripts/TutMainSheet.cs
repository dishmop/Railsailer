using UnityEngine;
using System.Collections;
using System.Text;

public class TutMainSheet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StringBuilder builder = new StringBuilder();
		if (PlayerPrefs.GetString("Player1") != "Joystick"){
			builder.Append("This rope is called the \"Mainsheet\".\n");
			builder.Append("It is used to pull the sail in towards the boat.\n");
			builder.Append("\n");
			builder.Append("You can do this by moving the mouse up and down.\n");
			builder.Append("Do ths a few times to move on.\n");
		}
		else{
			builder.Append("This rope is called the \"Mainsheet\".\n");
			builder.Append("It is used to pull the sail in towards the boat.\n");
			builder.Append("\n");
			builder.Append("You can do this with the right trigger.\n");
			builder.Append("Do ths a few times to move on.\n");
		}
		GetComponent<TextMesh>().text = builder.ToString(); 
		Tutorial.singleton.player.GetComponent<Player>().disableJib = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	}
}
