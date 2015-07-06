﻿using UnityEngine;

public class DebugUtils{

	public static float arrowSize = 0.1f;

	public static void Assert(bool condition){
		if (!condition){
			Debug.LogError("Generic Assert");
		}
	}
	
	public static void Assert(bool condition, Object context){
		if (!condition){
			Debug.LogError("Generic Assert", context);
		}
	}

	public static void Assert(bool condition, string msg){
		if (!condition){
//			 Debug.LogError(msg);
			}
	}

	public static void Assert(bool condition, string msg, Object context){
		if (!condition){
			Debug.LogError(msg, context);
		}
	}
	
	public static void DrawArrow(Vector3 startPos, Vector3 endPos, Color col){
		
		// Figure out vector axes so arrow is visible to camera
		Vector3 fwVec = endPos - startPos;
		Vector3 fwDir = fwVec.normalized;
		Vector3 centrePos = 0.5f * (startPos + endPos);
		Vector3 camVec;
		if (Camera.main.orthographic){
			camVec = Camera.main.transform.rotation * new Vector3(0, 0, 1);
		}
		else{
			camVec = Camera.main.transform.position - centrePos;
		}
		Vector3 sideDir = Vector3.Cross(fwVec, camVec).normalized;
		
		// Now construct the arrow
		Vector3 sidePos0 = endPos - (fwDir + sideDir) * arrowSize;
		Vector3 sidePos1 = endPos - (fwDir - sideDir) * arrowSize;
		
		// The shaft
		Debug.DrawLine(startPos, endPos, col);
		
		// The arrow
		Debug.DrawLine (endPos, sidePos0, col);
		Debug.DrawLine (endPos, sidePos1, col);
	}
	
	public static void DrawCircle(Vector3 startPos, float radius, Color col){
		int numPoints = (int)(radius+1) * 10;
		for (int i = 0; i < numPoints; ++i){
			float angleRadFrom = 2 * Mathf.PI * (float)i / (float)numPoints;
			float angleRadTo = 2 * Mathf.PI * (float)(i+1) / (float)numPoints;
			Vector3 unitOffsetFrom = new Vector3(Mathf.Sin(angleRadFrom), Mathf.Cos(angleRadFrom), 0);
			Vector3 unitOffsetTo = new Vector3(Mathf.Sin(angleRadTo), Mathf.Cos(angleRadTo), 0);
			Debug.DrawLine (startPos + radius * unitOffsetFrom, startPos + radius * unitOffsetTo, col);
			
		}
	}
}
