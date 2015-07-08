using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartUpBanner : MonoBehaviour {
	float startTime;
	
	// Use this for initialization
	void Start () {
		startTime = Time.time + 2f;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Time.time > startTime){
			Color thisCol = transform.GetComponent<Image>().color;
			thisCol.a = Mathf.Max (0, thisCol.a - 0.01f);
			transform.GetComponent<Image>().color = thisCol;
		}
		if (Time.time > startTime + 2f){
			gameObject.SetActive(false);
		}
	
	}
}

