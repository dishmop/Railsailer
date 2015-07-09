using UnityEngine;
using System.Collections;

public class FronEndUI : MonoBehaviour {

	public enum Mode{
		kMainMenu,
		kInstructions
	}
	public Mode mode = Mode.kMainMenu;
	
	public void GoToMainMenu(){
		mode = Mode.kMainMenu;
	}
	
	public void GoToInstructions(){
		mode = Mode.kInstructions;
	}
	
	public void StartGame(){
		Application.LoadLevel("Level1");
	}
	
	public void Player1Joystick(){
		PlayerPrefs.SetString("Player1", "Joystick");
	}

	public void Player2Joystick(){
		PlayerPrefs.SetString("Player2", "Joystick");
	}
	
	public void Player1Keyboard(){
		PlayerPrefs.SetString("Player1", "Keyboard");
	}
	
	public void Player2Keyboard(){
		PlayerPrefs.SetString("Player2", "Keyboard");
	}
	
	public void Player1AI(){
		PlayerPrefs.SetString("Player1", "AI");
	}
	
	public void Player2AI(){
		PlayerPrefs.SetString("Player2", "AI");
	}
	
		// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.FindChild("MainMenu").gameObject.SetActive(mode == Mode.kMainMenu);
		transform.FindChild("Instructions").gameObject.SetActive(mode == Mode.kInstructions);
		
	}
}
