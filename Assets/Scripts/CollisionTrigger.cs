using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour {

	public UnityEvent trigger;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter2D(Collider2D collider){
		trigger.Invoke();
	}
}
