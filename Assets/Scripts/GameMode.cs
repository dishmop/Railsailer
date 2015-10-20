using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMode : MonoBehaviour {
	public static GameMode singleton = null;
	public bool hackyTurnOffStart = false;
	public enum Mode{
		kInit,
		kSignalOff,
		kGetJoystick,
		kSignalOn1,
		kSignalOn2,
		kSignalOn3,
		kRace,
		kRaceComplete
	}
	public Mode mode = Mode.kInit;
	public GameObject gameCompletePanel;
	public GameObject[] cameras;
	
	public GameObject winningPlayer;
	public string mainMenuLevelName;
	public AudioSource bell;
	public AudioSource horn;
	public AudioSource clickWhir;
	public bool tutorial = false;
	public GameObject player1;
	public GameObject player2;
	public GameObject player1JoystickMsg;
	public GameObject player2JoystickMsg;
	public GameObject pressEscMsg;
	
	bool joystick1Done;
	bool joystick2Done;
	
	float countStepDuration = 2;
	
	float countInTime = 0;
	
	public void StartCountIn(){
		mode = Mode.kSignalOff;
		countInTime = Time.fixedTime;
	}
	
	public bool IsRacing(){
		return mode == Mode.kRace || mode == Mode.kSignalOn3;
	}
	
	public void TriggerWinner(GameObject winner){
		
		if (winningPlayer == null){
			winningPlayer = winner;
			mode = Mode.kRaceComplete;
			horn.Play ();
		}
	
	}
	
	public void ReturnToMainMenu(){
		Application.LoadLevel (mainMenuLevelName);
	}

	// Use this for initialization
	void Start () {
		if (!hackyTurnOffStart){
			StartCountIn();
		}
		joystick1Done = false;
		joystick2Done = false;
		
	
	}
	
	void Update(){
		Cursor.visible = (mode == Mode.kRaceComplete);
		Cursor.lockState = (mode == Mode.kRaceComplete) ? CursorLockMode.None : CursorLockMode.Confined;
	}
	
	public void TriggerCameras(){
		foreach (GameObject go in cameras){
			go.GetComponent<GameCamera>().TriggerZoom();
		}
	}
	
	public void PlayClickWhir(){
		clickWhir.Play();
	}
	
	void HandleJoysticks(){
		
		string player1Input = PlayerPrefs.GetString("Player1");
		string player2Input = PlayerPrefs.GetString("Player2");
		
		if (!joystick1Done && player1Input == "Joystick"){
			player1JoystickMsg.SetActive(true);
			if (Input.GetKeyDown("joystick 1 button 16") || Input.GetKeyDown("joystick 1 button 0")){
				player1.GetComponent<Player>().joystickId = "-J1";
				joystick1Done = true;
				clickWhir.Play();
			}
			if (Input.GetKeyDown("joystick 2 button 16") || Input.GetKeyDown("joystick 2 button 0")){
				player1.GetComponent<Player>().joystickId = "-J2";
				joystick1Done = true;
			}
		}
		else{
			player1JoystickMsg.SetActive(false);
			joystick1Done = true;
		}
		if (player2 != null){
			if (joystick1Done){
				if (!joystick2Done && player2Input == "Joystick"){
					player2JoystickMsg.SetActive(true);
					if (player1.GetComponent<Player>().joystickId != "-J1"){
						if (Input.GetKeyDown("joystick 1 button 16") || Input.GetKeyDown("joystick 1 button 0")){
							player2.GetComponent<Player>().joystickId = "-J1";
							joystick2Done = true;
							clickWhir.Play();
						}
					}
					if (player1.GetComponent<Player>().joystickId != "-J2"){
						if (Input.GetKeyDown("joystick 2 button 16") || Input.GetKeyDown("joystick 2 button 0")){
							player2.GetComponent<Player>().joystickId = "-J2";
							joystick2Done = true;
						}			
					}
				}
				else{
					player2JoystickMsg.SetActive(false);
					joystick2Done = true;
				}
			}
		}
		else{
			joystick2Done = true;
		}
		pressEscMsg.SetActive(!joystick1Done || !joystick2Done);
		

	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if (tutorial){
			HandleJoysticks();
			
			if (joystick1Done && joystick2Done && mode == Mode.kSignalOff){
				mode = Mode.kRace;
				countInTime = Time.fixedTime;
			}
		
			if (mode == Mode.kRace){				
				if (Time.fixedTime > countInTime + countStepDuration*2){
					TriggerCameras();
				} 		
				
			}

			return;
		}
	
		if (mode == Mode.kRaceComplete){
			gameCompletePanel.SetActive(true);
			gameCompletePanel.transform.FindChild("Text").GetComponent<Text>().text = winningPlayer.GetComponent<Player>().playerName + " is the Winner!";
			AudioListener.volume = AudioListener.volume-0.0025f;
			return;
		}
		gameCompletePanel.SetActive(false);
	
		if (mode == Mode.kInit){
			countInTime = Time.fixedTime;
		}
		
		Mode lastMode = mode;
		
		if (Time.fixedTime < countInTime + countStepDuration*2){
			HandleJoysticks();
			if (!joystick1Done || !joystick2Done){
				countInTime = Time.fixedTime;
				
				mode = Mode.kGetJoystick;
			}
			else{
				mode = Mode.kSignalOff;
			}
			
		} 
		else if (Time.fixedTime < countInTime + countStepDuration*3){
			TriggerCameras();
			
		} 
		else if (Time.fixedTime < countInTime + 4 * countStepDuration){
			mode = Mode.kSignalOn1;
		}
		else if (Time.fixedTime < countInTime + 5 * countStepDuration){
			mode = Mode.kSignalOn2;
		}
		else if (Time.fixedTime < countInTime + 6 * countStepDuration){
			mode = Mode.kSignalOn3;
		}
		else{
			mode = Mode.kRace;
		}
		
		if (lastMode != mode && mode == Mode.kSignalOn1){
			bell.Play();
		}
		if (lastMode != mode && mode == Mode.kSignalOn2){
			bell.Play();
		}
		if (lastMode != mode && mode == Mode.kSignalOn3){
			horn.Play();
		}
		
	}
	

	
	//----------------------------------------------
	void Awake(){
		if (singleton != null) Debug.LogError ("Error assigning singleton");
		singleton = this;
	}
	
	
	
	void OnDestroy(){
		singleton = null;
	}
}
