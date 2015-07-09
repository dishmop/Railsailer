using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuTick : MonoBehaviour {
	public string key;
	public string value; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.GetComponent<Image>().enabled = (PlayerPrefs.GetString(key) == value);
	}
}
