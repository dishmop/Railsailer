using UnityEngine;
using Vectrosity;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UIVectorGraph : MonoBehaviour {

	
	public static float arrowSize = 0.05f;
	public Color axesCol;
	
	class GraphVector{
		public Vector2 fromPos;
		public Vector2 toPos;
		public VectorLine line;
		public Material lineMaterial;
		
	}
	
	float xAxisMin = -50;
	float xAxisMax = 50;
	float yAxisMin = -2;
	float yAxisMax = 2;
	
	Vector2 screenRectMin;
	Vector2 screenRectMax;
	float screenRectWidth;
	float screenRectHeight;
	
	VectorLine axesLine;
	Material axesMaterial;
	
	
	public void SetAxesRanges(float xMin, float xMax, float yMin, float yMax){
		xAxisMin = xMin;
		xAxisMax = xMax;
		yAxisMin = yMin;
		yAxisMax = yMax;
	}
	

	List<GraphVector> vectors = new List<GraphVector>();
	
	public int AddVector(Color col){
		GraphVector newGraphVector = new GraphVector();
		newGraphVector.fromPos = Vector2.zero;
		newGraphVector.toPos = Vector2.zero;
		Vector2[] points = new Vector2[6];
		newGraphVector.lineMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		newGraphVector.lineMaterial.color = col;
		newGraphVector.line = new VectorLine("Vector", points, newGraphVector.lineMaterial, 2, LineType.Discrete);
		vectors.Add (newGraphVector);
		return vectors.Count() - 1;
	}
	
	public void SetVector(int id, Vector2 fromPos, Vector2 toPos){
		vectors[id].fromPos = fromPos;
		vectors[id].toPos = toPos;

		
	}
	
	void DrawVectors(){
		Vector2[] points = new Vector2[6];
		
		for (int i = 0; i < vectors.Count(); ++i){
			GraphVector graphVector = vectors[i];
			
			ConstructArrow(points, TransformGraphToScreen(graphVector.fromPos), TransformGraphToScreen(graphVector.toPos));
			for (int j = 0; j < points.Count (); ++j){
				graphVector.line.points2[j] = points[j];
			}
			
			graphVector.line.Draw();
			
		}
	}

	
	void ConstructArrow(Vector2[] points, Vector3 fromPos, Vector3 toPos){
		// Figure out vector axes so arrow is visible to camera
		Vector3 fwVec = toPos - fromPos;
		Vector3 fwDir = fwVec.normalized;
		Vector3 centrePos = 0.5f * (fromPos + toPos);
		Vector3 camVec;
		if (Camera.main.orthographic){
			camVec = Camera.main.transform.rotation * new Vector3(0, 0, 1);
		}
		else{
			camVec = Camera.main.transform.position - centrePos;
		}
		Vector3 sideDir = Vector3.Cross(fwVec, camVec).normalized;
		
		// Now construct the arrow
		Vector3 sidePos0 = toPos - (fwDir + sideDir) * arrowSize * screenRectHeight;
		Vector3 sidePos1 = toPos - (fwDir - sideDir) * arrowSize * screenRectHeight;
		
		// The shaft
		points[0] = fromPos;
		points[1] = toPos;
		
		points[2] = toPos;
		points[3] = sidePos0;
		
		
		points[4] = toPos;
		points[5] = sidePos1;
		
	}
	
	void ConsructAxes(){
		Vector2[] points = new Vector2[4];
		axesLine = new VectorLine("Axes", points, axesMaterial, 2.0f, LineType.Discrete);
	}
	
	void DrawAxes(){
		
		Vector2[] points = new Vector2[4];
		
		// X-Axis
		points[0] = TransformGraphToScreen(new Vector2(xAxisMin, 0));
		points[1] = TransformGraphToScreen(new Vector2(xAxisMax, 0));
		
		// Y-Axis
		points[2] = TransformGraphToScreen(new Vector2(0, yAxisMin));
		points[3] = TransformGraphToScreen(new Vector2(0, yAxisMax));
		
		for (int i = 0; i < 4; ++i){
			axesLine.points2[i] = points[i];
		}
		
		axesMaterial.color = axesCol;
		axesLine.Draw();
	}

	// Use this for initialization
	void Start () {
		axesMaterial = Material.Instantiate(UI.singleton.vectrosityMaterialPrefab);
		axesMaterial.color = axesCol;
		
		SetAxesRanges(-10, 10, -10, 10);
		ConsructAxes();
//		int id = AddVector(Color.red);
//		SetVector(id, new Vector2(-5, -5), new Vector2(5, 5));
	
	}
	
	// Update is called once per frame
	void Update () {
		Rect rect = GetComponent<RectTransform>().rect;
		Vector3 screenRectMin3D = GetComponent<RectTransform>().TransformPoint(new Vector3(rect.min.x, rect.min.y, 0));
		Vector3 screenRectMax3D = GetComponent<RectTransform>().TransformPoint(new Vector3(rect.max.x, rect.max.y, 0));
		
		
		screenRectMin = new Vector2 (screenRectMin3D.x, screenRectMin3D.y);
		screenRectMax = new Vector2 (screenRectMax3D.x, screenRectMax3D.y);
		screenRectWidth = screenRectMax.x - screenRectMin.x;
		screenRectHeight = screenRectMax.y - screenRectMin.y;
		
		DrawAxes();
		DrawVectors();
	
	}
	
	Vector2	TransformGraphToScreen(Vector2 dataIn){
		
		
		float xAxisRange = xAxisMax - xAxisMin;
		float yAxisRange = yAxisMax - yAxisMin;
		Vector3 unitScalePos =  new Vector2((dataIn.x - xAxisMin) / xAxisRange, (dataIn.y - yAxisMin) / yAxisRange);
		
		return new Vector2(screenRectMin.x + unitScalePos.x * screenRectWidth, screenRectMin.y + unitScalePos.y * screenRectHeight);
		
	}
}
