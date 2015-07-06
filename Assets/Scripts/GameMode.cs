using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMode : MonoBehaviour {
	public static GameMode singleton = null;
	
	public enum Mode{
		kInit,
		kSignalOff,
		kSignalOn1,
		kSignalOn2,
		kSignalOn3,
		kRace,
		kRaceComplete
	}
	public Mode mode = Mode.kInit;
	public GameObject gameCompletePanel;
	
	GameObject winningPlayer;
	
	float countStepDuration = 1;
	
	float countInTime = 0;
	
	public void StartCountIn(){
		mode = Mode.kSignalOff;
		countInTime = Time.fixedTime;
	}
	
	public bool IsRacing(){
		return mode == Mode.kRace || mode == Mode.kSignalOn3;
	}
	
	public void TriggerWinner(GameObject winner){
		winningPlayer = winner;
		mode = Mode.kRaceComplete;
	
	}

	// Use this for initialization
	void Start () {
		StartCountIn();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if (mode == Mode.kRaceComplete){
			gameCompletePanel.SetActive(true);
			gameCompletePanel.transform.FindChild("Text").GetComponent<Text>().text = winningPlayer.name + " is the Winner!";
			return;
		}
		gameCompletePanel.SetActive(false);
	
		if (mode == Mode.kInit){
			countInTime = 0;
		}
		
		if (Time.fixedTime < countInTime + countStepDuration){
			mode = Mode.kSignalOff;
		} 
		else if (Time.fixedTime < countInTime + 2 * countStepDuration){
			mode = Mode.kSignalOn1;
		}
		else if (Time.fixedTime < countInTime + 3 * countStepDuration){
			mode = Mode.kSignalOn2;
		}
		else if (Time.fixedTime < countInTime + 4 * countStepDuration){
			mode = Mode.kSignalOn3;
		}
		else{
			mode = Mode.kRace;
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
