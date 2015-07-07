using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vectrosity;

public class UIGraph : MonoBehaviour {

	public GameObject canvasGO;
	public Color dataCol;
	public Color axesCol;
	public Color borderCol;
	public Color cursorCol;
	
	int numCirclePoints = 20;
	float circleSizeProp = 0.02f;
	
	float xAxisMin = -50;
	float xAxisMax = 50;
	float yAxisMin = -2;
	float yAxisMax = 2;
	
	//float vCursorSizeProp = 0.2f;
	
	Vector2[] data;
	
	float borderProp = 0.1f;
	Vector2[] borderCorners = new Vector2[4];
	
	Vector3 worldBottomLeft;
	Vector3 worldTopRight;
	
	List<Vector2>	vCursors = new List<Vector2>();
	
	List<VectorLine> lines = new List<VectorLine>();
	
	Vector2 screenRectMin;
	Vector2 screenRectMax;
	float screenRectWidth;
	float screenRectHeight;
	
	
	Vector2 borderRectMin;
	float borderRectWidth;
	float borderRectHeight;
	
	VectorLine axesLine;
	VectorLine borderLine;
	VectorLine dataLine;
	
	Material axesMaterial;
	Material borderMaterial;
	Material dataMaterial;
	Material cursorMaterial;
	List<VectorLine>	cursorLines = new List<VectorLine>();
	

	
	
	
	
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
		cursorLines.Add (null);
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
		
		

		axesMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		axesMaterial.color = axesCol;
		
		dataMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		dataMaterial.color = dataCol;
		
		
		borderMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		borderMaterial.color = borderCol;
		
		cursorMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		cursorMaterial.color = cursorCol;

