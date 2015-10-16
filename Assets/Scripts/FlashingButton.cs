using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class FlashingButton : MonoBehaviour {

	public bool isPointerInside;
		
	public void OnPointeEnter(){
		isPointerInside = true;
	}
	
	public void OnPointerExit(){
		isPointerInside = false;
	}
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (GetComponent<TextMesh>() != null){
			Color col = GetComponent<TextMesh>().color;
			if (isPointerInside)
				col.a = 1f;
			else
				col.a = 0.75f + 0.25f * Mathf.Sin (10 * Time.realtimeSinceStartup);
			
			GetComponent<TextMesh>().color = col;
		}
		if (GetComponent<Text>() != null){
			Color col = GetComponent<Text>().color;
			if (isPointerInside)
				col.a = 1f;
			else
				col.a = 0.75f + 0.25f * Mathf.Sin (10 * Time.realtimeSinceStartup);
			
			GetComponent<Text>().color = col;
		}
		
	
	}
	
	void OnEnable(){
		isPointerInside = false;
	
	}
	
}
