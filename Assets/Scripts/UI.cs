using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {
	public static UI singleton = null;
	public bool mouseIsInUI;
	
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void MouseUIEnter(){
		mouseIsInUI = true;
	}
	
	public void MouseUIExit(){
		mouseIsInUI = false;
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