		UploadData(data);

	}
	
	

	
	// Update is called once per frame
	void LateUpdate () {
	
	
		Rect rect = GetComponent<RectTransform>().rect;
		Vector3 screenRectMin3D = GetComponent<RectTransform>().TransformPoint(new Vector3(rect.min.x, rect.min.y, 0));
		Vector3 screenRectMax3D = GetComponent<RectTransform>().TransformPoint(new Vector3(rect.max.x, rect.max.y, 0));
		
		worldBottomLeft = Camera.main.ScreenToWorldPoint(screenRectMin3D);
		worldTopRight = Camera.main.ScreenToWorldPoint(screenRectMax3D);
		
		screenRectMin = new Vector2 (screenRectMin3D.x, screenRectMin3D.y);
		screenRectMax = new Vector2 (screenRectMax3D.x, screenRectMax3D.y);
		screenRectWidth = screenRectMax.x - screenRectMin.x;
		screenRectHeight = screenRectMax.y - screenRectMin.y;
		
		Vector2 screenCentre = 0.5f * (screenRectMin + screenRectMax);
		borderRectWidth = screenRectWidth * (1-borderProp);
		borderRectHeight =  screenRectHeight * (1-borderProp);
		borderRectMin = screenCentre - new Vector2(0.5f * borderRectWidth, 0.5f * borderRectHeight);
		
		
		worldBottomLeft.z = 0;
		worldTopRight.z = 0;
		
		GenerateBorder();
		
		// Remove all the lines in our list
		foreach (VectorLine line in lines){
			VectorLine thisLine = line;
			VectorLine.Destroy( ref thisLine);
		}
		lines.Clear();
		
		
		
		DrawGraphBorder ();
		DrawAxes();
		DrawData();
		DrawVCursors();
		
		
		
//		foreach (VectorLine line in lines){
//			line.Draw();
//		}
	}
	
	void GenerateBorder(){
		Vector2 centrePos = 0.5f * (screenRectMin + screenRectMax);
		
		borderCorners[0] = screenRectMin;
		borderCorners[1] = new Vector3(screenRectMin.x, screenRectMax.y, 0);
		borderCorners[2] = screenRectMax;
		borderCorners[3] = new Vector3(screenRectMax.x, screenRectMin.y, 0);
		
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
	
	void DrawLine(Vector2 fromPoint, Vector2 toPoint, Color col){
		VectorLine newLine = VectorLine.SetLine(col, fromPoint, toPoint);
		lines.Add (newLine);
	}
	
	
	void DrawGraphBorder(){
	
		if (borderLine == null){
			Vector2[] points = new Vector2[5];
			for (int i = 0; i < 5; ++i){
				points[i] = borderCorners[i % 4];
			}
			
			borderLine = new VectorLine("Border", points, borderMaterial, 2.0f, LineType.Continuous);
		}
		else{
			for (int i = 0; i < 5; ++i){
				borderLine.points2[i] = borderCorners[i % 4];
			}
			
		}
		borderMaterial.color = borderCol;
		borderLine.Draw();
		

	}
	
	void DrawAxes(){
	
		Vector2[] points = new Vector2[4];

		// X-Axis
		points[0] = TransformGraphToGraphArea(new Vector2(xAxisMin, 0));
		points[1] = TransformGraphToGraphArea(new Vector2(xAxisMax, 0));
			
		// Y-Axis
		points[2] = TransformGraphToGraphArea(new Vector2(0, yAxisMin));
		points[3] = TransformGraphToGraphArea(new Vector2(0, yAxisMax));
		
		if (axesLine == null){
			axesLine = new VectorLine("Axes", points, axesMaterial, 2.0f, LineType.Discrete);
		}
		else{
			for (int i = 0; i < 4; ++i){
				axesLine.points2[i] = points[i];
			}
	
		}
		axesMaterial.color = axesCol;
		axesLine.Draw();
	}
	
	void DrawData(){
	


		if (data != null){
			Vector2[] points  = new Vector2[data.Count ()];
			for (int i = 0; i < points.Count (); ++i){
				points[i] = TransformGraphToGraphArea(data[i]);
			}
			
			if (dataLine != null && dataLine.points2.Count() != points.Count ()){
				VectorLine.Destroy(ref dataLine);
				dataLine = null;
			}
			
			if (dataLine == null){
				dataLine = new VectorLine("Data", points, dataMaterial, 2.0f, LineType.Continuous);
			}
			else{
				for (int i = 0; i < data.Count (); ++i){
					dataLine.points2[i] = points[i];
				}
			}
			dataMaterial.color = dataCol;
			
			dataLine.Draw ();
			

		}
	}
	
	void DrawVCursors(){
		for (int i = 0; i < vCursors.Count(); ++i){
			Vector2 vCursorPos = vCursors[i];
			Vector2 axisPos = TransformGraphToGraphArea(new Vector2(vCursorPos.x, 0));
			Vector2 cursorPos = TransformGraphToGraphArea(vCursorPos);

			Vector2[] points = new Vector2[2 + 2 * numCirclePoints];
			points[0] = axisPos;
			points[1] = cursorPos;
			
			for (int j = 0; j < numCirclePoints; ++j){
				float angleRadFrom = 2 * Mathf.PI * j / (numCirclePoints -1);
				float angleRadTo = 2 * Mathf.PI * (j+1) / (numCirclePoints -1);
				float radius = circleSizeProp * borderRectHeight;
				points[2+ 2*j] = cursorPos + radius * (new Vector2(Mathf.Sin(angleRadFrom), Mathf.Cos(angleRadFrom)));
				points[2+ 2*j+1] = cursorPos + radius * (new Vector2(Mathf.Sin(angleRadTo), Mathf.Cos(angleRadTo)));
				
			}

			
			if (cursorLines[i] == null){
				cursorLines[i] = new VectorLine("Cursor", points, cursorMaterial, 2.0f, LineType.Discrete);
			}
			else{
				for (int j = 0; j < points.Count (); ++j){
					cursorLines[i].points2[j]= points[j];
				}
			}
			cursorLines[i].Draw ();
			
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
	
	Vector2	TransformGraphToScreen(Vector2 dataIn){
	
		
		float xAxisRange = xAxisMax - xAxisMin;
		float yAxisRange = yAxisMax - yAxisMin;
		Vector3 unitScalePos =  new Vector2((dataIn.x - xAxisMin) / xAxisRange, (dataIn.y - yAxisMin) / yAxisRange);
		
		return new Vector2(screenRectMin.x + unitScalePos.x * screenRectWidth, screenRectMin.y + unitScalePos.y * screenRectHeight);
			
	}
	
	Vector2	TransformGraphToGraphArea(Vector2 dataIn){
		
		
		float xAxisRange = xAxisMax - xAxisMin;
		float yAxisRange = yAxisMax - yAxisMin;
		Vector3 unitScalePos =  new Vector2((dataIn.x - xAxisMin) / xAxisRange, (dataIn.y - yAxisMin) / yAxisRange);
		
		return new Vector2(borderRectMin.x + unitScalePos.x * borderRectWidth, borderRectMin.y + unitScalePos.y * borderRectHeight);
		
	}
}
