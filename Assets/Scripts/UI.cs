using UnityEngine;
using System.Collections;
using Vectrosity;

public class UI : MonoBehaviour {
	public static UI singleton = null;
	public bool mouseIsInUI;
	public Material mainCircleMaterial;
	public Material smallDottedLineMaterial;
	public Material gradiatedLineMaterial;
	public Material middleGradiatedMaterial;
	public Material arrowMaterial;
	public Texture2D arrowFrontTex;
	public Texture2D arrowBackTex;
	public Texture2D gradiatedEndTex;
	
	
	public Material vectrosityMaterialPrefab;
	
	
	
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
		
		// Make a prefab from the material the SetLine uses so we can clone it and use it again,. 
		VectorLine tempLine = VectorLine.SetLine(Color.green, Vector2.zero,Vector2.zero);
		vectrosityMaterialPrefab = tempLine.material;
		VectorLine.Destroy(ref tempLine);
		
		// Setup arrow ends
		VectorLine.SetEndCap ("rounded_arrow", EndCap.Both, arrowMaterial, arrowFrontTex, arrowBackTex);
		VectorLine.SetEndCap ("gradiated_end", EndCap.Front, gradiatedLineMaterial, gradiatedEndTex);
		
	}
	
	
	
	void OnDestroy(){
		singleton = null;
	}
}
