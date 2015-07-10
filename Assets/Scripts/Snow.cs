using UnityEngine;
//using UnityEngine.Serialization;
//using UnityEditor;
using System.Collections;

public class Snow : MonoBehaviour {
	public GameObject cameraGO;
	Vector3 lastPos;
	float baseEmissionRate;
	
	Quaternion startRot;


	
	void LateUpdate(){
		transform.rotation = startRot;
	}
	
	// Use this for initialization
	void Start () {
		startRot = transform.rotation;
		lastPos = transform.position;
		
		// Get the velocity of the camera
		
		baseEmissionRate =  transform.FindChild("Particle System").gameObject.GetComponent<ParticleSystem>().emissionRate;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 camVel = (transform.position - lastPos) * (1f/Time.fixedDeltaTime);
		Vector3 emmiterVel = Environment.singleton.windVel - camVel;
		
		float baseParticlesPerMeter = baseEmissionRate / Environment.singleton.windVel.magnitude;
		float newEmissionRate = emmiterVel.magnitude * baseParticlesPerMeter;
		
		transform.FindChild("Particle System").gameObject.GetComponent<ParticleSystem>().emissionRate = newEmissionRate;
		
		lastPos = transform.position;
	
	}
}
