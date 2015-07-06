using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIGraph : MonoBehaviour {

	public GameObject canvasGO;
	public Color dataPositiveCol;
	public Color dataNegativeCol;
	public Color xAxisCol;
	public Color yAxisCol;
	public Color vCursorCol;
	
	float xAxisMin = -50;
	float xAxisMax = 50;
	float yAxisMin = -2;
	float yAxisMax = 2;
	
	//float vCursorSizeProp = 0.2f;
	
	Vector2[] data;
	
	float borderProp = 0.1f;
	Vector3[] borderCorners = new Vector3[4];
	
	Vector3 worldBottomLeft;
	Vector3 worldTopRight;
	
	List<Vector2>	vCursors = new List<Vector2>();
	
	
	public void UploadData(Vector2[] newData){
		data = newData.ToArray();
	}
	
	public void SetAxesRanges(float xMin, float xMax, float yMin, float yMax){
		xAxisMin = xMin;
		xAxisMax = xMax;
		yAxisMin = yMin;
		yAxisMax = yMax;
	}

	
	
	public int AddVCursor(){
		vCursors.Add (Vector2.zero);
		return vCursors.Count()-1;
	}
	
	public void SetVCursor(int id, Vector2 pos){
		vCursors[id] = pos;
	}
	
	
	// Use this for initialization
	void Start () {
		
		Vector2[] data = new Vector2[100];
		for (int i = 0; i < 100; ++i){
			data[i] = new Vector2(i-50, Mathf.Sin((float)i / 2));
		}
		
		UploadData(data);
	}
	
	

	
	// Update is called once per frame
	void LateUpdate () {
	
		Rect rect = GetComponent<RectTransform>().rect;
		Vector3 screenRectMin = GetComponent<RectTransform>().TransformPoint(new Vector3(rect.min.x, rect.min.y, 0));
		Vector3 screenRectMax = GetComponent<RectTransform>().TransformPoint(new Vector3(rect.max.x, rect.max.y, 0));
		
		worldBottomLeft = Camera.main.ScreenToWorldPoint(screenRectMin);
		worldTopRight = Camera.main.ScreenToWorldPoint(screenRectMax);
		
		worldBottomLeft.z = 0;
		worldTopRight.z = 0;
		
		GenerateBorder();
		DrawGraphBorder ();
		DrawAxes();
		DrawData();
		DrawVCursors();
	}
	
	void GenerateBorder(){
		Vector3 centrePos = 0.5f * (worldBottomLeft + worldTopRight);
		
		borderCorners[0] = worldBottomLeft;
		borderCorners[1] = new Vector3(worldBottomLeft.x, worldTopRight.y, 0);
		borderCorners[2] = worldTopRight;
		borderCorners[3] = new Vector3(worldTopRight.x, worldBottomLeft.y, 0);
		
		for (int i = 0; i < 4; ++i){
			borderCorners[i] -= centrePos;
		}
		for (int i = 0; i < 4; ++i){
			borderCorners[i] *= 1-borderProp;
		}
		
		
		for (int i = 0; i < 4; ++i){
			borderCorners[i] += centrePos;
		}
	}
	
	void DrawGraphBorder(){
		for (int i = 0; i < 4; ++i){
			int j = (i + 1) % 4;
			Debug.DrawLine (borderCorners[i], borderCorners[j], Color.cyan);
		}
	}
	
	void DrawAxes(){
		// X-Axis
		Vector3 xAxisLeftPos = TransformGraphToWorld(new Vector2(xAxisMin, 0));
		Vector3 xAxisRightPos = TransformGraphToWorld(new Vector2(xAxisMax, 0));
		Debug.DrawLine(xAxisLeftPos, xAxisRightPos, xAxisCol);
		
		// Y-Axis
		Vector3 yAxisBottomPos = TransformGraphToWorld(new Vector2(0, yAxisMin));
		Vector3 yAxisTopPos = TransformGraphToWorld(new Vector2(0, yAxisMax));
		Debug.DrawLine(yAxisBottomPos, yAxisTopPos, yAxisCol);
	}
	
	void DrawData(){
		if (data != null){
			for (int i = 0; i < data.Count ()-1; ++i){
				if (data[i].y * data[i+1].y > 0){
					Vector3 thisPos = TransformGraphToWorld(data[i]);
					Vector3 nextPos = TransformGraphToWorld(data[i+1]);
					Debug.DrawLine(thisPos, nextPos, (data[i].y > 0) ? dataPositiveCol : dataNegativeCol);
				}
				else{
					// find crossing point
					Vector2 relVec = data[i+1] - data[i];
					Vector2 midData = data[i] - relVec * (data[i].y / relVec.y);
					Vector3 thisPos = TransformGraphToWorld(data[i]);
					Vector3 nextPos = TransformGraphToWorld(data[i+1]);
					Vector3 midPos = TransformGraphToWorld(midData);
					
					if (data[i].y > 0){
						Debug.DrawLine(thisPos, midPos, dataPositiveCol);
						Debug.DrawLine(midPos, nextPos, dataNegativeCol);
					}
					else{
						Debug.DrawLine(thisPos, midPos, dataNegativeCol);
						Debug.DrawLine(midPos, nextPos, dataPositiveCol);
					}
				}
			}
		}
	}
	
	void DrawVCursors(){
		foreach (Vector2 vCursorPos in vCursors){
			Vector3 axisPos = TransformGraphToWorld(new Vector2(vCursorPos.x, 0));
			Vector3 cursorPos = TransformGraphToWorld(vCursorPos);
			Debug.DrawLine(axisPos, cursorPos, vCursorCol);
		}
	}
	
	Vector3 TransformGraphToWorld(Vector2 dataIn){
		float xAxisRange = xAxisMax - xAxisMin;
		float yAxisRange = yAxisMax - yAxisMin;
		Vector3 unitScalePos =  new Vector2((dataIn.x - xAxisMin) / xAxisRange, (dataIn.y - yAxisMin) / yAxisRange);
		
		float worldXRange = borderCorners[2].x -  borderCorners[0].x;
		float worldYRange = borderCorners[2].y -  borderCorners[0].y;
		
		return new Vector3(borderCorners[0].x + unitScalePos.x * worldXRange, borderCorners[0].y + unitScalePos.y * worldYRange, 0);
		
	}
}
