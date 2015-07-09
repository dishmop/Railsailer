using UnityEngine;
using System.Collections;

public class FronEndUI : MonoBehaviour {
	public AudioSource clickWhir;
	public AudioSource littleClick;

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
		clickWhir.Play ();
		Application.LoadLevel("Level1");
	}
	
	public void Player1Joystick(){
		clickWhir.Play ();
		PlayerPrefs.SetString("Player1", "Joystick");
	}

	public void Player2Joystick(){
		clickWhir.Play ();
		PlayerPrefs.SetString("Player2", "Joystick");
	}
	
	public void Player1Keyboard(){
		clickWhir.Play ();
		PlayerPrefs.SetString("Player1", "Keyboard");
	}
	
	public void Player2Keyboard(){
		clickWhir.Play ();
		PlayerPrefs.SetString("Player2", "Keyboard");
	}
	
	public void Player1AI(){
		clickWhir.Play ();
		PlayerPrefs.SetString("Player1", "AI");
	}
	
	public void Player2AI(){
		clickWhir.Play ();
		PlayerPrefs.SetString("Player2", "AI");
	}
	
	public void LittleClick(){
		littleClick.Play();
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
