using UnityEngine;
using System.Collections;
using System.Text;

public class Tut03MainSheet : MonoBehaviour {

	public float cumulativeDeg ;
	float lastSailAngle = 1000;

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
		transform.FindChild("ContinueButton").gameObject.SetActive(false);
		cumulativeDeg = 0;
	}
	
	// Update is called once per frame
	void Update () {
		float sailAngle = Tutorial.singleton.player.GetComponent<Player>().recordSailAngle;
		if (lastSailAngle < 999){
			cumulativeDeg += Mathf.Abs(lastSailAngle - sailAngle);
		}
		lastSailAngle = sailAngle;
		if (cumulativeDeg > 200){
			transform.FindChild("ContinueButton").gameObject.SetActive(true);
		}
	
	}
}
