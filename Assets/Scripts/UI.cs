using UnityEngine;
using System.Collections;
using Vectrosity;

public class UI : MonoBehaviour {
	public static UI singleton = null;
	public bool mouseIsInUI;
	
	
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
	}
	
	
	
	void OnDestroy(){
		singleton = null;
	}
}
