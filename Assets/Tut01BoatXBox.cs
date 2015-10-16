using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class Tut01BoatXBox : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		if (PlayerPrefs.GetString("Player1") == "Joystick"){
			StringBuilder builder = new StringBuilder();
			builder.Append("This is your boat.\n");
			builder.Append("\n");
			builder.Append("The best way to control it is with \n");
			builder.Append("an XBox360 controller - which is\n"); 
			builder.Append("what you have chosen to do.\n"); 
			GetComponent<TextMesh>().text =builder.ToString();

		}
		else{
			StringBuilder builder = new StringBuilder();
			builder.Append("This is your boat.\n");
			builder.Append("\n");
			builder.Append("The best way to control it is with \n");
			builder.Append("an XBox360 controller. If you\n"); 
			builder.Append("have one I recommend you press\n"); 
			builder.Append("ESC to go back to the main menu\n"); 
			builder.Append("and select it as the input device\n"); 
			builder.Append("for Player 1, otherwise...\n");
			GetComponent<TextMesh>().text =builder.ToString();
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
