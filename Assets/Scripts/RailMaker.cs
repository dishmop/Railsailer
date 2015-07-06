using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class RailMaker : MonoBehaviour {

	public static RailMaker singleton  = null;
	public GameObject editorPanel;
	public string loadLevelFilename;
	public string saveLevelFilename;
	
	public enum DebugRenderStyle{
		kNone,
		kLine,
		kPoint,
	};
	public DebugRenderStyle renderStyle = DebugRenderStyle.kLine;
	
	public Color lineColor;
	public float lineWidth;

	
	// This data structure is use to store info about each line semgnet in the track
	// and also about the locaiton of things on the track
	public class LocationData{
	
		public LocationData(float dist, float distAlongSegment, float segmentLength, Vector3 pos, Quaternion rotation, int startIndex){
			this.dist = dist;
			this.distAlongSegment = distAlongSegment;
			this.segmentLength = segmentLength;
			this.pos = pos;
			this.rotation = rotation;
			this.startIndex = startIndex;
		}
		
		public float 		dist;
		public float 		distAlongSegment;
		public float		segmentLength;
		public Vector3 		pos;
		public Quaternion 	rotation;
		public int 			startIndex;
	};
	
	
	float drawTriggerThreshold = 0.01f;
	float maxDistThreshold = 0.02f;
	
	List<Vector3> points = new List<Vector3>();
	LocationData[] locationData = new LocationData[0];
	
	float trackLength = -1;
	
	
	// Line renderer stuff
	bool lineIsDirty = false;
	
	public bool HasTrackData(){
		return (points.Count() > 2);
	}
	
	public bool HasLocationData(){
		return (locationData.Count() > 2);
	}
	
	
	public LocationData GetTrackLocation(Vector3 pos){
	
		float minDist = 100;
		int locIndex = 0;
		for (int i = 0; i < locationData.Count (); ++i){
			LocationData data = locationData[i];
			Vector3 vecToTrack = data.pos - pos;
			float distToTrack = vecToTrack.magnitude;
			if (distToTrack < minDist){
				locIndex = i;
				minDist = distToTrack;
			}
			
		}
		return locationData[locIndex]; 
		
	}
	

	public LocationData GetTrackLocation(float trackDist){
		if (trackDist < 0){
			int div = (int)(trackDist / trackLength);
			trackDist += (1 - div) * trackLength;
			
		}
		float useTrackDist = trackDist % trackLength;
		
		int index = LookupTrackSegment(useTrackDist);
		LocationData data = locationData[index];
		
		float distLeft = useTrackDist - data.dist;
		float segmentLength = data.segmentLength;
		float propAlongSement = distLeft / segmentLength;
		LocationData nextData = GetWrappedLocationData(data.startIndex + 1);
		Vector3 pos = Vector3.Lerp(data.pos, nextData.pos, propAlongSement);
		Quaternion rotation = Quaternion.Slerp (data.rotation, nextData.rotation, propAlongSement);
		
		LocationData newLocation = new LocationData(useTrackDist, distLeft, segmentLength, pos, rotation, data.startIndex);
		
		return newLocation;
	}
	
	public LocationData GetTrackLocation(float trackDist, LocationData nearLocation){
		return GetTrackLocation(trackDist);
	}
	
	public void Sparsify(){
		
		List<Vector3> newPoints = new List<Vector3>();
		
		// Ensure there is a minimum distance between each point
		for (int i = 0; i < points.Count(); i += 2){
			newPoints.Add (points[i]);
		}
	
		points = newPoints;
		lineIsDirty = true;
		
	}

	public void LoadTrack(){
		TextAsset asset = Resources.Load(BuildResourcePath ()) as TextAsset;
		if (asset != null){
			Debug.Log ("Loading asset");
			Stream s = new MemoryStream(asset.bytes);
			DeserializeTrack(s);
			Resources.UnloadAsset(asset);
		}	
		lineIsDirty = true;
		
		
	}
	
	public void SaveTrack(){
		#if UNITY_EDITOR		
		FileStream file = File.Create(BuildFullPath());
		
		SerializeTrack(file);
		
		
		file.Close();
		
		// Ensure the assets are all realoaded and the cache cleared.
		UnityEditor.AssetDatabase.Refresh();
		#endif
	}
	
	
	public void ClearTrack(){
		points = new List<Vector3>();
		lineIsDirty = true;

	}
	
	Vector3[] ApplyFilter(Vector3[] inData, float[] kernal){
		Vector3[] outData = new Vector3[inData.Count()];
		for (int i = 0; i < inData.Count(); ++i){
			outData[i] = Vector3.zero;
			for (int j = 0; j < kernal.Count (); ++j){
				int dataIndex = (i + j + inData.Count() - kernal.Count ()/2) % inData.Count();
				outData[i] += inData[dataIndex] * kernal[j];
			}
		}
		return outData;
	}
	
	float[] CreateGuassianKernal(int filterSize, float sigma){
		float mu = filterSize/2f;
		float[] result = new float[filterSize];
		float multiplier = 1f / (sigma * Mathf.Sqrt(2f * Mathf.PI));
		
		float sum = 0;
		for (int i = 0; i < filterSize; ++i){
			float x = (float)i;
			result[i] = multiplier * Mathf.Exp (-(x - mu) * (x - mu) / (2 * sigma * sigma));
			sum += result[i];
		}
		
		float normMul = 1f / sum;
		for (int i = 0; i < filterSize; ++i){
			result[i] = result[i] * normMul;
			//result[i] = 1f / filterSize;
		}
		
		return result;
		
	}
	
	void CalcTrackLength(){
		trackLength = 0;
		for (int i = 0; i < points.Count(); ++i){
			int thisIndex = i;
			int nextIndex = (i + 1) % points.Count();
			trackLength += (points[thisIndex] - points[nextIndex]).magnitude;
		}
		
	}
	
	void ConstructLocationData(){
		float distTravelled = 0;
		locationData = new LocationData[points.Count()];
		for (int i = 0; i < points.Count(); ++i){
			int thisIndex = i;
			int nextIndex = (i + 1) % points.Count();
			Vector3 segment = points[thisIndex] - points[nextIndex];
			float radians = Mathf.Atan2 (segment.y, segment.x);
			Quaternion rotation = Quaternion.Euler(0, 0, 270 + Mathf.Rad2Deg * radians);
			
			locationData[i] = new LocationData(distTravelled, 0, segment.magnitude, points[i], rotation, i);

			locationData[i].startIndex = i;
			
			distTravelled += locationData[i].segmentLength;
			
		}
	}
	
	LocationData GetWrappedLocationData(int index){
		return locationData[index % locationData.Count ()];
	}
	
	// Does a bnary search to locate the segment of the track that we are in
	int LookupTrackSegment(float dist){
		int lowerBoundIndex = 0;
		int upperBoundIndex = locationData.Count();
		
		while (true){
			int midIndex = (lowerBoundIndex + upperBoundIndex)/ 2;
			if (midIndex == lowerBoundIndex){
				return midIndex;
			}
			else{
				float midDist = GetWrappedLocationData(midIndex).dist;
				
				if (dist > midDist){
					lowerBoundIndex = midIndex;
				}
				else
					upperBoundIndex = midIndex;
				
				}
			}
	}				
		
		
	string CreateLoadFilename(){
		return loadLevelFilename;
	}
	
	string CreateSaveFilename(){
		return saveLevelFilename;
	}
	
	// We saving using the standard file system
	string BuildFullPath(){
		return Application.dataPath + "/Resources/Tracks/" + CreateSaveFilename() + ".bytes";
		
	}
	
	// We load using the resources
	string BuildResourcePath(){
		return "Tracks/" + CreateLoadFilename();
	}
	
	void  SerializeTrack(Stream outStream){
		BinaryWriter bw = new BinaryWriter(outStream);
		
		bw.Write (points.Count());
		for (int i = 0; i < points.Count(); ++i){
			bw.Write (points[i].x);
			bw.Write (points[i].y);
			bw.Write (points[i].z);
		}
	}
	
	void  DeserializeTrack(Stream inStream){
		BinaryReader br = new BinaryReader(inStream);
		
		int numPoint = br.ReadInt32();
		points = new List<Vector3>();
		
		for (int i = 0; i < numPoint; ++i){
			Vector3 newPoint = Vector3.zero;
			newPoint.x = br.ReadSingle();
			newPoint.y = br.ReadSingle();
			newPoint.z = br.ReadSingle();
			points.Add (newPoint);
		}

	}
	
	// Use this for initialization
	void Start () {
		LoadTrack()	;
		
	}
	
	// Update is called once per frame
	void Update () {
		// Only record points if we have the mouse button down
		if (Input.GetMouseButton(0) && !UI.singleton.mouseIsInUI){
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint( Input.mousePosition);
			mouseWorldPos.z = 0;
			
			// Also, only record points if we have moved sufficiently far from our last point
			if (points.Count() == 0 || (points[points.Count() -1] - mouseWorldPos).magnitude > drawTriggerThreshold){
				points.Add(mouseWorldPos);
				lineIsDirty = true;
			}
			
		}
		if (lineIsDirty){
			ReconstructLineRenderer();
			CalcTrackLength();
			ConstructLocationData();
			lineIsDirty = false;
		}
		RenderLine();
		
		if (GameConfig.singleton.enableEdit){
			GetComponent<LineRenderer>().enabled = true;
		}
		else{
			GetComponent<LineRenderer>().enabled = false;
		}
	}
	
	void ReconstructLineRenderer(){
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		if (!HasTrackData()){
			lineRenderer.SetVertexCount(0);
		}
		else
		{
			lineRenderer.SetVertexCount(points.Count()+2);
			lineRenderer.SetColors(lineColor, lineColor);
			lineRenderer.SetWidth(lineWidth, lineWidth);
			for (int i = 0; i < points.Count() + 2; ++i){
				lineRenderer.SetPosition(i, points[i % points.Count()]);
			}
		}
	}
	
	void RenderLine(){
		switch (renderStyle){
			case DebugRenderStyle.kLine:
			{
				for (int i = 0; i < points.Count(); ++i){
					int thisIndex = i;
					int nextIndex = (i + 1) % points.Count();
					Debug.DrawLine(points[thisIndex], points[nextIndex], Color.green);
				}
				break;
			}
			case DebugRenderStyle.kPoint:
			{
				for (int i = 0; i < points.Count(); ++i){
					int thisIndex = i;
					int nextIndex = (i + 1) % points.Count();
					Vector3 dir =  (points[nextIndex] - points[thisIndex]).normalized;
					Debug.DrawLine(points[thisIndex], points[thisIndex] + dir * drawTriggerThreshold * 0.5f, Color.red);
				}
				break;
			}
		}

	}
	
	public void SmoothLine(){
		
		
		// Ensure there is a minimum distance between each point
		for (int i = 0; i < points.Count(); ++i){
			bool doCalc = true;
			while (doCalc){
				int thisIndex = i;
				int nextIndex = (i + 1) % points.Count();
				if ((points[thisIndex] - points[nextIndex]).magnitude > maxDistThreshold){
					Vector3 avPoint = 0.5f * (points[thisIndex]  + points[nextIndex]);
					points.Insert(thisIndex + 1, avPoint);
					
				}
				else{
					doCalc = false;
				}
			}
		}
		
		// Smooth out the curve
		float[] kernal = CreateGuassianKernal(20, 50);
		Vector3[] pointVec = points.ToArray();
		pointVec = ApplyFilter(pointVec, kernal);
		points = pointVec.ToList();
		lineIsDirty = true;
		
	}
	
	
	
	//----------
	
	
	
	void Awake(){
		if (singleton != null) Debug.LogError ("Error assigning singleton");
		singleton = this;
	}
	
	
	
	void OnDestroy(){
		singleton = null;
	}
	

}
