using UnityEngine;
using System.Collections;

public class Signals : MonoBehaviour {
	public GameObject signalOffGO;
	public GameObject signalOn1;
	public GameObject signalOn2;
	public GameObject signalOn3;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		signalOffGO.SetActive(GameMode.singleton.mode == GameMode.Mode.kSignalOff);
		signalOn1.SetActive(GameMode.singleton.mode == GameMode.Mode.kSignalOn1);
		signalOn2.SetActive(GameMode.singleton.mode == GameMode.Mode.kSignalOn2);
		signalOn3.SetActive(GameMode.singleton.mode == GameMode.Mode.kSignalOn3);
		
	
	}
}
