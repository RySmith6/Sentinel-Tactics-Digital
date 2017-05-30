using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapMath : MonoBehaviour
{
	public static MapMath mapMath;
	public float angleCutoff = 60;
	List<Hex> hexes;
	List<Hex> LOSTrail;
	List<Hex> LOSMap;
	// Use this for initialization
	void Awake()
	{
		if (mapMath == null) {
			DontDestroyOnLoad(gameObject);
			mapMath=this;
		} else if(mapMath!=this) {
			Destroy(gameObject);
		}
	}
	void Start ()
	{
		hexes = Map.map.hexModels;// hexes;
		LOSTrail = new List<Hex> ();
		LOSMap = new List<Hex> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	public int RangeTo(Hex origin, Hex target)
	{
		return AxialCoord.DistanceInHexes (origin.GetPosition(), target.GetPosition()) + Mathf.Abs (origin.GetElevation()- target.GetElevation());
	}
	public int Rangeto(AxialCoord a, AxialCoord b)
	{
		Hex aExisting = Map.map.HexModelExistsAt (a);
		Hex bExisting = Map.map.HexModelExistsAt (b);
		if ((aExisting != null) && bExisting != null) {
			return RangeTo(aExisting,bExisting);
		}else
			return -1;
	}
	#region MoveCalculations
	public int MoveCost(List<Hex> path)
	{
		int result = 0;
		for (int i=1; i<path.Count; i++) {
			if(path[i].IsNeighbor(path[i-1]))
			{
				result+=1+Mathf.Max(0, path[i].GetElevation()- path[i-1].GetElevation());
			}
			else
				return 0;
		}
		return result;
	}
	
	public List<Hex> HexesWithinMove(Hex origin, int moveValue)
	{
		List<Hex> resultList = new List<Hex> ();
		resultList.Add (origin);
		foreach (Hex h in FindNeighbors(origin)) {
			
			if(h!=null&&(moveValue>= 1+ Mathf.Max(h.GetElevation()-origin.GetElevation(), 0)))
			{
				AddUniqueFromRange(resultList, HexesWithinMove(h, moveValue - (1+ Mathf.Max(0, h.GetElevation()-origin.GetElevation()))));
				//resultList.AddRange(HexesWithinMove(h, moveValue - (1+ Mathf.Abs(h.GetElevation()-origin.GetElevation()))));
			}
		}
		return resultList;
	}
	#endregion
	#region LOSRegion
	public bool PrimeWarLOS(AxialCoord origin, AxialCoord target)
	{
		return false;
	}

	public bool LOSRecursive(AxialCoord origin, AxialCoord target)
	{
		if (origin == target)
			return true;
		float tolerance = AxialCoord.AngleTolerance (origin, target);
		return 
			LOSRecursive (Map.map.HexModelExistsAt (origin), Map.map.HexModelExistsAt (target),tolerance);
				//&& LOSRecursive (Map.map.HexModelExistsAt (target), Map.map.HexModelExistsAt (origin),tolerance);
	}
	public bool LOSRecursive(Hex origin, Hex target, float tolerance =0)
	{
		if (origin.IsNeighbor (target))
			return true;
		if (target.HasOutline == Outline.cover)
			return false;
		if (tolerance == 0)
			tolerance = AxialCoord.AngleTolerance (origin.position, target.position);
		Vector2 originVector = AxialCoord.TwoDWorldPosFromAxial (origin);
		List<Hex> startingNeighbors = NeighborsBetween (origin, target);
		return startingNeighbors.Exists (h => h!=null && 
			//FoF rules for LOS
//												!(h.GetElevation()>origin.GetElevation()
//		                                       ||h.GetElevation ()>target.GetElevation ()
//		                                       ||h.HasOutline==Outline.cover)
			//Prime War LOS
			!((h.GetElevation()>origin.GetElevation()
				&&h.GetElevation ()>target.GetElevation ())
				||h.HasOutline==Outline.cover)
		                                 && LOSRecursive (origin, target, h,
		                 SkewedVertex(originVector,AxialCoord.TwoDWorldPosFromAxial(target),
		             SharedVerteces(origin,h)),tolerance));
		//return LOSRecursive (origin, target, origin, tolerance);
	}
	public bool LOSRecursive(Hex origin, Hex target, Hex recursion, Vector2 SkewVertex, float tolerance =0)
	{
		if (recursion == null)
			recursion = origin;
		if (recursion == target )
			return true;
		bool visible = false;

		foreach (Hex h in NeighborsBetween(recursion,target)) {
			if(h!=null && !visible)//&& !tailNeighbors.Exists(x=> h.IsNeighbor (x)))
			{
				if(h==target)
					return true;
				float angleFromTarget = AxialCoord.AngleBetween(target,h,origin);
				float angleTolerance = AxialCoord.AngleTolerance(h.position,origin.position);
				if((h.GetElevation()>origin.GetElevation()
					&&h.GetElevation ()>target.GetElevation ())
					||(h.HasOutline==Outline.cover)&&(h.GetElevation()+1>origin.GetElevation()
						&&h.GetElevation ()+1>target.GetElevation ()))
				{
					visible |= false;
				}
				else if(angleFromTarget> tolerance )// - (tolerance/AxialCoord.DistanceInHexes(origin,h)))
				{
					float skewedAngle = AxialCoord.AngleBetween(target,h,SkewVertex);
					if(skewedAngle>tolerance)
					{
//						if(skewedAngle>tolerance*1.1f)
//						{
//						Vector2 skewer = ClosestSkewedVertex(SkewVertex
//							,AxialCoord.TwoDWorldPosFromAxial (target)
//							,FindVerteces (h));
//						float skewThroughAngle = AxialCoord.AngleBetween(target,skewer,SkewVertex);
//						if(skewThroughAngle>tolerance)
					visible |= (h == target || h.IsNeighbor (target));
//						else
//							visible |= LOSRecursive(origin,target, h,SkewVertex, tolerance);
//						}
//						else
//							visible |= LOSRecursive(origin,target, h,SkewVertex, tolerance);
					}
					else
						visible |= LOSRecursive(origin,target, h,SkewVertex, tolerance);
				}
				else
					visible |= LOSRecursive(origin,target, h,SkewVertex, tolerance);
			}
		}
		return visible;
	}
	public void LOSFullProcedural(AxialCoord a)
	{
		Hex h = Map.map.HexModelExistsAt (a);
		if (h != null)
			LOSFullProcedural (h);
	}
	public void LOSFullProcedural(Hex origin)
	{
		origin.HideVisibility ();
		int originElevation = origin.GetElevation ();
		int maxrange = MaximumDistanceFrom (origin);
		#region neighbors
		foreach (Hex h in FindNeighbors(origin)) {
			if(h!=null)
			{
				h.HideVisibility();
			if(h.HasOutline==Outline.cover)
			{h.visibility=Visibility.visible;
				h.displayVisibility();
				h.visibility=Visibility.cover;
			}
			else if(h.GetElevation()>origin.GetElevation()){
				h.visibility=Visibility.wall;
				h.displayVisibility();
			}
			else{
				h.visibility=Visibility.visible;
				h.displayVisibility();
			}
			}

		}
		#endregion
		for (int x = 2; x<=maxrange; x++) {
			List<Hex> radiusFrom = HexesAtRadiusFrom(origin,x);
			if(radiusFrom.Count>0)
			{
				foreach(Hex h in radiusFrom)
				{
					if(h!=null)
					{
						h.HideVisibility();
						if(h.HasOutline == Outline.cover)
						{
							h.visibility = Visibility.cover;
							h.displayVisibility();
						}
						else
						{
					int elevation = h.GetElevation();
					List<Hex> neighbors = NeighborsBetween(h,origin);
							#region Void-Free
						if(!neighbors.Exists(n=>n==null))
						{
					if(neighbors.TrueForAll(n=> (n.visibility == Visibility.visible)))//||(n.visibility==Visibility.valley)))
							{
								if(neighbors.TrueForAll (n=> n.GetElevation()>elevation))
									h.visibility=Visibility.valley;
									else
										h.visibility=Visibility.visible;
							}
					else if (neighbors.TrueForAll(n=> (n.visibility == Visibility.cover)||(n.visibility == Visibility.wall)||(n.visibility == Visibility.invisible)))
						h.visibility=Visibility.invisible;
					else if(neighbors.TrueForAll(n=> (n.visibility == Visibility.valley)&&(n.GetElevation()>=elevation)))//&&h.GetElevation()<origin.GetElevation())
						h.visibility=Visibility.valley;
					else // (neighbors.Exists(n=> (n.visibility==Visibility.visible)||(n.visibility==Visibility.valley)))
					{
						//Hex neighbor = neighbors.Find(n=> (n.visibility==Visibility.visible)||(n.visibility==Visibility.valley));
						if(LOSRecursive(h.position,origin.position))
						{
							h.visibility=Visibility.visible;
						}
						else
									{
										if(elevation<originElevation)
											h.visibility=Visibility.valley;
										else
							h.visibility=Visibility.invisible;
									}
					}

					}
							#endregion
						else
						{
							if(neighbors.TrueForAll(n=> n==null))
								h.visibility=Visibility.invisible;
							else{
								neighbors.RemoveAll(n=> n==null);
								if(neighbors.Exists(n=> (n.visibility == Visibility.cover)||(n.visibility == Visibility.wall)||(n.visibility == Visibility.invisible)))
								{
									h.visibility=Visibility.invisible;
								}
								else if(neighbors.Exists(n=> (n.visibility == Visibility.valley)&&(n.GetElevation()>=elevation)))//&&h.GetElevation()<origin.GetElevation())
									h.visibility=Visibility.valley;
								else if(neighbors.Exists(n=> (n.visibility==Visibility.visible)||(n.visibility==Visibility.valley))){
									if(LOSRecursive(h.position,origin.position))
								{
									h.visibility=Visibility.visible;
								}
								else
									h.visibility=Visibility.invisible;
								}
							}
						}
						if(h.visibility==Visibility.visible&&elevation>originElevation)
							h.visibility=Visibility.wall;
						h.displayVisibility();
					}
					}
				}
			}
		}
	}
	public bool LineOfSightTo(Hex origin, Hex target, int midpoints=6, int lerpSample=6)
	{

		if (origin == target || origin.IsNeighbor (target))
			return true;
		else {
			int originElevation = origin.GetElevation();
			int targetElevation = target.GetElevation();
			List<Vector3> closestOnOrigin = ClosestVertecesBetween (origin, target,0.99f,midpoints);
			List<Vector3> closestOnTarget = ClosestVertecesBetween (target, origin,0.99f,midpoints);
			foreach (Vector3 v in closestOnOrigin) {
				foreach (Vector3 u in closestOnTarget) {List<Hex> throughHexes = FindHexesAlongLine (v, u, lerpSample);

					if (!ObstructionAlongLine(throughHexes, origin,target)) {
						//Debug.DrawRay(v,u,Color.green,10.0f);
						return true;
					}
				}
			}
			
		}
		return false;
		
	}
	public bool LineOfSightTo(AxialCoord a, AxialCoord b, int sampleSize, int lerpSample)
	{
		Hex aExisting = Map.map.HexModelExistsAt (a);
		Hex bExisting = Map.map.HexModelExistsAt (b);
		if ((aExisting != null) && bExisting != null) {
			return LineOfSightTo (aExisting, bExisting, sampleSize, lerpSample);
		} else
			return false;
	}
	public bool ManualLOS(Vector3 origin, Vector3 terminus)
	{
		Hex originHex = Map.map.HexModelExistsAt(AxialCoord.AxialFromWorld(origin));
		Hex terminusHex = Map.map.HexModelExistsAt(AxialCoord.AxialFromWorld(terminus));
		if (originHex == null || terminusHex == null)
			return false;
		bool visible = true;
		Vector2 flatOrigin = (Vector2)origin;
		Vector2 flatTerminus = (Vector2)terminus;
		RaycastHit2D [] hits = Physics2D.LinecastAll (flatOrigin, flatTerminus);
		foreach (RaycastHit2D hit in hits) {
			AxialCoord a = AxialCoord.AxialFromWorld(hit.centroid);
			Hex h = Map.map.HexModelExistsAt(a);
			if(h==null)
				return false;
			else
			{
				if(h.HasOutline==Outline.cover)
					return false;
				else if ((h.GetElevation()>originHex.GetElevation())||(h.GetElevation()>originHex.GetElevation()))
					return false;
			}
		}
		return visible;
	}
	public List<Hex> FindHexesAlongLine(Vector3 origin, Vector3 terminus, int numberOfSamples)
	{
		int n = numberOfSamples * AxialCoord.DistanceInHexes (AxialCoord.AxialFromWorld (origin), AxialCoord.AxialFromWorld (terminus));
		List<Hex> results = new List<Hex> ();
		for(int i = 0;i <= n;i++)
		{
			float percentage = ((float)i)* 1.0f/((float)n);
			Vector3 lerped = Vector3.Lerp(origin, terminus, percentage);
			AxialCoord temp = AxialCoord.AxialFromWorld(lerped);
			Hex exists = Map.map.HexModelExistsAt( temp);
			if(//exists!=null && 
			   !results.Contains(exists))
				results.Add(exists);
			
			// (cube_round(cube_lerp(a, b, 1.0/N * i)))
		}
		return results;
	}
	public bool ObstructionAlongLine(List<Hex> hexesOnLine, Hex origin, Hex target)
	{
		int originElevation = origin.GetElevation();
		int targetElevation = target.GetElevation ();
		foreach (Hex h in hexesOnLine) {
			if(h==null||h.HasOutline==Outline.cover)
				return true;
			else if (h != origin && h != target) {
				if (h.GetElevation() > originElevation || h.GetElevation() > targetElevation)
				{
					//Debug.DrawRay (v,u,Color.red,4.0f);
					return true;
				}
			}
		}
		return false;
	}
	public List<Vector3> MiddleVertecesBetween(Hex origin, Hex target)
	{
		Vector3 targetPosition = AxialCoord.FlatWorldPositionFromAxial(target);
		Vertex[] verteces = FindVerteces(origin);
		List<Vertex> vList = new List<Vertex> ();
		foreach(Vertex v in verteces)
		vList.Add (v);
		vList.Sort(delegate(Vertex x, Vertex y) {
			float xdistance = Vector3.Distance (targetPosition, x.location);
			float ydistance = Vector3.Distance (targetPosition, y.location);
			return ydistance.CompareTo(xdistance);
		});
		vList.RemoveRange (4, 2);
		vList.RemoveRange (0, 2);
		List<Vector3> vectors = new List<Vector3> (vList.Count);
		foreach (Vertex v in vList)
			vectors.Add (v.location);
		return vectors;
	}
	public List<Vector3> ClosestVertecesBetween(Hex origin, Hex target, float fromEdge, int numberOfMidPoints)
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();
		List<Vector3> vectorResults = new List<Vector3> ();
		List<Vector3> midpointVectors = new List<Vector3> ();
		Vector3 targetPosition = AxialCoord.FlatWorldPositionFromAxial(target);
		float distanceFromOriginToTarget = Vector3.Distance(targetPosition,AxialCoord.FlatWorldPositionFromAxial(origin));
		Vertex[] verteces = FindVerteces (origin);
		Vertex cachedNosey = null;
		for (int i =0; i< verteces.Length; i++) {
			bool nosyNeighbor = false;
			float distance = Vector3.Distance (targetPosition, verteces [i].location);
			if ( distance<= distanceFromOriginToTarget) {
				foreach (Hex h in verteces[i].neighbors)
				{
					if((Vector3.Distance(targetPosition,AxialCoord.FlatWorldPositionFromAxial(h))<distance)
					   && h.GetElevation()>origin.GetElevation())
						nosyNeighbor=true;
				}
				if(nosyNeighbor)
				{
					if(cachedNosey!=null)
					{
						if(Vector3.Distance (targetPosition, cachedNosey.location)>distance)
					{
							vectorResults.Add (Vector3.Lerp(origin.GetWorldPosition(), verteces [i].location, fromEdge));
					}
					else
							vectorResults.Add (Vector3.Lerp(origin.GetWorldPosition(), cachedNosey.location, fromEdge));
					}
					cachedNosey = verteces[i];

				}
				else
					vectorResults.Add (Vector3.Lerp(origin.GetWorldPosition(), verteces [i].location, fromEdge));
			}
		}
		for(int i =1; i<vectorResults.Count;i++)
		{
			for (int x = 1; x<=numberOfMidPoints; x++)
			{
				midpointVectors.Add(Vector3.Lerp(vectorResults[i],vectorResults[i-1],(((float)x)*1f)/(numberOfMidPoints+1)));
			}
		}
		vectorResults.AddRange (midpointVectors);
		
		return midpointVectors;
		
		
	}
	public float DistanceBetweenHexes(Hex origin, Hex target)
	{
		Vector3 targetPosition = AxialCoord.FlatWorldPositionFromAxial(target);
		return Vector3.Distance(targetPosition,AxialCoord.FlatWorldPositionFromAxial(origin));
	}
	public List<Hex> LOSWithin(Hex origin, int radius=1,  int midpoints =6, int lerp=6, Hex trail = null)
	{
		List<Hex> resultList = new List<Hex> ();
		if (origin == null) {
			return resultList;
		}
		if (trail == null) {
			trail=origin;
		}
		if(LineOfSightTo(origin,trail,2,2))
		{
		resultList.Add (trail);
		if (radius > 0) {
			foreach (Hex h in FindNeighbors(trail)) {
				if(h!=null)
				{
						AddUniqueFromRange (resultList,LOSWithin(origin, radius -1,  midpoints,lerp, h));
					}
				}
			}

		}
		return resultList;

	}
	public List<Hex> LineOfSightWithin(AxialCoord position, int radius, int midpoints =6, int lerp=6)
	{
		List<Hex> resultList = new List<Hex> ();
		Hex origin = Map.map.HexModelExistsAt (position);
		if(origin != null)
		{
			if(radius>0){
				List<Hex> candidates = HexesInRadiusFrom(origin, radius);
			foreach (Hex h in candidates) // HexesInRadiusFrom(origin, radius))
			{
				if(LineOfSightTo(origin, h))
				{
					resultList.Add(h);
				}
			}
			}
			else{
				foreach (Hex h in Map.map.hexModels)
				{
					if(LineOfSightTo(origin, h, midpoints, lerp))
					{
						resultList.Add(h);
					}
				}
			}
			return resultList;
		}
		return null;
	}
	#endregion


	int MaximumDistanceFrom(Hex origin)
	{
		int distanceFromOrigin = 0;
		foreach (Hex h in Map.map.hexModels) {
			if(h!=null)
			{
				int distance = AxialCoord.DistanceInHexes(h,origin);
			distanceFromOrigin = Mathf.Max(distanceFromOrigin,distance);
			}
		}
		return distanceFromOrigin;
	}
	List<Hex> NeighborsBetween(Hex start, Hex finish)
	{
		List<Hex> nearestNeighbors;
		AxialCoord finishRelative = finish.position - start.position;
		AxialCoord temp = start.position;
		#region cardinal
		if ((finishRelative.q == 0) || (finishRelative.r == 0) || (finishRelative.q + finishRelative.r == 0)) {
			nearestNeighbors = new List<Hex>(1);
			if (finishRelative.q == 0) {
				if ( finishRelative.r>0)
					temp.r++;
				else
					temp.r--;
			} else if (finishRelative.r == 0) {
				if (finishRelative.q>0)
					temp.q++;
				else
					temp.q--;
			} else if (finishRelative.q + finishRelative.r == 0) {
				if (finishRelative.q>0) {
					temp.q++;
					temp.r--;
				} else {
					temp.q--;
					temp.r++;
				}
			}
			nearestNeighbors.Add(Map.map.HexModelExistsAt (temp));
		}
		#endregion
		else {
			nearestNeighbors = new List<Hex>(2);
			int absQ = Mathf.Abs (finishRelative.q);
			int absR = Mathf.Abs (finishRelative.r);
			int absQR = Mathf.Abs (finishRelative.q+finishRelative.r);

			if( (absQR<= absQ) || (absQR<= absR ))
			{


				if(absR<absQ)
				{
					if (finishRelative.q>0) {
							temp.q++;
							temp.r--;
						} else {
							temp.q--;
							temp.r++;
						}
						nearestNeighbors.Add (Map.map.HexModelExistsAt (temp));
						temp = start.position;

					if (finishRelative.q>0)
						temp.q++;
					else
						temp.q--;
					nearestNeighbors.Add (Map.map.HexModelExistsAt (temp));
				}
				else
				{
					if (finishRelative.q>0) {
							temp.q++;
							temp.r--;
						} else {
							temp.q--;
							temp.r++;
						}
						nearestNeighbors.Add (Map.map.HexModelExistsAt (temp));
						temp = start.position;
					if ( finishRelative.r>0)
					temp.r++;
					else
						temp.r--;
					nearestNeighbors.Add (Map.map.HexModelExistsAt (temp));
				}
			}
			else
			{
				if (finishRelative.q>0)
					temp.q++;
				else
					temp.q--;
				nearestNeighbors.Add (Map.map.HexModelExistsAt (temp));
				temp=start.position;
			
				if ( finishRelative.r>0)
					temp.r++;
				else
					temp.r--;
				nearestNeighbors.Add (Map.map.HexModelExistsAt (temp));
			
			}

		}
		return nearestNeighbors;
	}
	public static List<AxialCoord> PositionsAtRadiusFrom(AxialCoord origin, int radius)
	{
		List<AxialCoord> resultList = new List<AxialCoord> ();

			AxialCoord hex = AxialCoord.Hex_Add(origin, AxialCoord.Hex_Scale(AxialCoord.hex_direction(4),radius));
			//in each cardinal direction, i 
			
			for(int i = 0; i<6; i++)
			{
				//and adding additional neighbors for larger rings
				for(int a =0; a<radius;a++)
				{
					resultList.Add(hex);
					hex = AxialCoord.hex_neighbor(hex, i);
				}
			}
		
		return resultList;
	}
	public List<Hex> HexesAtRadiusFrom(Hex h, int radius)
	{
		List<Hex> resultHexes = new List<Hex> ();
		foreach (AxialCoord a in PositionsAtRadiusFrom(h.position, radius)) {
			resultHexes.Add (Map.map.HexModelExistsAt(a));
		}
		resultHexes.RemoveAll(n=> n==null);
		return resultHexes;
	}
	public static List<AxialCoord> PositionsInRadiusFrom(AxialCoord origin, int radius)
	{
		List<AxialCoord> resultList = new List<AxialCoord> ();
		//Center hex
		resultList.Add(origin);
		//x<radius, in the case of the tile, three
		
		for (int x =1; x<=radius; x++) {
			AxialCoord hex = AxialCoord.Hex_Add(origin, AxialCoord.Hex_Scale(AxialCoord.hex_direction(4),x));
			//in each cardinal direction, i 
			
			for(int i = 0; i<6; i++)
			{
				//and adding additional neighbors for larger rings
				for(int a =0; a<x;a++)
				{
					resultList.Add(hex);
					hex = AxialCoord.hex_neighbor(hex, i);
				}
			}
		}
		return resultList;
	}
	public List<Hex> HexesInRadiusFrom(Hex origin, int radius)
	{
		if (radius < 0) {
			return hexes;
		} else {
			List<Hex> resultList = new List<Hex> ();
			resultList.Add (origin);
			foreach (Hex h in FindNeighbors(origin)) {
				
				if (h!=null&&(radius >0)){
					AddUniqueFromRange (resultList, HexesInRadiusFrom (h, radius - 1));
					//resultList.AddRange(HexesInRadiusFrom(h, radius - 1));
				}
			}
			//resultList.
			return resultList;
		}
	}
	public Vector2 SkewedVertex(Vector2 origin, Vector2 target, List<Vertex> originalVerteces)
	{
		float distanceBetween = 0;
		Vector2 vertexLocation = origin;
		foreach (Vertex v in originalVerteces) {
			if( Vector2.Distance( (Vector2)v.location, target)>distanceBetween)
			{
				distanceBetween = Vector2.Distance( (Vector2)v.location, target);
				vertexLocation = Vector2.Lerp(origin, (Vector2)v.location, 0.99f);
			}
		}
		return vertexLocation;
	}
	public Vector2 ClosestSkewedVertex(Vector2 origin, Vector2 target, Vertex[] originalVerteces)
	{

		Vector2 vertexLocation = (Vector2)originalVerteces[0].location;
		float distanceToOrigin = Vector2.Distance (vertexLocation, origin);
		float distanceToTarget = Vector2.Distance (vertexLocation, target);
		foreach (Vertex v in originalVerteces) {
			Vector2 tempVector = (Vector2)v.location;
			if( distanceToOrigin+distanceToTarget> Vector2.Distance (tempVector,origin)+Vector2.Distance (tempVector,target))
			{
				vertexLocation = Vector2.Lerp(origin, (Vector2)v.location, 0.99f);
				distanceToOrigin = Vector2.Distance (vertexLocation, origin);
				distanceToTarget = Vector2.Distance (vertexLocation, target);
			}
		}
		return vertexLocation;
	}
	public List<Vertex> SharedVerteces(Hex origin, Hex neighbor)
	{
		return Map.map.verteces.FindAll (v => v.AdjacentTo (origin) && v.AdjacentTo (neighbor));
	}
	public void AddVertex(Vertex newVertex)
	{
		Map.map.AddVertex (newVertex);
		//return verteces.Find (v => v == newVertex);
	}
	void AddUniqueFromRange(List<Hex> original, List<Hex> additive)
	{
		foreach (Hex h in additive) {
			if (!original.Contains(h))
				original.Add(h);
		}
	}
	public static void AddUnique(List<Hex> original, Hex toBeAdded)
	{
		bool alreadyExists = false;
		foreach(Hex h in original)
		{
			alreadyExists |= (h == toBeAdded);
		}
		if (!alreadyExists) {
			original.Add(toBeAdded);
		}
	}
	static Hex[] FindNeighbors(Hex h)
	{
		Hex[] neighbors = new Hex[6];
		AxialCoord hexPos = h.GetPosition ();
		for(int i = 0; i<6; i++)
		{
			neighbors[i] = Map.map.HexModelExistsAt(AxialCoord.Hex_Add(hexPos, AxialCoord.hex_direction(i)));
		}
		return neighbors;
	}
	static Hex[] FindNeighbors(AxialCoord a)
	{
		Hex[] neighbors = new Hex[6];
		for(int i = 0; i<6; i++)
		{
			neighbors[i] = Map.map.HexModelExistsAt(AxialCoord.Hex_Add(a, AxialCoord.hex_direction(i)));
		}
		return neighbors;
	}
	Vertex[] FindVerteces(Hex h)
	{
		return Map.map.verteces.FindAll (v => v.AdjacentTo (h)).ToArray ();
	}
	public int SmoothHeightFromNeighbors(AxialCoord a)
	{
		Hex h = Map.map.HexModelExistsAt (a);
		Hex[] neighbors = FindNeighbors (a);
		int sum = 0;
		int count = 0;
		int avgElevation = 0;
		float randomValue = 0;
		if (h != null) {
			count++;
			sum += h.GetElevation ();
		}
		for(int i =0; i<neighbors.Length; i++)
		{
			if(neighbors[i]!=null)
			{
				count++;
				sum +=neighbors[i].GetElevation();
			}
		}
		avgElevation= Mathf.RoundToInt (sum / count);
		randomValue = Random.value;
		if (randomValue < 0.4f) {
			return avgElevation;
		} else if (randomValue < 0.8f) {
			return Mathf.Min (avgElevation + 1, 4);
		} else {
			return Mathf.Max(avgElevation-1,1);
		}
		
	}
	public static List<AxialCoord> HexesBetween(Hex origin, Hex endpoint)
	{
		List<AxialCoord> results = new List<AxialCoord> ();
		Vector3 originCenter = origin.transform.position;
		Vector3 endpointCenter = endpoint.transform.position;
		int hexDistance = AxialCoord.DistanceInHexes (origin.GetPosition (), endpoint.GetPosition ());
		for(int i = 1;i < hexDistance;i++)
		{
			float percentage = ((float)i)* 1.0f/((float)hexDistance);
			Vector3 lerped = Vector3.Lerp(originCenter, endpointCenter, percentage);
			AxialCoord temp = AxialCoord.AxialFromWorld(lerped);
			if(!results.Contains(temp))
				results.Add(temp);
		}
		return results;
	}
	public static AxialCoord CounterClockwiseAfterBlank(Hex h, Hex[] potentialNeighbors, int skip = 0)
	{
		bool blank = false;
		for (int i = 0; i<6; i++) {
			bool found = false;
			for(int x = 0; x< potentialNeighbors.Length && !found; x++)
			{
				if((potentialNeighbors[x]!=null)&&(potentialNeighbors[x].position == AxialCoord.Hex_Add(h.position, AxialCoord.hex_direction(i))))
				{
					if(blank)
						if(skip>0)
							return CounterClockwiseAfterBlank(potentialNeighbors[x], FindNeighbors(potentialNeighbors[x]), skip-1);
						return potentialNeighbors[x].position;
				}
			}
			if(!found)
				blank=true;
		}
		return AxialCoord.Hex_Add (h.position, AxialCoord.hex_direction (0));
	}
	public static Dictionary<Hex, AxialCoord> Rotations(List<Hex> hexesToRotate)
	{
		Dictionary<Hex, AxialCoord> resultDictionary = new Dictionary<Hex, AxialCoord> (hexesToRotate.Count);
		if(hexesToRotate.Count>1)
		{
			if(hexesToRotate.Count>6)
			{
				List<Hex> innerHexes = new List<Hex>(hexesToRotate.Count-6);
				innerHexes =FindInnerHexes (hexesToRotate);
				Hex[] outermostHexes = new Hex[hexesToRotate.Count];
				hexesToRotate.CopyTo (outermostHexes);
				foreach (Hex h in outermostHexes) {
					resultDictionary.Add (h, CounterClockwiseAfterBlank(h, FindNeighbors(h), 1)); 
				}
			}
			else
			{
				Hex[] outermostHexes = new Hex[hexesToRotate.Count];
				hexesToRotate.CopyTo (outermostHexes);
				foreach (Hex h in outermostHexes) {
					resultDictionary.Add (h, CounterClockwiseAfterBlank(h, FindNeighbors(h))); 
				}
			}
		
		}
		else
			resultDictionary.Add (hexesToRotate[0], hexesToRotate[0].position);
			return resultDictionary;
	}
	public static void RotateSelection()
	{
		List<Hex> toBeRotated = Map.map.hexesUnderEdit;
		Dictionary <Hex, AxialCoord> finalPositions = Rotations (toBeRotated);
		foreach (Hex h in toBeRotated) {
			if(finalPositions.ContainsKey(h))
			{
				h.SetPosition(finalPositions[h]);
				h.transform.Rotate(0, 0, -60);
			}
		}
	}
	public static void RotateAboutPosition(AxialCoord about)
	{
		List<Hex> toBeRotated = Map.map.hexesUnderEdit;
		Dictionary <Hex, AxialCoord> finalPositions = new Dictionary<Hex, AxialCoord> (toBeRotated.Count);
		int maxDistanceFromOrigin = 0;
		//CubeCoord offset = new CubeCoord(
		foreach (Hex h in toBeRotated) {
			AxialCoord offset = h.position - about;
			//finalPositions.Add(h, 
			h.SetPosition(AxialCoord.cube_to_hex(CubeCoord.RotateCounter(AxialCoord.hex_to_cube(offset)))+ about);
			h.Rotate(); //.transform.Rotate(0, 0, -60);
		}
	}
	public static void FlipAboutPosition(AxialCoord about)
	{
		List<Hex> toBeRotated = Map.map.hexesUnderEdit;
		foreach (Hex h in toBeRotated) {
			AxialCoord offset = h.position - about;
			//finalPositions.Add(h, 
			h.SetPosition(offset.FlipAboutOrigin() + about);// AxialCoord.cube_to_hex(CubeCoord.RotateCounter(AxialCoord.hex_to_cube(offset)))+ about);
			h.FlipFace(); //.transform.Rotate(0, 0, -60);
		}
	}
	public static List<Hex> FindInnerHexes(List<Hex> Hexes)
	{
		if (Hexes.Count >= 7) {
			List<AxialCoord> foundOnce = new List<AxialCoord> (Hexes.Count);
			List<Hex> innerHexes = new List<Hex> (Hexes.Count - 6);
			foreach(Hex h in Hexes)
			{
				int foundInSelection = 0;
				foreach(Hex n in FindNeighbors(h))
				{
					if(Hexes.Contains(n))
						foundInSelection++;
				}
				if(foundInSelection>4)
					innerHexes.Add(h);
			}
			return innerHexes;
		}
		return null;
 	}
	public static AxialCoord FindCenterOfSelection()
	{
//		int maxQ = -25;
//		int maxR = -25;
//		int minQ = 25;
//		int minR = 25;
		int totalR = 0;
		int totalQ = 0;
		int underEdit = Map.map.hexesUnderEdit.Count;
		if (underEdit > 0) {

			foreach (Hex h in Map.map.hexesUnderEdit) {
//				maxQ = Mathf.Max (maxQ, h.position.q);
//				maxR = Mathf.Max (maxR, h.position.r);
//				minQ = Mathf.Min (minQ, h.position.q);
//				minR = Mathf.Min (minR, h.position.r);
				totalQ += h.position.q;
				totalR += h.position.r;
			}
		

			int 
//				delQ = maxQ + minQ;
//			if (delQ < 0)
//				delQ = Mathf.RoundToInt (((maxQ + minQ) / (2)) - 0.1f);
//			else
//				delQ = Mathf.RoundToInt ((delQ / (2)) + 0.1f);
			delQ = Mathf.RoundToInt (totalQ / underEdit);
			int 
//				delR = maxR + minR;
//			if (delR < 0)
//				delR = Mathf.RoundToInt (((maxR + minR) / (2)) - 0.1f);
//			else
//				delR = Mathf.RoundToInt ((delR / (2)) + 0.1f);
			delR = Mathf.RoundToInt (totalR / underEdit);
			AxialCoord offset = new AxialCoord (delQ, delR);
			return offset;
		}
		return new AxialCoord(0,0);
	}
}


