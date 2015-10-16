using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class FronEndUI : MonoBehaviour {
	public AudioSource clickWhir;
	public AudioSource littleClick;
	public bool hasXBoxController;
	public bool clearPlayerPrefs = false;

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
	
	public void StartRace(){
		clickWhir.Play ();
		Application.LoadLevel("TheRace");
	}
	
	public void StartPond(){
		clickWhir.Play ();
		Application.LoadLevel("ThePond");
	}	
	
	public void StartTutorial(){
		clickWhir.Play ();
		Application.LoadLevel("Tutorial");
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
		if (clearPlayerPrefs){
			PlayerPrefs.DeleteAll();
		}
		hasXBoxController = false;
	}
	
	// Update is called once per frame
	void Update () {
		transform.FindChild("MainMenu").gameObject.SetActive(mode == Mode.kMainMenu);
		transform.FindChild("Instructions").gameObject.SetActive(mode == Mode.kInstructions);
		

		
		if (!PlayerPrefs.HasKey("Player1") && !clearPlayerPrefs){
			PlayerPrefs.SetString("Player1", "Keyboard");
			PlayerPrefs.SetString("Player2", "AI");
		}
		string[] names = Input.GetJoystickNames();
//		Debug.Log("Joystick names:");
		for (int i = 0; i < names.Count(); ++i){
//			Debug.Log(names[i]);
			if (names[i] == "©Microsoft Corporation Xbox 360 Wired Controller"){
				hasXBoxController = true;
			}
		}	
		if (!hasXBoxController){
			if (PlayerPrefs.GetString("Player1") == "Joystick"){
				PlayerPrefs.SetString("Player1", "Keyboard");
			}			
			if (PlayerPrefs.GetString("Player2") == "Joystick"){
				PlayerPrefs.SetString("Player2", "Keyboard");
			}
		}
		transform.FindChild("MainMenu").FindChild("Player1 panel").FindChild("Joystick").GetComponent<Button>().interactable = hasXBoxController;
		transform.FindChild("MainMenu").FindChild("Player2 panel").FindChild("Joystick").GetComponent<Button>().interactable = hasXBoxController;
		
		
	}
}
