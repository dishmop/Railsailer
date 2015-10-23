using UnityEngine;
//using UnityEngine.Serialization;
//using UnityEditor;
using System.Collections;

public class Snow : MonoBehaviour {
	public GameObject cameraGO;
	Vector3 lastPos;
	float baseEmissionRate;
	
	Quaternion startRot;
	float maxAlpha;


	
	void LateUpdate(){
		transform.rotation = startRot;
	}
	
	// Use this for initialization
	void Start () {
		startRot = transform.rotation;
		lastPos = transform.position;
		
		// Get the velocity of the camera
		
		baseEmissionRate =  transform.FindChild("Particle System").gameObject.GetComponent<ParticleSystem>().emissionRate;
		Color col = transform.FindChild("Particle System").GetComponent<Renderer>().material.color;
		maxAlpha = col.a;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 camVel = (transform.position - lastPos) * (1f/Time.fixedDeltaTime);
		Vector3 emmiterVel = Environment.singleton.windVel - camVel;
		
		float baseParticlesPerMeter = baseEmissionRate / Environment.singleton.windVel.magnitude;
		float newEmissionRate = emmiterVel.magnitude * baseParticlesPerMeter / Mathf.Pow(2f, SetupResolution.numReductions);
		
		transform.FindChild("Particle System").gameObject.GetComponent<ParticleSystem>().emissionRate = newEmissionRate;
		float maxCamSize = 15f;
		float minCamSize = 8.5f;
		float camRange = maxCamSize - minCamSize;
		float alphaVal = maxAlpha * Mathf.Clamp01(1f -(Camera.main.orthographicSize - minCamSize) / camRange);
		Color col = transform.FindChild("Particle System").GetComponent<Renderer>().material.color;
		col.a = alphaVal;
		transform.FindChild("Particle System").GetComponent<Renderer>().material.color = col;
		lastPos = transform.position;
	
	}
}
